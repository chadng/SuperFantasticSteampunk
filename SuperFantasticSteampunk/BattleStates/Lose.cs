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
            //TODO: Something
        }

        public override void Finish()
        {
            Battle.Finish();
        }

        public override void Update(GameTime gameTime)
        {
            //TODO: Update game over screen
            Finish();
        }
        #endregion
    }
}
