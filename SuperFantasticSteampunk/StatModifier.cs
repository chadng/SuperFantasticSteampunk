namespace SuperFantasticSteampunk
{
    class StatModifier
    {
        // Stat modifiers add a percentage; each property between -1.0 - 1.0
        #region Instance Properties
        public int TurnsLeft { get; private set; }
        public float MaxHealth { get; private set; }
        public float Attack { get; private set; }
        public float SpecialAttack { get; private set; }
        public float Defence { get; private set; }
        public float Speed { get; private set; }
        public float Charm { get; private set; }

        public bool Active
        {
            get { return TurnsLeft >= 0; }
        }
        #endregion

        #region Constructors
        public StatModifier(int turns, float maxHealth = 0.0f, float attack = 0.0f, float specialAttack = 0.0f, float defence = 0.0f, float speed = 0.0f, float charm = 0.0f)
        {
            TurnsLeft = turns;
            MaxHealth = maxHealth;
            Attack = attack;
            SpecialAttack = specialAttack;
            Defence = defence;
            Speed = speed;
            Charm = charm;
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
