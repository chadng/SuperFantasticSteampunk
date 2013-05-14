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
        private Camera camera;
        #endregion

        #region Instance Properties
        public OverworldState CurrentOverworldState
        {
            get { return states.Peek(); }
        }

        public Party PlayerParty { get; private set; }
        public List<Party> EnemyParties { get; private set; }
        public Map Map { get; private set; }
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

            Map = new Map(500, 500, 5, 5);

            PlayerParty.PrimaryPartyMember.StartOverworld(new Vector2(100.0f));
            addEntity(PlayerParty.PrimaryPartyMember.OverworldEntity);

            camera = new Camera(new Vector2(Game1.ScreenWidth, Game1.ScreenHeight));
            camera.Target = playerParty.PrimaryPartyMember.OverworldEntity;
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

            camera.Update(gameTime);
        }

        protected override void draw(Renderer renderer)
        {
            renderer.Camera = camera;

            if (CurrentOverworldState.OverworldStateRenderer != null)
                CurrentOverworldState.OverworldStateRenderer.BeforeDraw(renderer);

            base.draw(renderer);

            drawMap(renderer);

            if (CurrentOverworldState.OverworldStateRenderer != null)
                CurrentOverworldState.OverworldStateRenderer.AfterDraw(renderer);

            renderer.Camera = null;
        }

        protected override void finishCleanup()
        {
            PlayerParty.PrimaryPartyMember.Try(p => p.FinishOverworld());
            foreach (Party party in EnemyParties)
                party.PrimaryPartyMember.FinishOverworld();

            base.finishCleanup();
        }

        private void drawMap(Renderer renderer)
        {
            TextureData pixelTexture = ResourceManager.GetTextureData("white_pixel");

            Rectangle cameraBoundingBox = camera.GetBoundingBox();
            int startX = cameraBoundingBox.Left / Map.TileSize;
            int finishX = Math.Min((cameraBoundingBox.Left + cameraBoundingBox.Width) / Map.TileSize, Map.TileWidth);
            int startY = cameraBoundingBox.Top / Map.TileSize;
            int finishY = Math.Min((cameraBoundingBox.Top + cameraBoundingBox.Height) / Map.TileSize, Map.TileHeight);

            for (int x = startX; x <= finishX; ++x)
            {
                for (int y = startY; y <= finishY; ++y)
                {
                    if (Map.CollisionMap[x, y])
                        renderer.Draw(pixelTexture, new Vector2(x, y) * Map.TileSize, Color.Black, 0.0f, new Vector2(Map.TileSize));
                }
            }
        }
        #endregion
    }
}
