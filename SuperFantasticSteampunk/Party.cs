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
        public PartyBattleLayout BattleLayout { get; private set; }
        #endregion

        #region Constructors
        public Party(bool isEnemyParty)
        {
            WeaponInventories = new Dictionary<CharacterClass, Inventory>();
            foreach (CharacterClass characterClass in Enum.GetValues(typeof(CharacterClass)))
                WeaponInventories.Add(characterClass, new Inventory(isEnemyParty));
            ShieldInventory = new Inventory(isEnemyParty);
            ItemInventory = new Inventory(isEnemyParty);
        }
        #endregion

        #region Instance Methods
        public void AddPartyMember(PartyMember partyMember)
        {
            Add(partyMember);
            foreach (KeyValuePair<string, int> pair in partyMember.Data.DefaultWeaponsToDictionary())
                WeaponInventories[partyMember.CharacterClass].AddItem(pair.Key, partyMember, pair.Value < 0);
            foreach (KeyValuePair<string, int> pair in partyMember.Data.DefaultShieldsToDictionary())
                ShieldInventory.AddItem(pair.Key, partyMember, pair.Value < 0);
            foreach (KeyValuePair<string, int> pair in partyMember.Data.DefaultItemsToDictionary())
                ItemInventory.AddItem(pair.Key, partyMember, pair.Value < 0);
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
            if (BattleLayout == null)
                InitPartyBattleLayout();
            foreach (PartyMember partyMember in this)
            {
                partyMember.StartBattle();
                partyMember.EquipDefaultWeapon(this);
                battle.AddBattleEntity(partyMember.BattleEntity);
            }
        }

        public void FinishBattle(Battle battle)
        {
            BattleLayout.FinishBattle();
            foreach (PartyMember partyMember in this)
            {
                partyMember.BattleEntity.Kill();
                partyMember.FinishBattle();
            }
        }

        public void InitPartyBattleLayout(string arrangement = null, bool random = false, int minSize = -1)
        {
            BattleLayout = new PartyBattleLayout(this, minSize);
            if (arrangement != null)
                BattleLayout.ArrangeFromString(arrangement);
            else if (random)
                BattleLayout.ArrangeRandomly();
        }

        public bool HasMemberOfCharacterClass(CharacterClass characterClass)
        {
            foreach (PartyMember partyMember in this)
            {
                if (partyMember.CharacterClass == characterClass)
                    return true;
            }
            return false;
        }
        #endregion
    }
}
