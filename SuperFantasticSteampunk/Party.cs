using System;
using System.Collections.Generic;

namespace SuperFantasticSteampunk
{
    class Party : List<PartyMember>
    {
        #region Instance Properties
        public PartyMember PrimaryPartyMember
        {
            get { return Count == 0 ? null : this[0]; }
        }

        public Dictionary<CharacterClass, Inventory> WeaponInventories { get; private set; }
        public Inventory ShieldInventory { get; private set; }
        public Inventory ItemInventory { get; private set; }
        #endregion

        #region Constructors
        public Party()
        {
            WeaponInventories = new Dictionary<CharacterClass, Inventory>();
            foreach (CharacterClass characterClass in Enum.GetValues(typeof(CharacterClass)))
                WeaponInventories.Add(characterClass, new Inventory());
            ShieldInventory = new Inventory();
            ItemInventory = new Inventory();
        }
        #endregion

        #region Instance Methods
        public void AddPartyMember(PartyMember partyMember)
        {
            Add(partyMember);
            Func<int, int, int> keyExistsHandler = (a, b) => a < 0 || b < 0 ? -1 : a + b;
            WeaponInventories[partyMember.CharacterClass].Merge(partyMember.Data.DefaultWeaponsToDictionary(), keyExistsHandler);
            ShieldInventory.Merge(partyMember.Data.DefaultShieldsToDictionary(), keyExistsHandler);
            ItemInventory.Merge(partyMember.Data.DefaultItemsToDictionary(), keyExistsHandler);
        }

        public void RemovePartyMember(PartyMember partyMember)
        {
            Remove(partyMember);
        }

        public bool SetPrimaryPartyMember(PartyMember partyMember)
        {
            if (Contains(partyMember))
            {
                Remove(partyMember);
                Insert(0, partyMember);
                return true;
            }
            return false;
        }

        public bool SetPrimaryPartyMember(int index)
        {
            if (index < Count)
            {
                PartyMember partyMember = this[index];
                RemoveAt(index);
                Insert(0, partyMember);
                return true;
            }
            return false;
        }

        public void StartBattle(Battle battle)
        {
            foreach (PartyMember partyMember in this)
            {
                partyMember.StartBattle();
                partyMember.EquipDefaultWeapon(this);
                battle.AddBattleEntity(partyMember.BattleEntity);
            }
        }

        public void FinishBattle(Battle battle)
        {
            foreach (PartyMember partyMember in this)
            {
                partyMember.BattleEntity.Kill();
                partyMember.FinishBattle();
            }
        }
        #endregion
    }
}
