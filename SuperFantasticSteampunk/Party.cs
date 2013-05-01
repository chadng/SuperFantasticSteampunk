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
        public Dictionary<CharacterClass, Inventory> ShieldInventories { get; private set; }
        public Inventory ItemInventory { get; private set; }
        #endregion

        #region Constructors
        public Party()
        {
            WeaponInventories = new Dictionary<CharacterClass, Inventory>();
            ShieldInventories = new Dictionary<CharacterClass, Inventory>();
            foreach (CharacterClass characterClass in Enum.GetValues(typeof(CharacterClass)))
            {
                WeaponInventories.Add(characterClass, new Inventory());
                ShieldInventories.Add(characterClass, new Inventory());
            }
            ItemInventory = new Inventory();
        }
        #endregion

        #region Instance Methods
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
        #endregion
    }
}
