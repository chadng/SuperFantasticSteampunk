using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk
{
    class Battle : Scene
    {
        #region Instance Fields
        private BattleState state;
        #endregion

        #region Instance Properties
        public Party PlayerParty { get; private set; }
        public Party EnemyParty { get; private set; }
        #endregion

        #region Constructors
        public Battle(Party playerParty, Party enemyParty)
        {
            PlayerParty = playerParty;
            EnemyParty = enemyParty;
            this.state = new BattleStates.Intro(this);
        }
        #endregion

        #region Instance Methods
        public void ChangeState(BattleState battleState)
        {
            state = battleState;
        }

        protected override void update(GameTime gameTime)
        {
            state.Update(gameTime);
            base.update(gameTime);
        }

        protected override void draw(Spine.SkeletonRenderer skeletonRenderer)
        {
            state.Draw(skeletonRenderer);
            base.draw(skeletonRenderer);
        }
        #endregion
    }
}
