using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk
{
    class Overworld : Scene
    {
        #region Instance Fields
        private Stack<OverworldState> states;
        private bool stateChanged;
        #endregion

        #region Instance Properties
        public OverworldState CurrentOverworldState
        {
            get { return states.Peek(); }
        }

        public Party PlayerParty { get; private set; }
        public List<Party> EnemyParties { get; private set; }
        #endregion

        #region Constructors
        public Overworld(Party playerParty)
        {
            if (playerParty == null)
                throw new Exception("Party playerParty cannot be null");

            PlayerParty = playerParty;
            EnemyParties = new List<Party>();

            states = new Stack<OverworldState>();
            states.Push(new OverworldStates.Play(this));
            stateChanged = true;

            PlayerParty.PrimaryPartyMember.StartOverworld(new Vector2(100.0f));
            addEntity(PlayerParty.PrimaryPartyMember.OverworldEntity);
        }
        #endregion

        #region Instance Methods
        public void ChangeState(OverworldState overworldState)
        {
            states.Pop();
            states.Push(overworldState);
            stateChanged = true;
        }

        public void PushState(OverworldState overworldState)
        {
            CurrentOverworldState.Pause();
            Logger.Log(CurrentOverworldState.GetType().Name + " overworld state paused");
            states.Push(overworldState);
            stateChanged = true;
        }

        public void PopState()
        {
            OverworldState previousOverworldState = states.Pop();
            CurrentOverworldState.Resume(previousOverworldState);
            Logger.Log(CurrentOverworldState.GetType().Name + " overworld state resumed");
        }

        protected override void update(GameTime gameTime)
        {
            if (stateChanged)
            {
                stateChanged = false;
                CurrentOverworldState.Start();
                Logger.Log(CurrentOverworldState.GetType().Name + " overworld state started");
            }

            CurrentOverworldState.Update(gameTime);
            base.update(gameTime);

            if (CurrentOverworldState.OverworldStateRenderer != null)
                CurrentOverworldState.OverworldStateRenderer.Update(gameTime);
        }

        protected override void draw(Renderer renderer)
        {
            if (CurrentOverworldState.OverworldStateRenderer != null)
                CurrentOverworldState.OverworldStateRenderer.BeforeDraw(renderer);
            base.draw(renderer);
            if (CurrentOverworldState.OverworldStateRenderer != null)
                CurrentOverworldState.OverworldStateRenderer.AfterDraw(renderer);
        }
        #endregion
    }
}
