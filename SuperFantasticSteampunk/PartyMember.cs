namespace SuperFantasticSteampunk
{
    enum CharacterClass { Warrior, Marksman, Thief, Medic, Enemy }

    class PartyMember
    {
        #region Instance Properties
        public CharacterClass CharacterClass { get; private set; }
        public int MaxHealth { get; private set; }
        public int Health { get; private set; }
        public int Attack { get; private set; }
        public int SpecialAttack { get; private set; }
        public int Defence { get; private set; }
        public int Speed { get; private set; }
        public int Charm { get; private set; }
        public int Level { get; private set; }
        public int Experience { get; private set; }
        #endregion
    }
}
