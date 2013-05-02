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

            PlayerParty = playerParty.Tap(party => party.StartBattle(this));
            EnemyParty = enemyParty.Tap(party => party.StartBattle(this));
            states = new Stack<BattleState>();
            states.Push(new BattleStates.Intro(this));
            stateChanged = true;

            Vector2 pos = new Vector2(400.0f);
            foreach (PartyMember partyMember in PlayerParty)
            {
                partyMember.BattleEntity.Position = pos;
                pos.Y += 150.0f;
                pos.X -= 100.0f;
            }
            pos = new Vector2(700.0f, 400.0f);
            foreach (PartyMember partyMember in EnemyParty)
            {
                partyMember.BattleEntity.Position = pos;
                pos.Y += 150.0f;
                pos.X += 100.0f;
            }
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
            Logger.Log(CurrentBattleState.GetType().Name + " battle state paused");
            states.Push(battleState);
            stateChanged = true;
        }

        public void PopState()
        {
            BattleState previousBattleState = states.Pop();
            CurrentBattleState.Resume(previousBattleState);
            Logger.Log(CurrentBattleState.GetType().Name + " battle state resumed");
        }

        public void AddBattleEntity(Entity entity)
        {
            addEntity(entity);
        }

        public void RemoveBattleEntity(Entity entity)
        {
            removeEntity(entity);
        }

        protected override void finish()
        {
            PlayerParty.FinishBattle(this);
            EnemyParty.FinishBattle(this);
            base.finish();
        }

        protected override void update(GameTime gameTime)
        {
            if (stateChanged)
            {
                stateChanged = false;
                CurrentBattleState.Start();
                Logger.Log(CurrentBattleState.GetType().Name + " battle state started");
            }

            CurrentBattleState.Update(gameTime);
            base.update(gameTime);
            if (CurrentBattleState.BattleStateRenderer != null)
                CurrentBattleState.BattleStateRenderer.Update(gameTime);
        }

        protected override void draw(Renderer renderer)
        {
            if (CurrentBattleState.BattleStateRenderer != null)
                CurrentBattleState.BattleStateRenderer.BeforeDraw(renderer);
            base.draw(renderer);
            if (CurrentBattleState.BattleStateRenderer != null)
                CurrentBattleState.BattleStateRenderer.AfterDraw(renderer);
        }
        #endregion
    }
}
