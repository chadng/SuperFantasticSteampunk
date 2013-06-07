using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Spine;
using SuperFantasticSteampunk.OverworldStates;

namespace SuperFantasticSteampunk
{
    class Battle : Scene
    {
        #region Constants
        private const int uiHeight = 170;
        #endregion

        #region Instance Fields
        private Stack<BattleState> states;
        private bool stateChanged;
        private TextureData whitePixelTextureData;
        private Dictionary<CharacterClass, TextureData> characterClassHeadTextureData;
        private List<TextureData> backgroundTextureData;
        private float lowestBackgroundTextureWidth;
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

        public int PlayerPartyItemsUsed { get; private set; }
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

            repositionPartyMembers();
            updateCamera();
            camera.Scale = camera.TargetScale;
            updateCamera();
            camera.Position = camera.TargetPosition;

            generateBackground();
            generateBackgroundScenery();
            generateFloorScenery();

            whitePixelTextureData = ResourceManager.GetTextureData("white_pixel");
            characterClassHeadTextureData = new Dictionary<CharacterClass, TextureData> {
                { CharacterClass.Marksman, ResourceManager.GetTextureData("battle_ui/marksman_head") },
                { CharacterClass.Medic, ResourceManager.GetTextureData("battle_ui/medic_head") }
            };

            PlayerPartyItemsUsed = 0;
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

        public void IncrementItemsUsed(Party party)
        {
            if (party == PlayerParty)
                ++PlayerPartyItemsUsed;
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

        private void generateBackground()
        {
            const int backgroundsToSampleCount = 10;
            backgroundTextureData = new List<TextureData>(backgroundsToSampleCount);
            List<string> battleBackgroundTextureNames = OverworldEncounter.Overworld.Area.Data.BattleBackgroundTextureNamesToList();
            lowestBackgroundTextureWidth = -1;
            for (int i = 0; i < backgroundsToSampleCount; ++i)
            {
                TextureData textureData = ResourceManager.GetTextureData(battleBackgroundTextureNames.Sample());
                backgroundTextureData.Add(textureData);
                if (textureData.Width < lowestBackgroundTextureWidth || lowestBackgroundTextureWidth < 0)
                    lowestBackgroundTextureWidth = textureData.Width;
            }
        }

        private void generateBackgroundScenery()
        {
            const float startY = 200.0f;

            float lowestX, lowestY, highestX, highestY;
            getLowestAndHighestPositionalValuesForParties(out lowestX, out lowestY, out highestX, out highestY);
            float overflow = highestX - lowestX * Game1.ScreenScaleFactor.X;
            lowestX -= overflow;
            highestX += overflow;

            List<string> scenerySpriteNames = OverworldEncounter.Overworld.Area.Data.BattleBackgroundScenerySpriteNamesToList();
            Vector2 position = new Vector2(lowestX, startY);
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
            } while (position.X < highestX);
        }

        private void generateFloorScenery()
        {
            const float minStep = 30.0f;
            const int randomStep = 30;
            const float startY = 300.0f;

            float lowestX, lowestY, highestX, highestY;
            getLowestAndHighestPositionalValuesForParties(out lowestX, out lowestY, out highestX, out highestY);
            Vector2 overflow = new Vector2(highestX - lowestX, highestY - lowestY) * Game1.ScreenScaleFactor;
            lowestX -= overflow.X;
            highestX += overflow.X;
            highestY += overflow.Y;

            List<string> scenerySpriteNames = OverworldEncounter.Overworld.Area.Data.BattleFloorScenerySpriteNamesToList();

            Vector2 position = new Vector2(lowestX, startY);
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
                } while (position.Y < highestY);

                position.X += minStep + Game1.Random.Next(randomStep);
                position.Y = startY;
            } while (position.X < highestX);
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
            Vector2 borderSize = new Vector2(150.0f, 75.0f) * Game1.ScreenScaleFactor;
            float lowestX, lowestY, highestX, highestY;
            getLowestAndHighestPositionalValuesForParties(out lowestX, out lowestY, out highestX, out highestY);

            lowestX -= borderSize.X;
            lowestY -= borderSize.Y * 2.0f;
            highestX += borderSize.X;
            highestY += borderSize.Y + uiHeight;

            float scaleX = Game1.ScreenSize.X / (highestX - lowestX);
            float scaleY = Game1.ScreenSize.Y / (highestY - lowestY);

            if (scaleX < scaleY)
                camera.TargetScale = new Vector2(scaleX);
            else
                camera.TargetScale = new Vector2(scaleY);

            if (midUpdateAction != null)
                midUpdateAction();

            float averageX = (lowestX + highestX) / 2.0f;
            float averageY = (lowestY + highestY) / 2.0f;
            camera.TargetPosition = new Vector2(averageX, averageY);
        }

        private void getLowestAndHighestPositionalValuesForParties(out float lowestX, out float lowestY, out float highestX, out float highestY)
        {
            Vector2 firstPosition = (PlayerParty.Count > 0 ? PlayerParty : EnemyParty)[0].BattleEntity.Position;
            lowestX = firstPosition.X;
            lowestY = firstPosition.Y;
            highestX = firstPosition.X;
            highestY = firstPosition.Y;

            getLowestAndHighestPositionalValuesForParty(PlayerParty, ref lowestX, ref lowestY, ref highestX, ref highestY);
            getLowestAndHighestPositionalValuesForParty(EnemyParty, ref lowestX, ref lowestY, ref highestX, ref highestY);
        }

        private void getLowestAndHighestPositionalValuesForParty(Party party, ref float lowestX, ref float lowestY, ref float highestX, ref float highestY)
        {
            foreach (PartyMember partyMember in party)
            {
                Rectangle boundingBox = partyMember.BattleEntity.GetBoundingBox();

                if (boundingBox.Left < lowestX)
                    lowestX = boundingBox.Left;
                else if (boundingBox.Right > highestX)
                    highestX = boundingBox.Right;

                if (boundingBox.Top < lowestY)
                    lowestY = boundingBox.Top;
                else if (boundingBox.Bottom > highestY)
                    highestY = boundingBox.Bottom;
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

            float textureWidth = lowestBackgroundTextureWidth * scale * camera.Scale.X;
            int drawCount = (int)Math.Ceiling(cameraBoundingBox.Right / textureWidth) + 1;
            int textureDataIndex = 0;
            for (int i = 0; i < drawCount; ++i)
            {
                TextureData textureData = backgroundTextureData[textureDataIndex];
                renderer.Draw(textureData, new Vector2(textureData.Width * scale * i, 0.0f), Color.White, 0.0f, new Vector2(scale));
                if (++textureDataIndex > backgroundTextureData.Count)
                    textureDataIndex = 0;
            }
            
            if (cameraBoundingBox.Left < 0)
            {
                drawCount = (int)Math.Ceiling(Math.Abs(cameraBoundingBox.Left) / textureWidth) + 1;
                textureDataIndex = backgroundTextureData.Count - 1;
                for (int i = -1; i >= -drawCount; --i)
                {
                    TextureData textureData = backgroundTextureData[textureDataIndex];
                    renderer.Draw(textureData, new Vector2(textureData.Width * scale * i, 0.0f), Color.White, 0.0f, new Vector2(scale));
                    if (--textureDataIndex < 0)
                        textureDataIndex = backgroundTextureData.Count - 1;
                }
            }
        }

        private void drawPersistentGui(Renderer renderer)
        {
            Vector2 screenScaleFactor = Game1.ScreenScaleFactor;
            Vector2 headPadding = new Vector2(30.0f) * screenScaleFactor;
            Vector2 barPadding = new Vector2(20.0f) * screenScaleFactor;
            Vector2 barSize = new Vector2(290, 30) * screenScaleFactor;

            Vector2 uiPositition = new Vector2(0.0f, Game1.ScreenSize.Y - (uiHeight * screenScaleFactor.Y));
            Vector2 uiSize = new Vector2(Game1.ScreenSize.X, uiHeight * screenScaleFactor.Y);
            renderer.Draw(whitePixelTextureData, uiPositition, Color.Gray, 0.0f, uiSize, false);
            
            Vector2 position = new Vector2(headPadding.X, uiPositition.Y + headPadding.Y);
            for (int i = 0; i < PlayerParty.Count; ++i)
            {
                drawPartyMemberUi(PlayerParty[i], position, headPadding, barPadding, barSize, renderer);
                position.X += (headPadding.X * 2) + barPadding.X + barSize.X + (characterClassHeadTextureData[PlayerParty[i].CharacterClass].Width * screenScaleFactor.X);
            }
        }

        private void drawPartyMemberUi(PartyMember partyMember, Vector2 position, Vector2 headPadding, Vector2 barPadding, Vector2 barSize, Renderer renderer)
        {
            Vector2 screenScaleFactor = Game1.ScreenScaleFactor;
            Vector2 minScale = new Vector2(MathHelper.Min(screenScaleFactor.X, screenScaleFactor.Y));

            TextureData textureData = characterClassHeadTextureData[partyMember.CharacterClass];

            if (partyMemberIsSelected(partyMember))
            {
                Vector2 underlayPosition = position - (new Vector2(10.0f) * screenScaleFactor);
                Vector2 underlayScale = new Vector2((headPadding.X * 2.0f) + barPadding.X + barSize.X + (textureData.Width * screenScaleFactor.X), ((headPadding.Y * 1.4f) + (textureData.Height * minScale.Y)));
                renderer.Draw(whitePixelTextureData, underlayPosition, new Color(0.6f, 0.6f, 0.6f), 0.0f, underlayScale, false);
            }

            renderer.Draw(textureData, position, Color.White, 0.0f, minScale, false);

            renderer.DrawText(partyMember.Name, position + new Vector2(0.0f, (textureData.Height + 5.0f) * minScale.Y), Color.White, 0.0f, Vector2.Zero, minScale);

            position.X += (textureData.Width * screenScaleFactor.X) + headPadding.X;
            float percentageHealth = partyMember.Health / (float)partyMember.MaxHealth;
            Color healthBarColor = percentageHealth > 0.5f ? Color.Lerp(Color.Yellow, Color.Green, (percentageHealth - 0.5f) / 0.5f) : Color.Lerp(Color.Red, Color.Yellow, percentageHealth / 0.5f);
            drawBar(position, barSize, percentageHealth, healthBarColor, renderer);
            string barText = "HP: " + partyMember.Health.ToString() + "/" + partyMember.MaxHealth;
            renderer.DrawText(barText, position + (new Vector2(10.0f, 7.0f) * screenScaleFactor), Color.White, 0.0f, Vector2.Zero, minScale);
        }

        private void drawBar(Vector2 position, Vector2 size, float percentage, Color color, Renderer renderer)
        {
            renderer.Draw(whitePixelTextureData, position, Color.Black, 0.0f, size, false);
            renderer.Draw(whitePixelTextureData, position, color, 0.0f, new Vector2(size.X * percentage, size.Y), false);
        }

        private bool partyMemberIsSelected(PartyMember partyMember)
        {
            BattleStates.Think thinkState = CurrentBattleState as BattleStates.Think;
            if (thinkState != null)
                return thinkState.CurrentPartyMember == partyMember;

            BattleStates.SelectTarget selectTargetState = CurrentBattleState as BattleStates.SelectTarget;
            if (selectTargetState != null)
                return selectTargetState.Actor == partyMember;

            return false;
        }
        #endregion
    }
}
