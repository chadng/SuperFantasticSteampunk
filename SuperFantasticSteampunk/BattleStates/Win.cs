using Microsoft.Xna.Framework;
using SuperFantasticSteampunk.OverworldStates;

namespace SuperFantasticSteampunk.BattleStates
{
    class Win : BattleState
    {
        #region Instance Properties
        public override bool KeepPartyMembersStatic
        {
            get { return true; }
        }
        #endregion

        #region Constructors
        public Win(Battle battle)
            : base(battle)
        {
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            //TODO: Calculate experience
        }

        public override void Finish()
        {
            Battle.OverworldEncounter.State = EncounterState.Won;
            ChangeState(new Outro(Battle));
        }

        public override void Update(GameTime gameTime)
        {
            //TODO: Update win screen
            Finish();
        }
        #endregion
    }
}
