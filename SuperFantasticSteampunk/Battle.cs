using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Spine;

namespace SuperFantasticSteampunk
{
    class Battle : Scene
    {

        #region Instance Fields
        private Stack<BattleState> states;
        private bool stateChanged;
        #endregion

        #region Instance Properties
        public BattleState CurrentBattleState
        {
            get { return states.Peek(); }
        }

        public Party PlayerParty { get; private set; }
        public Party EnemyParty { get; private set; }
        #endregion

        #region Constructors
        public Battle(Party playerParty, Party enemyParty)
        {
            if (playerParty == null)
                throw new Exception("Party playerParty cannot be null");
            if (enemyParty == null)
                throw new Exception("Party enemyParty cannot be null");

            PlayerParty = playerParty;
            EnemyParty = enemyParty;
            states = new Stack<BattleState>();
            states.Push(new BattleStates.Intro(this));
            stateChanged = true;
        }
        #endregion

        #region Instance Methods
        public void ChangeState(BattleState battleState)
        {
            states.Pop();
            states.Push(battleState);
            stateChanged = true;
        }

        public void PushState(BattleState battleState)
        {
            CurrentBattleState.Pause();
            states.Push(battleState);
            stateChanged = true;
        }

        public void PopState()
        {
            BattleState previousBattleState = states.Pop();
            CurrentBattleState.Resume(previousBattleState);
        }

        protected override void update(GameTime gameTime)
        {
            if (stateChanged)
            {
                stateChanged = false;
#if DEBUG
                System.Console.WriteLine(CurrentBattleState.GetType().Name + " battle state started");
#endif
                CurrentBattleState.Start();
            }

            CurrentBattleState.Update(gameTime);
            base.update(gameTime);
        }

        protected override void draw(SkeletonRenderer skeletonRenderer)
        {
            CurrentBattleState.Draw(skeletonRenderer);
            base.draw(skeletonRenderer);
        }
        #endregion
    }
}
