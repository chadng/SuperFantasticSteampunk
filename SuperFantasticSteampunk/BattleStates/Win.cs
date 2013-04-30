using Microsoft.Xna.Framework;

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
            throw new System.NotImplementedException();
        }

        public override void Finish()
        {
            Scene.Finish();
        }

        public override void Update(GameTime gameTime)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
