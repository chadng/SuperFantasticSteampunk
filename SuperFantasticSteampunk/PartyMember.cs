using System;

namespace SuperFantasticSteampunk
{
    class PartyMember
    {
        #region Instance Properties
        public PartyMemberData PartyMemberData { get; private set; }
        public int MaxHealth { get; private set; }
        public int Health { get; private set; }
        public int Attack { get; private set; }
        public int SpecialAttack { get; private set; }
        public int Defence { get; private set; }
        public int Speed { get; private set; }
        public int Charm { get; private set; }
        public int Level { get; private set; }
        public int Experience { get; private set; }

        public CharacterClass CharacterClass
        {
            get { return PartyMemberData.CharacterClass; }
        }
        #endregion

        #region Constructors
        public PartyMember(PartyMemberData partyMemberData)
        {
            if (partyMemberData == null)
                throw new Exception("PartyMemberData cannot be null");
            PartyMemberData = partyMemberData;
        }
        #endregion
    }
}
