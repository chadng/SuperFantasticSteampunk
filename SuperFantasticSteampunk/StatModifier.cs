namespace SuperFantasticSteampunk
{
    class StatModifier
    {
        // Stat modifiers add an absolute value
        #region Instance Properties
        public int TurnsLeft { get; private set; }
        public int MaxHealth { get; private set; }
        public int MeleeAttack { get; private set; }
        public int RangedAttack { get; private set; }
        public int Defence { get; private set; }
        public int Speed { get; private set; }

        public bool Active
        {
            get { return TurnsLeft >= 0; }
        }
        #endregion

        #region Constructors
        public StatModifier(int turns, int maxHealth = 0, int meleeAttack = 0, int rangedAttack = 0, int defence = 0, int speed = 0)
        {
            TurnsLeft = turns;
            MaxHealth = maxHealth;
            MeleeAttack = meleeAttack;
            RangedAttack = rangedAttack;
            Defence = defence;
            Speed = speed;
        }
        #endregion

        #region Instance Methods
        public void DecrementTurnsLeft()
        {
            --TurnsLeft;
        }
        #endregion
    }
}
