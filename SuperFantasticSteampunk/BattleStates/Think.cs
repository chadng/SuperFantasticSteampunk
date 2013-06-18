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
        public bool InfiniteInInventory { get; set; }
        #endregion

        #region Constructors
        public ThinkAction(ThinkActionType type, string optionName, PartyMember actor = null, PartyMember target = null)
        {
            Type = type;
            OptionName = optionName;
            Actor = actor;
            Target = target;
            Active = true;
            InfiniteInInventory = false;
        }
        #endregion
    }

    class ThinkMenuOption
    {
        #region Static Fields
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
        #region Constants
        public static readonly string[] OuterMenuOptions = {
            "Move",
            "Attack",
            "Defend",
            "Item",
            "Run"
        };
        #endregion

        #region Instance Fields
        private ThinkAction currentThinkAction; // Assigned to just before SelectTarget state is pushed
        private Dictionary<CharacterClass, List<ThinkMenuOption>> weaponMenuOptions;
        private Dictionary<CharacterClass, List<ThinkMenuOption>> shieldMenuOptions;
        private List<ThinkMenuOption> itemMenuOptions;
        private List<ThinkAction> actions;
        private InputButtonListener inputButtonListener;
        private bool inOuterMenu;
        #endregion

        #region Instance Properties
        public ThinkActionType CurrentThinkActionType { get; private set; }
        public int CurrentOptionNameIndex { get; private set; }
        public PartyMember CurrentPartyMember { get; private set; }
        public List<ThinkMenuOption> MenuOptions { get; private set; }
        public int CurrentOuterMenuOptionIndex { get; private set; }

        public override bool KeepPartyMembersStatic
        {
            get { return true; }
        }

        public new ThinkRenderer BattleStateRenderer
        {
            get { return base.BattleStateRenderer as ThinkRenderer; }
            set { base.BattleStateRenderer = value; }
        }
        #endregion

        #region Constructors
        public Think(Battle battle)
            : base(battle)
        {
            CurrentThinkActionType = ThinkActionType.None;
            currentThinkAction = null;
            CurrentOptionNameIndex = 0;
            MenuOptions = null;
            inOuterMenu = true;
            CurrentOuterMenuOptionIndex = 0;

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
                { InputButton.Left, new ButtonEventHandlers(down: nextOption) },
                { InputButton.Right, new ButtonEventHandlers(down: previousOption) },
                { InputButton.A, new ButtonEventHandlers(up: selectOption) },
                { InputButton.B, new ButtonEventHandlers(up: cancelAction) }
            });
        }
        #endregion

        #region Instance Methods
        public bool PartyMemberHasCompletedThinkAction(PartyMember partyMember)
        {
            foreach (ThinkAction action in actions)
            {
                if (action.Actor == partyMember)
                    return true;
            }
            return false;
        }

        public override void Start()
        {
            getNextPartyMember();
            repopulateMenuOptions();
            BattleStateRenderer = new ThinkRenderer(this);
        }

        public override void Finish()
        {
            base.Finish();
            thinkAboutEnemyPartyActions();
            ChangeState(new Act(Battle, actions));
        }

        public override void Resume(BattleState previousBattleState)
        {
            base.Resume(previousBattleState);

            if (previousBattleState is SelectTarget)
                finishThinkForCurrentPartyMember();
        }

        public override void Update(Delta delta)
        {
            if (actions.Count == Battle.PlayerParty.Count)
            {
                Finish();
                return;
            }
            inputButtonListener.Update(delta);
        }

        private void previousOption()
        {
            changeOption(-1);
        }

        private void nextOption()
        {
            changeOption(1);
        }

        private void changeOption(int relativeIndex)
        {
            if (inOuterMenu && BattleStateRenderer.IsTransitioningMenu)
                return;

            chooseRelativeOption(relativeIndex);
            if (inOuterMenu)
                BattleStateRenderer.StartMenuTransition(relativeIndex);
        }

        private void selectOption()
        {
            if (inOuterMenu)
                selectOuterMenuOption();
            else
                selectAction();
        }

        private void selectOuterMenuOption()
        {
            switch (OuterMenuOptions[CurrentOuterMenuOptionIndex].ToLower())
            {
            case "move": PushState(new MoveActor(Battle, CurrentPartyMember, this)); break;
            case "attack": showAttackMenu(); break;
            case "defend": showDefendMenu(); break;
            case "item": showItemMenu(); break;
            case "run": ChangeState(new Run(Battle)); break;
            }
        }

        private void showAttackMenu()
        {
            initThinkActionTypeMenu(ThinkActionType.Attack);
            if (menuIsEmpty())
                hideOpenMenu();
            else
            {
                checkUsabilityOfWeaponMenuOptions();
                equipCurrentOption();
                inOuterMenu = false;
            }
        }

        private void showDefendMenu()
        {
            initThinkActionTypeMenu(ThinkActionType.Defend);
            if (menuIsEmpty())
                hideOpenMenu();
            else
            {
                equipCurrentOption();
                inOuterMenu = false;
            }
        }

        private void showItemMenu()
        {
            initThinkActionTypeMenu(ThinkActionType.UseItem);
            if (menuIsEmpty())
                hideOpenMenu();
            else
            {
                equipCurrentOption();
                inOuterMenu = false;
            }
        }

        private void hideOpenMenu()
        {
            initThinkActionTypeMenu(ThinkActionType.None);
            inOuterMenu = true;
        }

        private void cancelAction()
        {
            if (inOuterMenu)
            {
                if (actions.Count > 0)
                {
                    ThinkAction lastAction = actions[actions.Count - 1];
                    actions.RemoveAt(actions.Count - 1);
                    CurrentPartyMember = lastAction.Actor;
                    if (!ThinkMenuOption.IsDefaultOptionName(lastAction.OptionName))
                    {
                        Inventory inventory = getInventoryFromThinkActionType(lastAction.Type, CurrentPartyMember.CharacterClass);
                        if (inventory != null)
                            inventory.AddItem(lastAction.OptionName);
                    }
                    repopulateMenuOptions();
                    Logger.Log("Back to action selection for party member " + (actions.Count + 1).ToString());
                }
            }
            else
                hideOpenMenu();
        }

        private void chooseRelativeOption(int relativeIndex)
        {
            if (inOuterMenu)
            {
                CurrentOuterMenuOptionIndex += relativeIndex;
                if (CurrentOuterMenuOptionIndex >= OuterMenuOptions.Length)
                    CurrentOuterMenuOptionIndex = 0;
                else if (CurrentOuterMenuOptionIndex < 0)
                    CurrentOuterMenuOptionIndex = OuterMenuOptions.Length - 1;
            }
            else
            {
                CurrentOptionNameIndex += relativeIndex;
                if (CurrentOptionNameIndex >= MenuOptions.Count)
                    CurrentOptionNameIndex = MenuOptions.Count - 1;
                else if (CurrentOptionNameIndex < 0)
                    CurrentOptionNameIndex = 0;

                equipCurrentOption();

                Logger.Log("Selected option: " + MenuOptions[CurrentOptionNameIndex].Name);
            }
        }

        private void equipCurrentOption()
        {
            ThinkMenuOption option = MenuOptions[CurrentOptionNameIndex];
            if (CurrentThinkActionType == ThinkActionType.Attack)
            {
                CurrentPartyMember.EquipWeapon(option.Name);
                Logger.Log(CurrentPartyMember.Data.Name + " equipped '" + option.Name + "' weapon");
            }
            else if (CurrentThinkActionType == ThinkActionType.Defend)
            {
                CurrentPartyMember.EquipShield(ThinkMenuOption.IsDefaultOption(option) ? null : option.Name);
                Logger.Log(CurrentPartyMember.Data.Name + " equipped '" + option.Name + "' shield");
            }
            else
            {
                CurrentPartyMember.EquipWeapon(null);
                CurrentPartyMember.EquipShield(null);
            }
        }

        private void initThinkActionTypeMenu(ThinkActionType thinkActionType)
        {
            setThinkActionType(thinkActionType);
            Logger.Log(thinkActionType.ToString() + " action menu opened");
            selectDefaultOptionName();
            if (!menuIsEmpty())
                Logger.Log("Selected option: " + MenuOptions[CurrentOptionNameIndex].Name);
        }

        private void selectAction()
        {
            if (MenuOptions[CurrentOptionNameIndex].Disabled)
                return;

            ThinkMenuOption option = MenuOptions[CurrentOptionNameIndex];
            currentThinkAction = new ThinkAction(CurrentThinkActionType, option.Name, CurrentPartyMember);
            if (CurrentThinkActionType == ThinkActionType.Defend)
                finishThinkForCurrentPartyMember();
            else
            {
                currentThinkAction.InfiniteInInventory = option.Amount < 0;
                PushState(new SelectTarget(Battle, currentThinkAction));
            }
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
            if (menuIsEmpty())
                CurrentOptionNameIndex = 0;
            else if (CurrentThinkActionType == ThinkActionType.Defend && MenuOptions.Count > 1)
                CurrentOptionNameIndex = 1;
            else
                CurrentOptionNameIndex = 0;                
        }

        private bool menuIsEmpty()
        {
            return MenuOptions == null || MenuOptions.Count < 1;
        }

        private void getNextPartyMember()
        {
            if (actions.Count < Battle.PlayerParty.Count)
            {
                inOuterMenu = true;
                CurrentOuterMenuOptionIndex = 0;
                IEnumerable<PartyMember> finishedPartyMembers = actions.Select<ThinkAction, PartyMember>(thinkAction => thinkAction.Actor);
                CurrentPartyMember = Battle.PlayerParty.Find(partyMember => !finishedPartyMembers.Contains(partyMember));
                Logger.Log("Action selection for party member " + (actions.Count + 1).ToString());
            }
        }

        private void repopulateMenuOptions()
        {
            itemMenuOptions.Clear();
            foreach (CharacterClass characterClass in Enum.GetValues(typeof(CharacterClass)))
            {
                weaponMenuOptions[characterClass].Clear();
                shieldMenuOptions[characterClass].Clear();
                shieldMenuOptions[characterClass].Add(ThinkMenuOption.NoShield);
            }

            foreach (CharacterClass characterClass in Enum.GetValues(typeof(CharacterClass)))
            {
                foreach (InventoryItem item in Battle.PlayerParty.WeaponInventories[characterClass].GetSortedItems(CurrentPartyMember))
                    weaponMenuOptions[characterClass].Add(new ThinkMenuOption(item.Key, item.Value, false));
                foreach (InventoryItem item in Battle.PlayerParty.ShieldInventory.GetSortedItems(CurrentPartyMember))
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
                    if (ThinkMenuOption.IsDefaultOption(menuOption))
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
                Inventory inventory = getInventoryFromThinkActionType(currentThinkAction.Type, CurrentPartyMember.CharacterClass);
                if (inventory != null)
                    inventory.UseItem(currentThinkAction.OptionName, CurrentPartyMember);
                actions.Add(currentThinkAction);
                getNextPartyMember();
                repopulateMenuOptions();
                currentThinkAction = null;
                initThinkActionTypeMenu(ThinkActionType.None);
            }
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
