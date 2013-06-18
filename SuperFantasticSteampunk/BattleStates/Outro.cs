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
            Logger.Log(battle.PlayerPartyItemsUsed.ToString() + " items were used");
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
            base.Finish();
            Battle.Finish();
        }

        public override void Update(Delta delta)
        {
            Time -= delta.Time;
            if (Time <= 0.0f)
                Finish();
        }
        #endregion
    }
}
