using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.BattleStates
{
    class Intro : BattleState
    {
        #region Constants
        public const float FadeTimeInSeconds = 0.8f;
        #endregion

        #region Instance Properties
        public float Time { get; private set; }

        public override bool KeepPartyMembersStatic
        {
            get { return true; }
        }
        #endregion

        #region Constructors
        public Intro(Battle battle)
            : base(battle)
        {
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            Time = 0.0f;
            BattleStateRenderer = new IntroOutroRenderer(this);
        }

        public override void Finish()
        {
            base.Finish();
            ChangeState(new Think(Battle));
        }

        public override void Update(Delta delta)
        {
            Time += delta.Time;
            if (Time >= FadeTimeInSeconds)
                Finish();
        }
        #endregion
    }
}
