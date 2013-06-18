using Microsoft.Xna.Framework;
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
            //TODO: Something
        }

        public override void Finish()
        {
            base.Finish();
            Battle.OverworldEncounter.State = EncounterState.Lost;
            ChangeState(new Outro(Battle));
        }

        public override void Update(Delta delta)
        {
            //TODO: Update game over screen
            Finish();
        }
        #endregion
    }
}
