namespace SuperFantasticSteampunk
{
    enum CharacterClass { Warrior, Marksman, Thief, Medic, Enemy }

    class PartyMemberData // Represents base stats for a PartyMember
    {
        #region Instance Properties
        public string Name { get; private set; }
        public CharacterClass CharacterClass { get; private set; }
        public int MaxHealth { get; private set; }
        public int Attack { get; private set; }
        public int SpecialAttack { get; private set; }
        public int Defence { get; private set; }
        public int Speed { get; private set; }
        public int Charm { get; private set; }
        public int ExperienceMultiplier { get; private set; }
        public string OverworldSpriteName { get; private set; }
        public string BattleSkeletonName { get; private set; }
        public string BattleSkeletonSkinName { get; private set; }
        public float BattleAltitude { get; private set; }
        public string BattleShadowFollowBoneName { get; private set; }
        #endregion
    }
}
