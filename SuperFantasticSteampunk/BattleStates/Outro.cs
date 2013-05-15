using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.BattleStates
{
    class Outro : BattleState
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
        public Outro(Battle battle)
            : base(battle)
        {
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            Time = FadeTimeInSeconds;
            BattleStateRenderer = new IntroOutroRenderer(this);
        }

        public override void Finish()
        {
            Battle.Finish();
        }

        public override void Update(GameTime gameTime)
        {
            Time -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Time <= 0.0f)
                Finish();
        }
        #endregion
    }
}
