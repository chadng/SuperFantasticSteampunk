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
            throw new System.NotImplementedException();
        }

        public override void Finish()
        {
            ChangeState(new Act(battle));
        }

        public override void Update(GameTime gameTime)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
