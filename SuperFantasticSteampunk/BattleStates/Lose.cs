using SuperFantasticSteampunk.OverworldStates;

namespace SuperFantasticSteampunk.BattleStates
{
    class Lose : BattleState
    {
        #region Instance Properties
        public override bool KeepPartyMembersStatic
        {
            get { return true; }
        }
        #endregion

        #region Constructors
        public Lose(Battle battle)
            : base(battle)
        {
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
        }

        public override void Finish()
        {
            base.Finish();
            ChangeState(new Outro(EncounterState.Lost, Battle));
        }

        public override void Update(Delta delta)
        {
            Finish();
        }
        #endregion
    }
}
