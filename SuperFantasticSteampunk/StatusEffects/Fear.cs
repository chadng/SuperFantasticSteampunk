namespace SuperFantasticSteampunk.StatusEffects
{
    class Fear : StatusEffect
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
            get { return StatusEffectType.Fear; }
        }

        public override bool Active
        {
            get { return turns <= durationInTurns; }
        }
        #endregion

        #region Constructors
        public Fear(PartyMember inflictor)
        {
            Inflictor = inflictor;
            turns = 0;
        }
        #endregion

        #region Instance Methods
        public override void BeforeActStart(BattleStates.ThinkAction thinkAction)
        {
            base.BeforeActStart(thinkAction);
            thinkAction.Active = false;
        }

        public override void EndTurnStart(PartyMember partyMember)
        {
            base.EndTurnStart(partyMember);
            if (Game1.Random.Next(durationInTurns) < turns)
                turns = durationInTurns + 1;
            else
                ++turns;
        }
        #endregion
    }
}
