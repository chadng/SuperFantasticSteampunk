using SuperFantasticSteampunk.OverworldStates;

namespace SuperFantasticSteampunk.BattleStates
{
    class Run : BattleState
    {
        #region Instance Properties
        public override bool KeepPartyMembersStatic
        {
            get { return true; }
        }
        #endregion

        #region Constructors
        public Run(Battle battle)
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
            Battle.OverworldEncounter.State = EncounterState.Ran;
            ChangeState(new Outro(Battle));
        }

        public override void Update(Delta delta)
        {
            Finish();
        }
        #endregion
    }
}
