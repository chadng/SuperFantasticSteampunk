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
        public string Description { get; private set; }
        public int Amount { get; private set; }
        public bool Disabled { get; set; }
        #endregion

        #region Static Constructors
        static ThinkMenuOption()
        {
            NoWeapon = new ThinkMenuOption("No weapon", null, -1, false);
            NoShield = new ThinkMenuOption("No shield", null, -1, false);
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
        public ThinkMenuOption(string name, string description, int amount, bool disabled)
        {
            Name = name;
            Description = description ?? "No description";
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
            "Defend",
            "Item",
            "Run",
            "Attack"
        };
        #endregion

        #region Instance Fields
        private ThinkAction currentThinkAction; // Assigned to just before SelectTarget state is pushed
        private Dictionary<CharacterClass, List<ThinkMenuOption>> weaponMenuOptions;
        private List<ThinkMenuOption> shieldMenuOptions;
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
        public bool Paused { get; private set; }

        public PartyMember PreviouslyActedPartyMember
        {
            get { return actions.Count > 0 ? actions[actions.Count - 1].Actor : null; }
        }

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
            foreach (CharacterClass characterClass in Enum.GetValues(typeof(CharacterClass)))
                weaponMenuOptions.Add(characterClass, new List<ThinkMenuOption>());
            shieldMenuOptions = new List<ThinkMenuOption>();
            itemMenuOptions = new List<ThinkMenuOption>();

            actions = new List<ThinkAction>(battle.PlayerParty.Count);

            inputButtonListener = new InputButtonListener(new Dictionary<InputButton, ButtonEventHandlers> {
                { InputButton.Up, new ButtonEventHandlers(down: upHandler) },
                { InputButton.Down, new ButtonEventHandlers(down: downHandler) },
                { InputButton.Left, new ButtonEventHandlers(down: leftHandler) },
                { InputButton.Right, new ButtonEventHandlers(down: rightHandler) },
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
            Paused = false;
        }

        public override void Finish()
        {
            base.Finish();
            thinkAboutEnemyPartyActions();
            ChangeState(new Act(Battle, actions));
        }

        public override void Pause()
        {
            Paused = true;
            base.Pause();
        }

        public override void Resume(BattleState previousBattleState)
        {
            base.Resume(previousBattleState);
            Paused = false;

            if (previousBattleState is SelectTarget)
                finishThinkForCurrentPartyMember();
            else if (previousBattleState is MoveActor)
                BattleStateRenderer.StartOuterMenuOptionDeselectTransition();
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

        private void upHandler()
        {
            if (!inOuterMenu)
                previousOption();
        }

        private void downHandler()
        {
            if (!inOuterMenu)
                nextOption();
        }

        private void leftHandler()
        {
            if (inOuterMenu)
                nextOption();
        }

        private void rightHandler()
        {
            if (inOuterMenu)
                previousOption();
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
                BattleStateRenderer.StartOuterMenuOptionTransition(relativeIndex);
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
            BattleStateRenderer.StartOuterMenuOptionSelectTransition();
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
            BattleStateRenderer.StartOuterMenuOptionDeselectTransition();
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
            case ThinkActionType.Defend: MenuOptions = shieldMenuOptions; break;
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
                setOuterMenuOptionIndexForCurrentPartyMember();
                Logger.Log("Action selection for party member " + (actions.Count + 1).ToString());
            }
        }

        private void repopulateMenuOptions()
        {
            itemMenuOptions.Clear();
            shieldMenuOptions.Clear();
            shieldMenuOptions.Add(ThinkMenuOption.NoShield);
            foreach (CharacterClass characterClass in Enum.GetValues(typeof(CharacterClass)))
            {
                if (characterClass == CharacterClass.Enemy)
                    continue;

                weaponMenuOptions[characterClass].Clear();

                foreach (InventoryItem item in Battle.PlayerParty.WeaponInventories[characterClass].GetSortedItems(CurrentPartyMember))
                {
                    WeaponData weaponData = ResourceManager.GetWeaponData(item.Key);
                    weaponMenuOptions[characterClass].Add(new ThinkMenuOption(item.Key, weaponData.Description, item.Value, false));
                }
            }
            foreach (InventoryItem item in Battle.PlayerParty.ItemInventory.GetSortedItems(CurrentPartyMember))
            {
                ItemData itemData = ResourceManager.GetItemData(item.Key);
                itemMenuOptions.Add(new ThinkMenuOption(item.Key, itemData.Description, item.Value, false));
            }
            foreach (InventoryItem item in Battle.PlayerParty.ShieldInventory.GetSortedItems(CurrentPartyMember))
            {
                ShieldData shieldData = ResourceManager.GetShieldData(item.Key);
                shieldMenuOptions.Add(new ThinkMenuOption(item.Key, shieldData.Description, item.Value, false));
            }
            checkUsabilityOfWeaponMenuOptions();
        }

        private void checkUsabilityOfWeaponMenuOptions()
        {
            foreach (CharacterClass characterClass in Enum.GetValues(typeof(CharacterClass)))
            {
                if (characterClass == CharacterClass.Enemy)
                    continue;

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
                Battle.LastUsedThinkActionTypes.AddOrReplace(CurrentPartyMember, new Wrapper<ThinkActionType>(currentThinkAction.Type));
                getNextPartyMember();
                repopulateMenuOptions();
                currentThinkAction = null;
                initThinkActionTypeMenu(ThinkActionType.None);
                if (actions.Count == Battle.PlayerParty.Count)
                    BattleStateRenderer = null;
                else
                    BattleStateRenderer.ResetOuterMenuTransitions();
            }
        }


        private void setOuterMenuOptionIndexForCurrentPartyMember()
        {
            if (CurrentPartyMember == null)
                return;
            Wrapper<ThinkActionType> thinkActionTypeWrapper;
            if (Battle.LastUsedThinkActionTypes.TryGetValue(CurrentPartyMember, out thinkActionTypeWrapper))
            {
                switch (thinkActionTypeWrapper.Value)
                {
                case ThinkActionType.Attack: CurrentOuterMenuOptionIndex = OuterMenuOptions.IndexOf("Attack"); break;
                case ThinkActionType.Defend: CurrentOuterMenuOptionIndex = OuterMenuOptions.IndexOf("Defend"); break;
                case ThinkActionType.UseItem: CurrentOuterMenuOptionIndex = OuterMenuOptions.IndexOf("Item"); break;
                default: CurrentOuterMenuOptionIndex = 0; break;
                }
                if (CurrentOuterMenuOptionIndex < 0)
                    CurrentOuterMenuOptionIndex = 0;
            }
            else
                CurrentOuterMenuOptionIndex = 0;
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
