using SuperFantasticSteampunk.BattleStates;

namespace SuperFantasticSteampunk.StatusEffects
{
    class Paralysis : StatusEffect
    {
        #region Constants
        private const int chanceOfEffect = 25;
        #endregion

        #region Instance Fields
        private bool finished;
        #endregion

        #region Instance Properties
        public override StatusEffectType Type
        {
            get { return StatusEffectType.Paralysis; }
        }

        public override bool Active
        {
            get { return true; }
        }
        #endregion

        #region Constructors
        public Paralysis()
        {
            finished = false;
        }
        #endregion

        #region Instance Methods
        public override void BeforeActStart(ThinkAction thinkAction)
        {
            base.BeforeActStart(thinkAction);
            finished = false;
        }

        public override void BeforeActUpdate(ThinkAction thinkAction)
        {
            base.BeforeActUpdate(thinkAction);
            if (Game1.Random.Next(100) <= chanceOfEffect)
                thinkAction.Active = false;
            finished = true;
            //TODO: Paralysis animation
        }

        public override bool BeforeActIsFinished()
        {
            return finished;
        }
        #endregion
    }
}
