namespace SuperFantasticSteampunk.StatusEffects
{
    class Doom : StatusEffect
    {
        #region Constants
        private const int durationInTurns = 3;
        #endregion

        #region Instance Fields
        private int turns;
        #endregion

        #region Instance Properties
        public PartyMember Inflictor { get; private set; }

        public override StatusEffectType Type
        {
            get { return StatusEffectType.Doom; }
        }

        public override bool Active
        {
            get { return Inflictor != null; }
        }
        #endregion

        #region Constructors
        public Doom(PartyMember inflictor)
        {
            Inflictor = inflictor;
            turns = 0;
        }
        #endregion

        #region Instance Methods
        public override void EndTurnStart(PartyMember partyMember)
        {
            base.EndTurnStart(partyMember);

            if (!Inflictor.Alive)
                Inflictor = null;
             else if (++turns >= durationInTurns)
            {
                partyMember.DoDamage(partyMember.Health);
                Inflictor = null;
            }
        }
        #endregion
    }
}
