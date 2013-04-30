using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.BattleStates
{
    using InventoryItem = KeyValuePair<string, int>;

    enum ThinkActionType { None, Attack, Defend, UseItem }

    class ThinkAction
    {
        #region Instance Fields
        public readonly ThinkActionType Type;
        public readonly string OptionName;
        public PartyMember Target;
        #endregion

        #region Constructors
        public ThinkAction(ThinkActionType type, string optionName, PartyMember target = null)
        {
            Type = type;
            OptionName = optionName;
            Target = target;
        }
        #endregion
    }

    class Think : BattleState
    {
        #region Instance Fields
        private ThinkActionType currentThinkActionType;
        private int currentOptionNameIndex;
        private PartyMember currentPartyMember;
        private List<string> optionNames;
        private List<string> weaponOptionNames;
        private List<string> itemOptionNames;
        private Dictionary<PartyMember, ThinkAction> actions;
        private InputButtonListener inputButtonListener;
        #endregion

        #region Constructors
        public Think(Battle battle)
            : base(battle)
        {
            currentThinkActionType = ThinkActionType.None;
            currentOptionNameIndex = 0;
            currentPartyMember = null;
            optionNames = null;
            weaponOptionNames = new List<string>().Tap(names => names.Add("Cancel"));
            itemOptionNames = new List<string>().Tap(names => names.Add("Cancel"));
            actions = new Dictionary<PartyMember, ThinkAction>(battle.PlayerParty.Count);

            inputButtonListener = new InputButtonListener(new Dictionary<InputButton, ButtonEventHandlers> {
                { InputButton.Up, new ButtonEventHandlers(down: buttonUpDown) },
                { InputButton.Down, new ButtonEventHandlers(down: buttonDownDown) },
                { InputButton.A, new ButtonEventHandlers(down: buttonADown, up: buttonAUp) },
                { InputButton.B, new ButtonEventHandlers(down: buttonBDown, up: buttonBUp) },
                { InputButton.X, new ButtonEventHandlers(down: buttonXDown, up: buttonXUp) }
            });
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            foreach (InventoryItem item in battle.PlayerParty.WeaponInventory.GetSortedItems())
                weaponOptionNames.Add(item.Value.ToString() + " x " + item.Key);
            foreach (InventoryItem item in battle.PlayerParty.ItemInventory.GetSortedItems())
                itemOptionNames.Add(item.Value.ToString() + " x " + item.Key);

            currentPartyMember = battle.PlayerParty[0];
        }

        public override void Finish()
        {
            ChangeState(new Act(battle));
        }

        public override void Update(GameTime gameTime)
        {
            inputButtonListener.Update(gameTime);
        }

        private void buttonUpDown()
        {
            if (currentThinkActionType == ThinkActionType.None || optionNames == null)
                return;

            if (--currentOptionNameIndex < 0)
                currentOptionNameIndex = 0;
        }

        private void buttonDownDown()
        {
            if (currentThinkActionType == ThinkActionType.None || optionNames == null)
                return;

            if (++currentOptionNameIndex >= optionNames.Count)
                currentOptionNameIndex = optionNames.Count - 1;
        }

        private void buttonADown()
        {
            if (currentThinkActionType == ThinkActionType.None)
                initThinkActionTypeMenu(ThinkActionType.Attack);
        }

        private void buttonAUp()
        {
            if (currentThinkActionType == ThinkActionType.Attack)
                selectAction();
        }

        private void buttonBDown()
        {
            if (currentThinkActionType == ThinkActionType.None)
                initThinkActionTypeMenu(ThinkActionType.Defend);
        }

        private void buttonBUp()
        {
            if (currentThinkActionType == ThinkActionType.Defend)
                selectAction();
        }

        private void buttonXDown()
        {
            if (currentThinkActionType == ThinkActionType.None)
                initThinkActionTypeMenu(ThinkActionType.UseItem);
        }

        private void buttonXUp()
        {
            if (currentThinkActionType == ThinkActionType.UseItem)
                selectAction();
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

            actions.Add(currentPartyMember, new ThinkAction(currentThinkActionType, optionNames[currentOptionNameIndex]));
            //TODO: choose target
        }

        private void setThinkActionType(ThinkActionType thinkActionType)
        {
            currentThinkActionType = thinkActionType;
            if (thinkActionType == ThinkActionType.None)
                optionNames = null;
            else if (thinkActionType == ThinkActionType.UseItem)
                optionNames = itemOptionNames;
            else
                optionNames = weaponOptionNames;
        }

        private void selectDefaultOptionName()
        {
            if (optionNames == null || optionNames.Count == 1)
                currentOptionNameIndex = 0;
            else
                currentOptionNameIndex = 1;                
        }
        #endregion
    }
}
