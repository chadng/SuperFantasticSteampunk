using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Spine;
using SuperFantasticSteampunk.OverworldStates;

namespace SuperFantasticSteampunk
{
    class Battle : Scene
    {
        #region Instance Fields
        private Stack<BattleState> states;
        private bool stateChanged;
        private TextureData whitePixelTextureData;
        private Camera camera;
        #endregion

        #region Instance Properties
        public BattleState CurrentBattleState
        {
            get { return states.Peek(); }
        }

        public Party PlayerParty { get; private set; }
        public Party EnemyParty { get; private set; }

        public PartyBattleLayout PlayerPartyLayout { get; private set; }
        public PartyBattleLayout EnemyPartyLayout { get; private set; }

        public Encounter OverworldEncounter { get; private set; }
        #endregion

        #region Constructors
        public Battle(Party playerParty, Party enemyParty, Encounter overworldEncounter)
        {
            if (playerParty == null)
                throw new Exception("Party playerParty cannot be null");
            if (enemyParty == null)
                throw new Exception("Party enemyParty cannot be null");

            PlayerParty = playerParty.Tap(party => party.StartBattle(this));
            EnemyParty = enemyParty.Tap(party => party.StartBattle(this));

            PlayerPartyLayout = new PartyBattleLayout(PlayerParty);
            EnemyPartyLayout = new PartyBattleLayout(EnemyParty);

            states = new Stack<BattleState>();
            states.Push(new BattleStates.Intro(this));
            stateChanged = true;

            OverworldEncounter = overworldEncounter;

            camera = new Camera(Game1.ScreenSize);
            camera.Position = camera.Size / 2.0f;

            generateBackgroundScenery();
            generateFloorScenery();

            repositionPartyMembers();
            updateCamera();
            camera.Scale = camera.TargetScale;
            updateCamera();
            camera.Position = camera.TargetPosition;
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

        public Party GetPartyForPartyMember(PartyMember partyMember)
        {
            if (PlayerParty.Contains(partyMember))
                return PlayerParty;
            if (EnemyParty.Contains(partyMember))
                return EnemyParty;
            return null;
        }

        public PartyBattleLayout GetPartyBattleLayoutForPartyMember(PartyMember partyMember)
        {
            if (PlayerParty.Contains(partyMember))
                return PlayerPartyLayout;
            if (EnemyParty.Contains(partyMember))
                return EnemyPartyLayout;
            return null;
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

            if (CurrentBattleState.KeepPartyMembersStatic)
                repositionPartyMembers();

            base.update(gameTime);

            if (CurrentBattleState.BattleStateRenderer != null)
                CurrentBattleState.BattleStateRenderer.Update(gameTime);

            updateCamera(() => camera.Update(gameTime));
        }

        protected override void draw(Renderer renderer)
        {
            Game1.BackgroundColor = Game1.GrassColor;
            renderer.Camera = camera;
            renderer.Tint = Clock.GetCurrentColor();

            drawBackground(renderer);

            if (CurrentBattleState.BattleStateRenderer != null)
                CurrentBattleState.BattleStateRenderer.BeforeDraw(renderer);

            base.draw(renderer);

            renderer.ResetTint();

            drawPersistentGui(renderer);

            if (CurrentBattleState.BattleStateRenderer != null)
                CurrentBattleState.BattleStateRenderer.AfterDraw(renderer);

            renderer.Camera = null;
        }

        protected override void finishCleanup()
        {
            Logger.Log("Finishing battle");
            PlayerParty.FinishBattle(this);
            EnemyParty.FinishBattle(this);
            base.finishCleanup();
        }

        private void generateBackgroundScenery()
        {
            const float overflow = 400.0f;
            List<string> scenerySpriteNames = OverworldEncounter.Overworld.Area.Data.BattleBackgroundScenerySpriteNamesToList();
            Vector2 position = new Vector2(-overflow, 200.0f);
            do
            {
                SpriteData spriteData = ResourceManager.GetSpriteData(scenerySpriteNames.Sample());
                if (Game1.Random.Next(3) == 0)
                {
                    Vector2 sceneryPosition = new Vector2(position.X, position.Y + Game1.Random.Next(50));
                    addScenery(spriteData, sceneryPosition);
                    position.X += spriteData.Width + Game1.Random.Next(spriteData.Width);
                }
                else
                    position.X += Game1.Random.Next(spriteData.Width);
            } while (position.X < Game1.ScreenSize.X + overflow);
        }

        private void generateFloorScenery()
        {
            const float overflow = 400.0f;
            const float minStep = 30.0f;
            const int randomStep = 30;
            const float startY = 300.0f;

            List<string> scenerySpriteNames = OverworldEncounter.Overworld.Area.Data.BattleFloorScenerySpriteNamesToList();

            Vector2 position = new Vector2(-overflow, startY);
            do
            {
                do
                {
                    SpriteData spriteData = ResourceManager.GetSpriteData(scenerySpriteNames.Sample());
                    if (Game1.Random.Next(10) == 0)
                    {
                        Vector2 sceneryPosition = new Vector2(position.X, position.Y + Game1.Random.Next(50));
                        addScenery(spriteData, sceneryPosition);
                        position.X += spriteData.Width + Game1.Random.Next(spriteData.Width);
                    }

                    position.Y += minStep + Game1.Random.Next(randomStep);
                } while (position.Y < Game1.ScreenSize.Y + overflow);

                position.X += minStep + Game1.Random.Next(randomStep);
                position.Y = startY;
            } while (position.X < Game1.ScreenSize.X + overflow);
        }

        private void addScenery(SpriteData spriteData, Vector2 position)
        {
            Scenery scenery = new Scenery(new Sprite(spriteData), position);
            scenery.Scale = new Vector2(0.5f);
            if (scenery.Sprite.Data.Animations.Count > 0)
                scenery.Sprite.SetAnimation(new List<string>(scenery.Sprite.Data.Animations.Keys).Sample());
            addEntity(scenery);
        }

        private void updateCamera(Action midUpdateAction = null)
        {
            float borderSize = Game1.ScreenSize.X / (5.0f + ((Game1.ScreenSize.X / 2700.0f) * 10.0f));
            Vector2 firstPosition = (PlayerParty.Count > 0 ? PlayerParty : EnemyParty)[0].BattleEntity.Position;
            float lowestX = firstPosition.X, lowestY = firstPosition.Y;
            float highestX = firstPosition.X, highestY = firstPosition.Y;

            getLowestAndHighestPositionalValuesForParty(PlayerParty, ref lowestX, ref lowestY, ref highestX, ref highestY);
            getLowestAndHighestPositionalValuesForParty(EnemyParty, ref lowestX, ref lowestY, ref highestX, ref highestY);

            lowestX -= borderSize;
            lowestY -= borderSize * 2.5f;
            highestX += borderSize;
            highestY += borderSize;

            float scaleX = Game1.ScreenSize.X / (highestX - lowestX);
            float scaleY = Game1.ScreenSize.Y / (highestY - lowestY);

            if (scaleX < scaleY)
                camera.TargetScale = new Vector2(scaleX);
            else
                camera.TargetScale = new Vector2(scaleY);

            if (midUpdateAction != null)
                midUpdateAction();

            float scale = camera.Scale.X;
            float averageX = (lowestX + highestX) / 2.0f;
            float averageY = (lowestY + highestY) / 2.0f;
            camera.TargetPosition = new Vector2(averageX, averageY) * scale;
        }

        private void getLowestAndHighestPositionalValuesForParty(Party party, ref float lowestX, ref float lowestY, ref float highestX, ref float highestY)
        {
            foreach (PartyMember partyMember in party)
            {
                Vector2 position = partyMember.BattleEntity.Position;

                if (position.X < lowestX)
                    lowestX = position.X;
                else if (position.X > highestX)
                    highestX = position.X;

                if (position.Y < lowestY)
                    lowestY = position.Y;
                else if (position.Y > highestY)
                    highestY = position.Y;
            }
        }

        private void repositionPartyMembers()
        {
            Vector2 position = new Vector2(600.0f, 400.0f);
            PlayerPartyLayout.ForEachList(list =>
            {
                for (int i = 0; i < list.Count; ++i)
                    list[i].BattleEntity.Position = new Vector2(position.X - (200.0f * i), position.Y);
                position.X -= 150.0f;
                position.Y += 150.0f;
            });

            position = new Vector2(1200.0f, 400.0f);
            EnemyPartyLayout.ForEachList(list =>
            {
                for (int i = 0; i < list.Count; ++i)
                    list[i].BattleEntity.Position = new Vector2(position.X + (200.0f * i), position.Y);
                position.X += 150.0f;
                position.Y += 150.0f;
            });
        }

        private void drawBackground(Renderer renderer)
        {
            const float scale = 0.5f;
            Rectangle cameraBoundingBox = camera.GetBoundingBox();

            TextureData textureData = OverworldEncounter.Overworld.Area.BattleBackgroundTextureData;
            float textureWidth = textureData.Width * scale * camera.Scale.X;
            int drawCount = (int)Math.Ceiling(cameraBoundingBox.Right / textureWidth) + 1;
            for (int i = 0; i < drawCount; ++i)
                renderer.Draw(textureData, new Vector2(textureData.Width * scale * i, 0.0f), Color.White, 0.0f, new Vector2(scale));
            
            if (cameraBoundingBox.Left < 0)
            {
                drawCount = (int)(Math.Abs(cameraBoundingBox.Left) / textureWidth) + 1;
                for (int i = -1; i >= -drawCount; --i)
                    renderer.Draw(textureData, new Vector2(textureData.Width * scale * i, 0.0f), Color.White, 0.0f, new Vector2(scale));
            }
        }

        private void drawPersistentGui(Renderer renderer)
        {
            foreach (PartyMember partyMember in PlayerParty)
                drawHealthBar(partyMember, renderer);
        }

        private void drawHealthBar(PartyMember partyMember, Renderer renderer)
        {
            if (whitePixelTextureData == null)
                whitePixelTextureData = ResourceManager.GetTextureData("white_pixel");
            if (whitePixelTextureData == null)
                return;

            float percentageHealth = partyMember.Health / (float)partyMember.MaxHealth;
            Color color = Color.Lerp(Color.Red, Color.Green, percentageHealth);
            float width = 200.0f * percentageHealth;
            float height = 20.0f;

            renderer.Draw(whitePixelTextureData, partyMember.BattleEntity.Position + new Vector2(20.0f), color, 0.0f, new Vector2(width, height));
        }
        #endregion
    }
}
