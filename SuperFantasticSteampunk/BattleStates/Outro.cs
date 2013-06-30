using Microsoft.Xna.Framework;
using SuperFantasticSteampunk.OverworldStates;

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
        public Outro(EncounterState encounterState, Battle battle)
            : base(battle)
        {
            Battle.OverworldEncounter.State = encounterState;
            Logger.Log(battle.PlayerPartyItemsUsed.ToString() + " items were used");
            BattleStateRenderer = new IntroOutroRenderer(this);
            Time = FadeTimeInSeconds * (encounterState == EncounterState.Won ? (1.0f - WinRenderer.MaxOverlayAlpha) : 1.0f);
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
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
