using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.BattleStates
{
    class Think : BattleState
    {
        #region Constructors
        public Think(Battle battle)
            : base(battle)
        {
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            //TODO: Set up data structures for storing position data and move selections
        }

        public override void Finish()
        {
            ChangeState(new Act(battle));
        }

        public override void Update(GameTime gameTime)
        {
            //TODO: Allow player to select from a menu to reposition and choose moves for party
            Finish();
        }
        #endregion
    }
}
