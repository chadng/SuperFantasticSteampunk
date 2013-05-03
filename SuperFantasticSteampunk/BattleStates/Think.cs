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
        private ThinkAction currentThinkAction; // Assigned to just before SelectTarget state is pushed
        private Dictionary<CharacterClass, List<string>> weaponOptionNames;
        private Dictionary<CharacterClass, List<string>> shieldOptionNames;
        private List<string> itemOptionNames;
        private List<ThinkAction> actions;
        private InputButtonListener inputButtonListener;
        #endregion

        #region Instance Properties
        public ThinkActionType CurrentThinkActionType { get; private set; }
        public int CurrentOptionNameIndex { get; private set; }
        public PartyMember CurrentPartyMember { get; private set; }
        public List<string> OptionNames { get; private set; }
        #endregion

        #region Constructors
        public Think(Battle battle)
            : base(battle)
        {
            CurrentThinkActionType = ThinkActionType.None;
            currentThinkAction = null;
            CurrentOptionNameIndex = 0;
            OptionNames = null;

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
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            getNextPartyMember();
            repopulateOptionNames();
            BattleStateRenderer = new ThinkRenderer(this);
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
                finishThinkForCurrentPartyMember();
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
            if (CurrentThinkActionType == ThinkActionType.None || OptionNames == null)
                return;

            if (--CurrentOptionNameIndex < 0)
                CurrentOptionNameIndex = 0;

            Logger.Log("Selected option: " + OptionNames[CurrentOptionNameIndex]);
        }

        private void chooseNextOption()
        {
            if (CurrentThinkActionType == ThinkActionType.None || OptionNames == null)
                return;

            if (++CurrentOptionNameIndex >= OptionNames.Count)
                CurrentOptionNameIndex = OptionNames.Count - 1;

            Logger.Log("Selected option: " + OptionNames[CurrentOptionNameIndex]);
        }

        private void showAttackMenu()
        {
            if (CurrentThinkActionType == ThinkActionType.None)
                initThinkActionTypeMenu(ThinkActionType.Attack);
        }

        private void hideAttackMenu()
        {
            if (CurrentThinkActionType == ThinkActionType.Attack)
                selectAction();
        }

        private void showDefendMenu()
        {
            if (CurrentThinkActionType == ThinkActionType.None)
                initThinkActionTypeMenu(ThinkActionType.Defend);
        }

        private void hideDefendMenu()
        {
            if (CurrentThinkActionType == ThinkActionType.Defend)
                selectAction();
        }

        private void showItemMenu()
        {
            if (CurrentThinkActionType == ThinkActionType.None)
                initThinkActionTypeMenu(ThinkActionType.UseItem);
        }

        private void hideItemMenu()
        {
            if (CurrentThinkActionType == ThinkActionType.UseItem)
                selectAction();
        }

        private void cancelAction()
        {
            if (CurrentThinkActionType == ThinkActionType.None && actions.Count > 0)
            {
                ThinkAction lastAction = actions[actions.Count - 1];
                actions.RemoveAt(actions.Count - 1);
                CurrentPartyMember = lastAction.Actor;
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
            if (OptionNames != null)
                Logger.Log("Selected option: " + OptionNames[CurrentOptionNameIndex]);
        }

        private void selectAction()
        {
            if (CurrentOptionNameIndex == 0) // i.e. Cancel
            {
                initThinkActionTypeMenu(ThinkActionType.None);
                return;
            }

            currentThinkAction = new ThinkAction(CurrentThinkActionType, OptionNames[CurrentOptionNameIndex], CurrentPartyMember);
            if (CurrentThinkActionType == ThinkActionType.Defend)
                finishThinkForCurrentPartyMember();
            else
                PushState(new SelectTarget(Battle, currentThinkAction));
        }

        private void setThinkActionType(ThinkActionType thinkActionType)
        {
            CurrentThinkActionType = thinkActionType;
            switch (thinkActionType)
            {
            case ThinkActionType.Attack: OptionNames = weaponOptionNames[CurrentPartyMember.CharacterClass]; break;
            case ThinkActionType.Defend: OptionNames = shieldOptionNames[CurrentPartyMember.CharacterClass]; break;
            case ThinkActionType.UseItem: OptionNames = itemOptionNames; break;
            default: OptionNames = null; break;
            }
        }

        private void selectDefaultOptionName()
        {
            if (OptionNames == null || OptionNames.Count == 1)
                CurrentOptionNameIndex = 0;
            else
                CurrentOptionNameIndex = 1;                
        }

        private void getNextPartyMember()
        {
            if (actions.Count < Battle.PlayerParty.Count)
            {
                IEnumerable<PartyMember> finishedPartyMembers = actions.Select<ThinkAction, PartyMember>(thinkAction => thinkAction.Actor);
                CurrentPartyMember = Battle.PlayerParty.Find(partyMember => !finishedPartyMembers.Contains(partyMember));
                Logger.Log("Action selection for party member " + (actions.Count + 1).ToString());
            }
        }

        private Inventory getInventoryFromThinkActionType(ThinkActionType thinkActionType)
        {
            switch (thinkActionType)
            {
            case ThinkActionType.Attack: return Battle.PlayerParty.WeaponInventories[CurrentPartyMember.CharacterClass];
            case ThinkActionType.Defend: return Battle.PlayerParty.ShieldInventories[CurrentPartyMember.CharacterClass];
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
                foreach (InventoryItem item in Battle.PlayerParty.WeaponInventories[characterClass].GetSortedItems(CurrentPartyMember))
                    weaponOptionNames[characterClass].Add(item.Key);
                foreach (InventoryItem item in Battle.PlayerParty.ShieldInventories[characterClass].GetSortedItems(CurrentPartyMember))
                    shieldOptionNames[characterClass].Add(item.Key);
            }
            foreach (InventoryItem item in Battle.PlayerParty.ItemInventory.GetSortedItems(CurrentPartyMember))
                itemOptionNames.Add(item.Key);
        }

        private void finishThinkForCurrentPartyMember()
        {
            if (currentThinkAction.Target != null || currentThinkAction.Type == ThinkActionType.Defend)
            {
                Inventory inventory = getInventoryFromThinkActionType(currentThinkAction.Type);
                if (inventory != null)
                    inventory.UseItem(currentThinkAction.OptionName, CurrentPartyMember);
                actions.Add(currentThinkAction);
                getNextPartyMember();
                repopulateOptionNames();
            }

            currentThinkAction = null;
            initThinkActionTypeMenu(ThinkActionType.None);
        }

        private void thinkAboutEnemyPartyActions()
        {
            //TODO: Move and add actions for enemy party
        }
        #endregion
    }
}
