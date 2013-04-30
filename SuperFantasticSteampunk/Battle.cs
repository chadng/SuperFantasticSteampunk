using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Spine;

namespace SuperFantasticSteampunk
{
    class Battle : Scene
    {
        #region Instance Fields
        private BattleState state;
        private bool stateChanged;
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
            stateChanged = true;
        }
        #endregion

        #region Instance Methods
        public void ChangeState(BattleState battleState)
        {
            state = battleState;
            stateChanged = true;
        }

        protected override void update(GameTime gameTime)
        {
            if (stateChanged)
            {
                stateChanged = false;
#if DEBUG
                System.Console.WriteLine(state.GetType().Name + " battle state started");
#endif
                state.Start();
            }

            state.Update(gameTime);
            base.update(gameTime);
        }

        protected override void draw(SkeletonRenderer skeletonRenderer)
        {
            state.Draw(skeletonRenderer);
            base.draw(skeletonRenderer);
        }
        #endregion
    }
}
