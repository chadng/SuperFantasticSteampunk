using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SuperFantasticSteampunk.OverworldStates;

namespace SuperFantasticSteampunk.BattleStates
{
    class Win : BattleState
    {
        #region Constants
        private const int minimumForItemsWonRandomizer = 5;
        private const int itemsUsedDivisorForItemsWonRandomizer = 5;
        #endregion

        #region Instance Properties
        public override bool KeepPartyMembersStatic
        {
            get { return true; }
        }
        #endregion

        #region Constructors
        public Win(Battle battle)
            : base(battle)
        {
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            addNewItemsToPlayerPartyInventory();
        }

        public override void Finish()
        {
            Battle.OverworldEncounter.State = EncounterState.Won;
            ChangeState(new Outro(Battle));
        }

        public override void Update(Delta delta)
        {
            //TODO: Update win screen
            Finish();
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
                ThinkActionType.UseItem
            };

            int count = Battle.PlayerPartyItemsUsed + Game1.Random.Next(Math.Max(minimumForItemsWonRandomizer, Battle.PlayerPartyItemsUsed / itemsUsedDivisorForItemsWonRandomizer));
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
                    Battle.PlayerParty.WeaponInventories[weaponData.CharacterClass].AddItem(weaponData.Name);
                    Logger.Log("Player party won weapon " + weaponData.Name);
                    break;

                case ThinkActionType.Defend:
                    if (shields[rarity].Count == 0)
                    {
                        --i;
                        continue;
                    }
                    ShieldData shieldData = shields[rarity].Sample();
                    Battle.PlayerParty.ShieldInventory.AddItem(shieldData.Name);
                    Logger.Log("Player party won shield " + shieldData.Name);
                    break;

                case ThinkActionType.UseItem:
                    if (items[rarity].Count == 0)
                    {
                        --i;
                        continue;
                    }
                    ItemData itemData = items[rarity].Sample();
                    Battle.PlayerParty.ItemInventory.AddItem(itemData.Name);
                    Logger.Log("Player party won item " + itemData.Name);
                    break;
                }
            }
        }
        #endregion
    }
}
