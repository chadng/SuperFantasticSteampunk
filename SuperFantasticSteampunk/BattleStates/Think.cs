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
        private Dictionary<CharacterClass, List<string>> shieldOptionNames;
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
            optionNames = null;

            weaponOptionNames = new Dictionary<CharacterClass, List<string>>();
            shieldOptionNames = new Dictionary<CharacterClass, List<string>>();
            itemOptionNames = new List<string>();
            foreach (CharacterClass characterClass in Enum.GetValues(typeof(CharacterClass)))
            {
                weaponOptionNames.Add(characterClass, new List<string>());
                shieldOptionNames.Add(characterClass, new List<string>());
            }

            actions = new List<ThinkAction>(battle.PlayerParty.Count);

            inputButtonListener = new InputButtonListener(new Dictionary<InputButton, ButtonEventHandlers> {
                { InputButton.Up, new ButtonEventHandlers(down: choosePreviousOption) },
                { InputButton.Down, new ButtonEventHandlers(down: chooseNextOption) },
                { InputButton.A, new ButtonEventHandlers(down: showAttackMenu, up: hideAttackMenu) },
                { InputButton.B, new ButtonEventHandlers(down: showDefendMenu, up: hideDefendMenu) },
                { InputButton.X, new ButtonEventHandlers(down: showItemMenu, up: hideItemMenu) },
                { InputButton.Y, new ButtonEventHandlers(up: cancelAction) }
            });

            BattleStateRenderer = new ThinkRenderer(this);
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            repopulateOptionNames();
            getNextPartyMember();
        }

        public override void Finish()
        {
            thinkAboutEnemyPartyActions();
            ChangeState(new Act(Battle, actions));
        }

        public override void Resume(BattleState previousBattleState)
        {
            base.Resume(previousBattleState);

            if (previousBattleState is SelectTarget)
            {
                if (currentThinkAction.Target != null)
                {
                    Inventory inventory = getInventoryFromThinkActionType(currentThinkAction.Type);
                    if (inventory != null)
                        inventory.UseItem(currentThinkAction.OptionName);
                    repopulateOptionNames();
                    actions.Add(currentThinkAction);
                    getNextPartyMember();
                }

                currentThinkAction = null;
                initThinkActionTypeMenu(ThinkActionType.None);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (actions.Count == Battle.PlayerParty.Count)
            {
                Finish();
                return;
            }
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
            if (currentThinkActionType == ThinkActionType.None && actions.Count > 0)
            {
                ThinkAction lastAction = actions[actions.Count - 1];
                actions.RemoveAt(actions.Count - 1);
                currentPartyMember = lastAction.Actor;
                Inventory inventory = getInventoryFromThinkActionType(lastAction.Type);
                if (inventory != null)
                    inventory.AddItem(lastAction.OptionName);
                repopulateOptionNames();
                Logger.Log("Back to action selection for party member " + (actions.Count + 1).ToString());
            }
        }

        private void initThinkActionTypeMenu(ThinkActionType thinkActionType)
        {
            setThinkActionType(thinkActionType);
            Logger.Log(thinkActionType.ToString() + " action menu opened");
            selectDefaultOptionName();
            if (optionNames != null)
                Logger.Log("Selected option: " + optionNames[currentOptionNameIndex]);
        }

        private void selectAction()
        {
            if (currentOptionNameIndex == 0) // i.e. Cancel
            {
                initThinkActionTypeMenu(ThinkActionType.None);
                return;
            }

            currentThinkAction = new ThinkAction(currentThinkActionType, optionNames[currentOptionNameIndex], currentPartyMember);
            PushState(new SelectTarget(Battle, currentThinkAction));
        }

        private void setThinkActionType(ThinkActionType thinkActionType)
        {
            currentThinkActionType = thinkActionType;
            switch (thinkActionType)
            {
            case ThinkActionType.Attack: optionNames = weaponOptionNames[currentPartyMember.CharacterClass]; break;
            case ThinkActionType.Defend: optionNames = shieldOptionNames[currentPartyMember.CharacterClass]; break;
            case ThinkActionType.UseItem: optionNames = itemOptionNames; break;
            default: optionNames = null; break;
            }
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
            if (actions.Count < Battle.PlayerParty.Count)
            {
                IEnumerable<PartyMember> finishedPartyMembers = actions.Select<ThinkAction, PartyMember>(thinkAction => thinkAction.Actor);
                currentPartyMember = Battle.PlayerParty.Find(partyMember => !finishedPartyMembers.Contains(partyMember));
                Logger.Log("Action selection for party member " + (actions.Count + 1).ToString());
            }
        }

        private Inventory getInventoryFromThinkActionType(ThinkActionType thinkActionType)
        {
            switch (thinkActionType)
            {
            case ThinkActionType.Attack: return Battle.PlayerParty.WeaponInventories[currentPartyMember.CharacterClass];
            case ThinkActionType.Defend: return Battle.PlayerParty.ShieldInventories[currentPartyMember.CharacterClass];
            case ThinkActionType.UseItem: return Battle.PlayerParty.ItemInventory;
            default: return null;
            }
        }

        private void repopulateOptionNames()
        {
            itemOptionNames.Clear();
            itemOptionNames.Add("Cancel");
            foreach (CharacterClass characterClass in Enum.GetValues(typeof(CharacterClass)))
            {
                weaponOptionNames[characterClass].Tap(names => names.Clear()).Add("Cancel");
                shieldOptionNames[characterClass].Tap(names => names.Clear()).Add("Cancel");
            }

            foreach (CharacterClass characterClass in Enum.GetValues(typeof(CharacterClass)))
            {
                foreach (InventoryItem item in Battle.PlayerParty.WeaponInventories[characterClass].GetSortedItems())
                    weaponOptionNames[characterClass].Add(item.Key);
                foreach (InventoryItem item in Battle.PlayerParty.ShieldInventories[characterClass].GetSortedItems())
                    shieldOptionNames[characterClass].Add(item.Key);
            }
            foreach (InventoryItem item in Battle.PlayerParty.ItemInventory.GetSortedItems())
                itemOptionNames.Add(item.Key);
        }

        private void thinkAboutEnemyPartyActions()
        {
            //TODO: Move and add actions for enemy party
        }
        #endregion
    }
}
