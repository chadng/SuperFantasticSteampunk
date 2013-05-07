namespace SuperFantasticSteampunk
{
    enum WeaponType { Melee, Ranged }
    enum WeaponUseAgainst { Player, Enemy, Both }

    class WeaponData
    {
        #region Instance properties
        public string Name { get; private set; }
        public WeaponType WeaponType { get; private set; }
        public WeaponUseAgainst WeaponUseAgainst { get; private set; }
        public CharacterClass CharacterClass { get; private set; }
        public int Power { get; private set; }
        public string TextureName { get; private set; }
        #endregion
    }
}
