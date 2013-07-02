using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SuperFantasticSteampunk.OverworldStates;

namespace SuperFantasticSteampunk.BattleStates
{
    class Win : BattleState
    {
        #region Constants
        private const int minimumItemsWon = 7;
        private const int minimumForItemsWonRandomizer = 15;
        private const int itemsUsedDivisorForItemsWonRandomizer = 5;
        #endregion

        #region Instance Fields
        private InputButtonListener inputButtonListener;
        private int minCurrentItemIndex;
        private int maxCurrentItemIndex;
        #endregion

        #region Instance Properties
        public string WinMessage { get; private set; }
        public List<Tuple<string, CharacterClass>> ItemsWon { get; private set; }
        public int CurrentItemIndex { get; private set; }

        public new WinRenderer BattleStateRenderer
        {
            get { return base.BattleStateRenderer as WinRenderer; }
            protected set { base.BattleStateRenderer = value; }
        }

        public override bool KeepPartyMembersStatic
        {
            get { return true; }
        }
        #endregion

        #region Constructors
        public Win(Battle battle)
            : base(battle)
        {
            WinMessage = ResourceManager.BattleWinMessages.Sample();
            ItemsWon = new List<Tuple<string, CharacterClass>>();

            inputButtonListener = new InputButtonListener(new Dictionary<InputButton, ButtonEventHandlers> {
                { InputButton.Up, new ButtonEventHandlers(down: previousItem) },
                { InputButton.Down, new ButtonEventHandlers(down: nextItem) }
            });
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            ItemsWon.Clear();
            addNewItemsToPlayerPartyInventory();
            BattleStateRenderer = new WinRenderer(this);
            getMinAndMaxCurrentItemIndex();
            CurrentItemIndex = minCurrentItemIndex;
        }

        public override void Finish()
        {
            base.Finish();
            ChangeState(new Outro(EncounterState.Won, Battle));
        }

        public override void Update(Delta delta)
        {
            if (BattleStateRenderer.FadeInFinished)
            {
            inputButtonListener.Update(delta);
                if (Input.AnyActionButton())
                    Finish();
            }
        }

        private void previousItem()
        {
            if (--CurrentItemIndex < minCurrentItemIndex)
                CurrentItemIndex = minCurrentItemIndex;
        }

        private void nextItem()
        {
            if (++CurrentItemIndex > maxCurrentItemIndex)
                CurrentItemIndex = maxCurrentItemIndex;
        }

        private void addNewItemsToPlayerPartyInventory()
        {
            Dictionary<Rarity, List<WeaponData>> weapons = new Dictionary<Rarity, List<WeaponData>>();
            Dictionary<Rarity, List<ShieldData>> shields = new Dictionary<Rarity, List<ShieldData>>();
            Dictionary<Rarity, List<ItemData>> items = new Dictionary<Rarity, List<ItemData>>();

            foreach (Rarity rarity in Enum.GetValues(typeof(Rarity)))
            {
                if (rarity == Rarity.Never)
                    continue;

                weapons[rarity] = ResourceManager.GetWeaponDataWhere(wd => wd.Rarity == rarity);
                shields[rarity] = ResourceManager.GetShieldDataWhere(sd => sd.Rarity == rarity);
                items[rarity] = ResourceManager.GetItemDataWhere(id => id.Rarity == rarity);
            }

            List<Rarity> rarityList = new List<Rarity> {
                Rarity.Common,
                Rarity.Uncommon,
                Rarity.Rare,
                Rarity.Common,
                Rarity.Uncommon,
                Rarity.Common
            };

            List<ThinkActionType> thinkActionTypeList = new List<ThinkActionType> {
                ThinkActionType.Attack,
                ThinkActionType.Defend,
                ThinkActionType.Attack,
                ThinkActionType.UseItem
            };

            int count = Math.Max(minimumItemsWon, Battle.PlayerPartyItemsUsed + Game1.Random.Next(Math.Max(minimumForItemsWonRandomizer, Battle.PlayerPartyItemsUsed / itemsUsedDivisorForItemsWonRandomizer)));
            for (int i = 0; i < count; ++i)
            {
                Rarity rarity = rarityList.Sample();
                switch (thinkActionTypeList.Sample())
                {
                case ThinkActionType.Attack:
                    if (weapons[rarity].Count == 0)
                    {
                        --i;
                        continue;
                    }
                    WeaponData weaponData = weapons[rarity].Sample();
                    if (!Battle.PlayerParty.HasMemberOfCharacterClass(weaponData.CharacterClass))
                    {
                        --i;
                        continue;
                    }
                    string weaponName = new Attributes(weaponData).ToString(weaponData.Name);
                    Battle.PlayerParty.WeaponInventories[weaponData.CharacterClass].AddItem(weaponName, null);
                    ItemsWon.Add(new Tuple<string, CharacterClass>(weaponName, weaponData.CharacterClass));
                    Logger.Log("Player party won weapon " + weaponName);
                    break;

                case ThinkActionType.Defend:
                    if (shields[rarity].Count == 0)
                    {
                        --i;
                        continue;
                    }
                    ShieldData shieldData = shields[rarity].Sample();
                    string shieldName = new Attributes(shieldData).ToString(shieldData.Name);
                    Battle.PlayerParty.ShieldInventory.AddItem(shieldName, null);
                    ItemsWon.Add(new Tuple<string, CharacterClass>(shieldName, CharacterClass.Enemy));
                    Logger.Log("Player party won shield " + shieldName);
                    break;

                case ThinkActionType.UseItem:
                    if (items[rarity].Count == 0)
                    {
                        --i;
                        continue;
                    }
                    ItemData itemData = items[rarity].Sample();
                    string itemName = itemData.Name.ToUpperFirstChar();
                    Battle.PlayerParty.ItemInventory.AddItem(itemName, null);
                    ItemsWon.Add(new Tuple<string, CharacterClass>(itemName, CharacterClass.Enemy));
                    Logger.Log("Player party won item " + itemName);
                    break;
                }
            }
        }

        private void getMinAndMaxCurrentItemIndex()
        {
            int visibleItemCount = BattleStateRenderer.VisibleItemCount;
            if (ItemsWon.Count <= visibleItemCount)
                minCurrentItemIndex = maxCurrentItemIndex = 0;
            else
            {
                // dumbest thing but requires no brainpower
                int startIndex, startIndexBefore = 0;
                int finishIndex, finishIndexBefore = ItemsWon.Count - 1;
                for (int i = 0; i < ItemsWon.Count; ++i)
                {
                    Battle.CalculateStartAndFinishIndexesForMenuList(ItemsWon.Count, visibleItemCount, i, out startIndex, out finishIndex);
                    if (startIndexBefore >= 0)
                    {
                        if (startIndex > startIndexBefore)
                        {
                            minCurrentItemIndex = Math.Max(i - 1, 0);
                            startIndexBefore = -1;
                        }
                    }
                    else if (finishIndexBefore == finishIndex)
                    {
                        maxCurrentItemIndex = i - 1;
                        break;
                    }
                    finishIndexBefore = finishIndex;
                }
            }
        }
        #endregion
    }
}
