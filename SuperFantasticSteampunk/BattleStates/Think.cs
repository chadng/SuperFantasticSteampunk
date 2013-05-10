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
        #region Instance Properties
        public ThinkActionType Type { get; private set; }
        public string OptionName { get; private set; }
        public PartyMember Actor { get; set; }
        public PartyMember Target { get; set; }
        public bool Active { get; set; }
        #endregion

        #region Constructors
        public ThinkAction(ThinkActionType type, string optionName, PartyMember actor = null, PartyMember target = null)
        {
            Type = type;
            OptionName = optionName;
            Actor = actor;
            Target = target;
            Active = true;
        }
        #endregion
    }

    class ThinkMenuOption
    {
        #region Static Fields
        public static readonly ThinkMenuOption Cancel;
        public static readonly ThinkMenuOption NoWeapon;
        public static readonly ThinkMenuOption NoShield;
        #endregion

        #region Instance Properties
        public string Name { get; private set; }
        public int Amount { get; private set; }
        public bool Disabled { get; set; }
        #endregion

        #region Static Constructors
        static ThinkMenuOption()
        {
            Cancel = new ThinkMenuOption("Cancel", -1, false);
            NoWeapon = new ThinkMenuOption("No weapon", -1, false);
            NoShield = new ThinkMenuOption("No shield", -1, false);
        }
        #endregion

        #region Static Methods
        public static bool IsDefaultOption(ThinkMenuOption option)
        {
            return IsDefaultOptionName(option.Name);
        }


        public static bool IsDefaultOptionName(string name)
        {
            return NoWeapon.Name == name || NoShield.Name == name;
        }
        #endregion

        #region Constructors
        public ThinkMenuOption(string name, int amount, bool disabled)
        {
            Name = name;
            Amount = amount;
            Disabled = disabled;
        }
        #endregion
    }

    class Think : BattleState
    {

        #region Instance Fields
        private ThinkAction currentThinkAction; // Assigned to just before SelectTarget state is pushed
        private Dictionary<CharacterClass, List<ThinkMenuOption>> weaponMenuOptions;
        private Dictionary<CharacterClass, List<ThinkMenuOption>> shieldMenuOptions;
        private List<ThinkMenuOption> itemMenuOptions;
        private List<ThinkAction> actions;
        private InputButtonListener inputButtonListener;
        #endregion

        #region Instance Properties
        public ThinkActionType CurrentThinkActionType { get; private set; }
        public int CurrentOptionNameIndex { get; private set; }
        public PartyMember CurrentPartyMember { get; private set; }
        public List<ThinkMenuOption> MenuOptions { get; private set; }
        #endregion

        #region Constructors
        public Think(Battle battle)
            : base(battle)
        {
            CurrentThinkActionType = ThinkActionType.None;
            currentThinkAction = null;
            CurrentOptionNameIndex = 0;
            MenuOptions = null;

            weaponMenuOptions = new Dictionary<CharacterClass, List<ThinkMenuOption>>();
            shieldMenuOptions = new Dictionary<CharacterClass, List<ThinkMenuOption>>();
            itemMenuOptions = new List<ThinkMenuOption>();
            foreach (CharacterClass characterClass in Enum.GetValues(typeof(CharacterClass)))
            {
                weaponMenuOptions.Add(characterClass, new List<ThinkMenuOption>());
                shieldMenuOptions.Add(characterClass, new List<ThinkMenuOption>());
            }

            actions = new List<ThinkAction>(battle.PlayerParty.Count);

            inputButtonListener = new InputButtonListener(new Dictionary<InputButton, ButtonEventHandlers> {
                { InputButton.Up, new ButtonEventHandlers(down: moveUp) },
                { InputButton.Down, new ButtonEventHandlers(down: moveDown) },
                { InputButton.Left, new ButtonEventHandlers(down: moveLeft) },
                { InputButton.Right, new ButtonEventHandlers(down: moveRight) },
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
            repopulateMenuOptions();
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

        private void moveUp()
        {
            if (CurrentThinkActionType == ThinkActionType.None)
            {
                Battle.PlayerPartyLayout.MovePartyMemberUp(CurrentPartyMember);
                checkUsabilityOfWeaponMenuOptions();
            }
            else
                chooseRelativeOption(-1);
        }

        private void moveDown()
        {
            if (CurrentThinkActionType == ThinkActionType.None)
            {
                Battle.PlayerPartyLayout.MovePartyMemberDown(CurrentPartyMember);
                checkUsabilityOfWeaponMenuOptions();
            }
            else
                chooseRelativeOption(1);
        }

        private void moveLeft()
        {
            if (CurrentThinkActionType == ThinkActionType.None)
            {
                Battle.PlayerPartyLayout.MovePartyMemberBack(CurrentPartyMember);
                checkUsabilityOfWeaponMenuOptions();
            }
        }

        private void moveRight()
        {
            if (CurrentThinkActionType == ThinkActionType.None)
            {
                Battle.PlayerPartyLayout.MovePartyMemberForward(CurrentPartyMember);
                checkUsabilityOfWeaponMenuOptions();
            }
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
                if (!ThinkMenuOption.IsDefaultOptionName(lastAction.OptionName))
                {
                    Inventory inventory = getInventoryFromThinkActionType(lastAction.Type);
                    if (inventory != null)
                        inventory.AddItem(lastAction.OptionName);
                }
                repopulateMenuOptions();
                Logger.Log("Back to action selection for party member " + (actions.Count + 1).ToString());
            }
        }

        private void chooseRelativeOption(int relativeIndex)
        {
            if (CurrentThinkActionType == ThinkActionType.None || MenuOptions == null)
                return;

            CurrentOptionNameIndex += relativeIndex;
            if (CurrentOptionNameIndex >= MenuOptions.Count)
                CurrentOptionNameIndex = MenuOptions.Count - 1;
            else if (CurrentOptionNameIndex < 0)
                CurrentOptionNameIndex = 0;

            equipCurrentOption();

            Logger.Log("Selected option: " + MenuOptions[CurrentOptionNameIndex].Name);
        }

        private void equipCurrentOption()
        {
            ThinkMenuOption option = MenuOptions[CurrentOptionNameIndex];
            if (CurrentThinkActionType == ThinkActionType.Attack)
            {
                CurrentPartyMember.EquipWeapon(ThinkMenuOption.IsDefaultOption(option) ? null : option.Name);
                Logger.Log(CurrentPartyMember.Data.Name + " equipped '" + option.Name + "' weapon");
            }
            else if (CurrentThinkActionType == ThinkActionType.Defend)
            {
                CurrentPartyMember.EquipShield(ThinkMenuOption.IsDefaultOption(option) ? null : option.Name);
                Logger.Log(CurrentPartyMember.Data.Name + " equipped '" + option.Name + "' shield");
            }
        }

        private void initThinkActionTypeMenu(ThinkActionType thinkActionType)
        {
            setThinkActionType(thinkActionType);
            Logger.Log(thinkActionType.ToString() + " action menu opened");
            selectDefaultOptionName();
            if (MenuOptions != null)
                Logger.Log("Selected option: " + MenuOptions[CurrentOptionNameIndex].Name);
        }

        private void selectAction()
        {
            if (CurrentOptionNameIndex == 0 || MenuOptions[CurrentOptionNameIndex].Disabled) // i.e. Cancel
            {
                initThinkActionTypeMenu(ThinkActionType.None);
                return;
            }

            currentThinkAction = new ThinkAction(CurrentThinkActionType, MenuOptions[CurrentOptionNameIndex].Name, CurrentPartyMember);
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
            case ThinkActionType.Attack: MenuOptions = weaponMenuOptions[CurrentPartyMember.CharacterClass]; break;
            case ThinkActionType.Defend: MenuOptions = shieldMenuOptions[CurrentPartyMember.CharacterClass]; break;
            case ThinkActionType.UseItem: MenuOptions = itemMenuOptions; break;
            default: MenuOptions = null; break;
            }
        }

        private void selectDefaultOptionName()
        {
            if (MenuOptions == null || MenuOptions.Count <= 2)
                CurrentOptionNameIndex = 0;
            else
                CurrentOptionNameIndex = 2;                
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

        private void repopulateMenuOptions()
        {
            itemMenuOptions.Clear();
            itemMenuOptions.Add(ThinkMenuOption.Cancel);
            foreach (CharacterClass characterClass in Enum.GetValues(typeof(CharacterClass)))
            {
                weaponMenuOptions[characterClass].Tap(menuOptions =>
                {
                    menuOptions.Clear();
                    menuOptions.Add(ThinkMenuOption.Cancel);
                    menuOptions.Add(ThinkMenuOption.NoWeapon);
                });
                shieldMenuOptions[characterClass].Tap(menuOptions =>
                {
                    menuOptions.Clear();
                    menuOptions.Add(ThinkMenuOption.Cancel);
                    menuOptions.Add(ThinkMenuOption.NoShield);
                });
            }

            foreach (CharacterClass characterClass in Enum.GetValues(typeof(CharacterClass)))
            {
                foreach (InventoryItem item in Battle.PlayerParty.WeaponInventories[characterClass].GetSortedItems(CurrentPartyMember))
                    weaponMenuOptions[characterClass].Add(new ThinkMenuOption(item.Key, item.Value, false));
                foreach (InventoryItem item in Battle.PlayerParty.ShieldInventories[characterClass].GetSortedItems(CurrentPartyMember))
                    shieldMenuOptions[characterClass].Add(new ThinkMenuOption(item.Key, item.Value, false));
            }
            foreach (InventoryItem item in Battle.PlayerParty.ItemInventory.GetSortedItems(CurrentPartyMember))
                itemMenuOptions.Add(new ThinkMenuOption(item.Key, item.Value, false));
            checkUsabilityOfWeaponMenuOptions();
        }

        private void checkUsabilityOfWeaponMenuOptions()
        {
            foreach (CharacterClass characterClass in Enum.GetValues(typeof(CharacterClass)))
            {
                foreach (ThinkMenuOption menuOption in weaponMenuOptions[characterClass])
                {
                    if (menuOption.Name == ThinkMenuOption.Cancel.Name || ThinkMenuOption.IsDefaultOption(menuOption))
                    {
                        menuOption.Disabled = false;
                        continue;
                    }
                    WeaponData weaponData = ResourceManager.GetWeaponData(menuOption.Name);
                    menuOption.Disabled = weaponData == null || (weaponData.WeaponType == WeaponType.Melee && !Battle.PlayerPartyLayout.PartyMemberInFrontLine(CurrentPartyMember));
                }
            }
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
                repopulateMenuOptions();
            }

            currentThinkAction = null;
            initThinkActionTypeMenu(ThinkActionType.None);
        }

        private void thinkAboutEnemyPartyActions()
        {
            foreach (PartyMember partyMember in Battle.EnemyParty)
            {
                //TODO: improve this
                ThinkAction thinkAction = new ThinkAction(ThinkActionType.Attack, "default", partyMember, Battle.PlayerParty.Sample());
                if (thinkAction.Actor.EquippedWeapon == null)
                    thinkAction.Actor.EquipWeapon(thinkAction.OptionName);
                actions.Add(thinkAction);
            }
        }
        #endregion
    }
}
