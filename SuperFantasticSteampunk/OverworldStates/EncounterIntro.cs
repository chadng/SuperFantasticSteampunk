using System;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.OverworldStates
{
    class EncounterIntro : OverworldState
    {
        #region Constants
        public const float FadeTimeInSeconds = 0.4f;
        #endregion

        #region Instance Fields
        private Party enemyParty;
        #endregion

        #region Instance Properties
        public float Time { get; private set; }
        #endregion

        #region Constructors
        public EncounterIntro(Overworld overworld, Party enemyParty)
            : base(overworld)
        {
            if (enemyParty == null)
                throw new Exception("Party enemyParty cannot be null");
            this.enemyParty = enemyParty;
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            Time = 0.0f;
            OverworldStateRenderer = new EncounterIntroRenderer(this);
        }

        public override void Finish()
        {
            base.Finish();
            PopState();
        }

        public override void Pause()
        {
            enemyParty = null;
            base.Pause();
        }

        public override void Resume(OverworldState previousOverworldState)
        {
            base.Resume(previousOverworldState);
            Time = FadeTimeInSeconds;
        }

        public override void Update(Delta delta)
        {
            if (enemyParty != null) // if before battle
            {
                Time += delta.Time;
                if (Time >= FadeTimeInSeconds)
                    PushState(new Encounter(Overworld, enemyParty));
            }
            else
            {
                Time -= delta.Time;
                if (Time <= 0.0f)
                    Finish();
            }
        }
        #endregion
    }
}
