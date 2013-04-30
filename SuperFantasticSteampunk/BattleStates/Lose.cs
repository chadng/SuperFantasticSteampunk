using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.BattleStates
{
    class Lose : BattleState
    {
        #region Constructors
        public Lose(Battle battle)
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
