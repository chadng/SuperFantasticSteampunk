using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.BattleStates
{
    using InventoryItem = KeyValuePair<string, int>;

    enum ThinkActionType { None, Attack, Defend, UseItem }

    class ThinkAction
    {
        #region Instance Fields
        public ThinkActionType Type { get; private set; }
        public string OptionName { get; private set; }
        public PartyMember Actor { get; set; }
        public PartyMember Target { get; set; }
        #endregion

        #region Constructors
        public ThinkAction(ThinkActionType type, string optionName, PartyMember actor = null, PartyMember target = null)
        {
            Type = type;
            OptionName = optionName;
            Actor = actor;
            Target = target;
        }
        #endregion
    }

    class Think : BattleState
    {
        #region Instance Fields
        private ThinkActionType currentThinkActionType;
        private ThinkAction currentThinkAction; // Assigned to just before SelectTarget state is pushed
        private int currentOptionNameIndex;
        private PartyMember currentPartyMember;
        private List<string> optionNames;
        private Dictionary<CharacterClass, List<string>> weaponOptionNames;
        private List<string> itemOptionNames;
        private List<ThinkAction> actions;
        private InputButtonListener inputButtonListener;
        #endregion

        #region Constructors
        public Think(Battle battle)
            : base(battle)
        {
            currentThinkActionType = ThinkActionType.None;
            currentThinkAction = null;
            currentOptionNameIndex = 0;
            currentPartyMember = null;
            optionNames = null;

            weaponOptionNames = new Dictionary<CharacterClass, List<string>>();
            foreach (CharacterClass characterClass in Enum.GetValues(typeof(CharacterClass)))
                weaponOptionNames.Add(characterClass, new List<string>().Tap(names => names.Add("Cancel")));

            itemOptionNames = new List<string>().Tap(names => names.Add("Cancel"));
            actions = new List<ThinkAction>(battle.PlayerParty.Count);

            inputButtonListener = new InputButtonListener(new Dictionary<InputButton, ButtonEventHandlers> {
                { InputButton.Up, new ButtonEventHandlers(down: choosePreviousOption) },
                { InputButton.Down, new ButtonEventHandlers(down: chooseNextOption) },
                { InputButton.A, new ButtonEventHandlers(down: showAttackMenu, up: hideAttackMenu) },
                { InputButton.B, new ButtonEventHandlers(down: showDefendMenu, up: hideDefendMenu) },
                { InputButton.X, new ButtonEventHandlers(down: showItemMenu, up: hideItemMenu) },
                { InputButton.Y, new ButtonEventHandlers(up: cancelAction) }
            });
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            foreach (CharacterClass characterClass in Enum.GetValues(typeof(CharacterClass)))
            {
                foreach (InventoryItem item in battle.PlayerParty.WeaponInventories[characterClass].GetSortedItems())
                    weaponOptionNames[characterClass].Add(item.Key);
            }
            foreach (InventoryItem item in battle.PlayerParty.ItemInventory.GetSortedItems())
                itemOptionNames.Add(item.Key);

            currentPartyMember = battle.PlayerParty[0];
        }

        public override void Finish()
        {
            ChangeState(new Act(battle));
        }

        public override void Resume(BattleState previousBattleState)
        {
            base.Resume(previousBattleState);

            if (previousBattleState is SelectTarget)
            {
                if (currentThinkAction.Target != null)
                {
                    actions.Add(currentThinkAction);
                    getNextPartyMember();
                }
                else // i.e. Cancel
                {
                    currentThinkAction = null;
                    initThinkActionTypeMenu(ThinkActionType.None);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            inputButtonListener.Update(gameTime);
        }

        private void choosePreviousOption()
        {
            if (currentThinkActionType == ThinkActionType.None || optionNames == null)
                return;

            if (--currentOptionNameIndex < 0)
                currentOptionNameIndex = 0;

            Logger.Log("Selected option: " + optionNames[currentOptionNameIndex]);
        }

        private void chooseNextOption()
        {
            if (currentThinkActionType == ThinkActionType.None || optionNames == null)
                return;

            if (++currentOptionNameIndex >= optionNames.Count)
                currentOptionNameIndex = optionNames.Count - 1;

            Logger.Log("Selected option: " + optionNames[currentOptionNameIndex]);
        }

        private void showAttackMenu()
        {
            if (currentThinkActionType == ThinkActionType.None)
                initThinkActionTypeMenu(ThinkActionType.Attack);
        }

        private void hideAttackMenu()
        {
            if (currentThinkActionType == ThinkActionType.Attack)
                selectAction();
        }

        private void showDefendMenu()
        {
            if (currentThinkActionType == ThinkActionType.None)
                initThinkActionTypeMenu(ThinkActionType.Defend);
        }

        private void hideDefendMenu()
        {
            if (currentThinkActionType == ThinkActionType.Defend)
                selectAction();
        }

        private void showItemMenu()
        {
            if (currentThinkActionType == ThinkActionType.None)
                initThinkActionTypeMenu(ThinkActionType.UseItem);
        }

        private void hideItemMenu()
        {
            if (currentThinkActionType == ThinkActionType.UseItem)
                selectAction();
        }

        private void cancelAction()
        {
            if (currentThinkActionType == ThinkActionType.None && actions.Count > 1)
            {
                ThinkAction lastAction = actions[actions.Count - 1];
                actions.RemoveAt(actions.Count - 1);
                currentPartyMember = lastAction.Actor;
            }
        }

        private void initThinkActionTypeMenu(ThinkActionType thinkActionType)
        {
            setThinkActionType(thinkActionType);
            selectDefaultOptionName();
        }

        private void selectAction()
        {
            if (currentOptionNameIndex == 0) // i.e. Cancel
            {
                initThinkActionTypeMenu(ThinkActionType.None);
                return;
            }

            currentThinkAction = new ThinkAction(currentThinkActionType, optionNames[currentOptionNameIndex], currentPartyMember);
            PushState(new SelectTarget(battle, currentThinkAction));
        }

        private void setThinkActionType(ThinkActionType thinkActionType)
        {
            currentThinkActionType = thinkActionType;
            if (thinkActionType == ThinkActionType.None)
                optionNames = null;
            else if (thinkActionType == ThinkActionType.UseItem)
                optionNames = itemOptionNames;
            else
                optionNames = weaponOptionNames[currentPartyMember.CharacterClass];
        }

        private void selectDefaultOptionName()
        {
            if (optionNames == null || optionNames.Count == 1)
                currentOptionNameIndex = 0;
            else
                currentOptionNameIndex = 1;                
        }

        private void getNextPartyMember()
        {
            if (actions.Count == battle.PlayerParty.Count)
                Finish();
            else
            {
                IEnumerable<PartyMember> finishedPartyMembers = actions.Select<ThinkAction, PartyMember>(thinkAction => thinkAction.Actor);
                currentPartyMember = battle.PlayerParty.Find(partyMember => !finishedPartyMembers.Contains(partyMember));
            }
        }
        #endregion
    }
}
