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
                WeaponInventories.Add(characterClass, new Inventory().Tap(i =>
                {
                    i.Add("test1", 5);
                    i.Add("test2", 5);
                    i.Add("default", -1);
                }));
                ShieldInventories.Add(characterClass, new Inventory().Tap(i =>
                {
                    i.Add("shield1", 5);
                    i.Add("shield2", 5);
                }));
            }
            ItemInventory = new Inventory();
        }
        #endregion

        #region Instance Methods
        public void AddPartyMember(PartyMember partyMember)
        {
            Add(partyMember);
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
                battle.AddBattleEntity(partyMember.BattleEntity);
            }
        }

        public void FinishBattle(Battle battle)
        {
            foreach (PartyMember partyMember in this)
            {
                battle.RemoveBattleEntity(partyMember.BattleEntity);
                partyMember.FinishBattle();
            }
        }
        #endregion
    }
}
