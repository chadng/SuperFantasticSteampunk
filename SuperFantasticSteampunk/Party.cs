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

        public Inventory WeaponInventory { get; private set; }
        public Inventory ItemInventory { get; private set; }
        #endregion

        #region Constructors
        public Party()
        {
            WeaponInventory = new Inventory();
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
