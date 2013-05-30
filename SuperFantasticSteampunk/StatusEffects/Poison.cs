namespace SuperFantasticSteampunk.StatusEffects
{
    class Poison : StatusEffect
    {
        #region Instance Fields
        private bool finished;
        #endregion

        #region Instance Properties
        public override StatusEffectType Type
        {
            get { return StatusEffectType.Poison; }
        }

        public override bool Active
        {
            get { return true; }
        }
        #endregion

        #region Constructors
        public Poison()
        {
            finished = false;
        }
        #endregion

        #region Instance Methods
        public override void EndTurnStart(PartyMember partyMember)
        {
            base.EndTurnStart(partyMember);
            finished = false;
        }

        public override void EndTurnUpdate(PartyMember partyMember)
        {
            base.EndTurnUpdate(partyMember);
            int damage = partyMember.Health / 8;
            partyMember.DoDamage(damage > 0 ? damage : 1, true);
            finished = true;
            //TODO: Poison animation
        }

        public override bool EndTurnIsFinished()
        {
            return finished;
        }
        #endregion
    }
}
