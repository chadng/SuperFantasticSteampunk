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
        public Area Area { get; private set; }
        #endregion

        #region Constructors
        public Overworld(Party playerParty, Area area)
        {
            if (playerParty == null)
                throw new Exception("Party playerParty cannot be null");

            PlayerParty = playerParty;
            Area = area;

            EnemyParties = new List<Party>();

            states = new Stack<OverworldState>();
            states.Push(new OverworldStates.Play(this));
            stateChanged = true;

            Map = new Map(100, 100, 5, 5);

            PlayerParty.PrimaryPartyMember.StartOverworld(new Vector2(200.0f));
            addEntity(PlayerParty.PrimaryPartyMember.OverworldEntity);

            camera = new Camera(Game1.ScreenSize);
            camera.Target = playerParty.PrimaryPartyMember.OverworldEntity;

            populateWithScenery();
            populateWithEnemies();
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

        public void AddEnemyParty(Party party, Vector2 position)
        {
            if (party.PrimaryPartyMember.OverworldEntity == null)
                party.PrimaryPartyMember.StartOverworld(position);
            EnemyParties.Add(party);
            addEntity(party.PrimaryPartyMember.OverworldEntity);
        }

        public void SetEntityAnimationForVelocity(Entity entity)
        {
            if (entity.Skeleton == null)
                setEntitySpriteAnimationForVelocity(entity);
            else
                setEntitySkeletonAnimationForVelocity(entity);
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
            Clock.Update(gameTime);
        }

        protected override void draw(Renderer renderer)
        {
            Game1.BackgroundColor = Game1.GrassColor;
            renderer.Camera = camera;
            renderer.Tint = Clock.GetCurrentColor();

            if (CurrentOverworldState.OverworldStateRenderer != null)
                CurrentOverworldState.OverworldStateRenderer.BeforeDraw(renderer);

            drawMap(renderer);

            base.draw(renderer);


            renderer.ResetTint();

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

        private void populateWithScenery()
        {
            AddEntity(new Scenery(ResourceManager.GetNewSprite(Area.ScenerySpriteNames.Sample()), new Vector2(100.0f, 400.0f)));
        }

        private void populateWithEnemies()
        {
            for (int i = 3; i < 6; ++i)
            {
                Party enemyParty = new Party();
                enemyParty.AddPartyMember(ResourceManager.GetNewPartyMember(Area.EnemyNames.Sample()));
                enemyParty.AddPartyMember(ResourceManager.GetNewPartyMember(Area.EnemyNames.Sample()));
                enemyParty.AddPartyMember(ResourceManager.GetNewPartyMember(Area.EnemyNames.Sample()));
                enemyParty.AddPartyMember(ResourceManager.GetNewPartyMember(Area.EnemyNames.Sample()));
                AddEnemyParty(enemyParty, new Vector2(100.0f * i));
            }
        }

        private void drawMap(Renderer renderer)
        {
            TextureData pixelTexture = ResourceManager.GetTextureData("white_pixel");
            TileData testTile = Area.Data.OverworldTileTextureNamesToList()[0];
            TextureData testTexturData = ResourceManager.GetTextureData(testTile.TextureDataName);

            Rectangle cameraBoundingBox = camera.GetBoundingBox();
            int startX = Math.Max((cameraBoundingBox.Left / Map.TileSize) - 1, 0);
            int finishX = Math.Min(((cameraBoundingBox.Left + cameraBoundingBox.Width) / Map.TileSize) + 2, Map.TileWidth - 1);
            int startY = Math.Max((cameraBoundingBox.Top / Map.TileSize) - 1, 0);
            int finishY = Math.Min(((cameraBoundingBox.Top + cameraBoundingBox.Height) / Map.TileSize) + 2, Map.TileHeight - 1);

            for (int x = startX; x <= finishX; ++x)
            {
                for (int y = startY; y <= finishY; ++y)
                {
                    if (Map.CollisionMap[x, y] && x % 2 == 0 && y % 2 == 0)
                    {
                        //renderer.Draw(pixelTexture, new Vector2(x, y) * Map.TileSize, Color.Black, 0.0f, new Vector2(Map.TileSize));
                        renderer.Draw(testTexturData, new Vector2(x + 0.5f, y + 0.5f) * Map.TileSize, Color.White);
                    }
                }
            }
        }

        private void setEntitySpriteAnimationForVelocity(Entity entity)
        {
            if (entity.Velocity == Vector2.Zero)
            {
                if (entity.Sprite.CurrentAnimation == null)
                    entity.Sprite.SetAnimation("idle_down");
                else
                {
                    string animationName = entity.Sprite.CurrentAnimation.Name;
                    if (!animationName.StartsWith("idle_"))
                        entity.Sprite.SetAnimation(animationName.Replace("walk_", "idle_"));
                }
            }
            else
            {
                string animationName = "walk_" + directionFromVelocity(entity.Velocity);
                if (entity.Sprite.CurrentAnimation == null || animationName != entity.Sprite.CurrentAnimation.Name)
                    entity.Sprite.SetAnimation(animationName);
            }
        }

        private void setEntitySkeletonAnimationForVelocity(Entity entity)
        {
            if (entity.Velocity == Vector2.Zero)
            {
                if (entity.AnimationState.Animation == null)
                    entity.AnimationState.SetAnimation("idle_down", true);
                else
                {
                    string animationName = entity.AnimationState.Animation.Name;
                    if (!animationName.StartsWith("idle_"))
                        entity.AnimationState.SetAnimation(animationName.Replace("walk_", "idle_"), true);
                }
            }
            else
            {
                string animationName = "walk_" + directionFromVelocity(entity.Velocity);
                if (entity.AnimationState.Animation == null || animationName != entity.AnimationState.Animation.Name)
                    entity.AnimationState.SetAnimation(animationName, true);
            }
        }

        private string directionFromVelocity(Vector2 velocity)
        {
            const double oneOverRootTwo = 0.7071067811865475;

            velocity.Normalize();

            if (velocity.Y <= -oneOverRootTwo)
                return "up";
            else if (velocity.Y >= oneOverRootTwo)
                return "down";
            else if (velocity.X < 0)
                return "left";
            else if (velocity.X > 0)
                return "right";

            return "down";
        }
        #endregion
    }
}
