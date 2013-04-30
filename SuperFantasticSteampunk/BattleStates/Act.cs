using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.BattleStates
{
    class Act : BattleState
    {
        #region Constructors
        public Act(Battle battle)
            : base(battle)
        {
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            //TODO: Order entities by speed, setup for moving
        }

        public override void Finish()
        {
            ChangeState(new EndTurn(battle));
        }

        public override void Update(GameTime gameTime)
        {
            //TODO: Perform move for each entity. Defence stances happen first
            Finish();
        }
        #endregion
    }
}
