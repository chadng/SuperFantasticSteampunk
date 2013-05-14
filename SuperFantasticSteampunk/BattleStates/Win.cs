using Microsoft.Xna.Framework;
using SuperFantasticSteampunk.OverworldStates;

namespace SuperFantasticSteampunk.BattleStates
{
    class Win : BattleState
    {
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
            Battle.Finish();
        }

        public override void Update(GameTime gameTime)
        {
            //TODO: Update win screen
            Finish();
        }
        #endregion
    }
}
