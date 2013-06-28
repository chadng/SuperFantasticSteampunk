using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Spine;
using SuperFantasticSteampunk.OverworldStates;

namespace SuperFantasticSteampunk
{
    class Battle : Scene
    {
        #region Constants
        public static readonly Color UiColor = new Color(98, 55, 56);
        private const int uiHeight = 170;
        #endregion

        #region Instance Fields
        public readonly Dictionary<CharacterClass, TextureData> CharacterClassHeadTextureData;
        private readonly TextureData whitePixelTextureData;
        private readonly TextureData arrowTextureData;
        private readonly TextureData borderTextureDataN;
        private readonly TextureData borderTextureDataE;
        private readonly TextureData borderTextureDataW;
        private readonly TextureData borderTextureDataNE;
        private readonly TextureData borderTextureDataNW;
        private readonly Dictionary<InputButton, TextureData> gamepadButtonTextureData;
        private readonly TextureData blankGamepadTextureData;
        private Stack<BattleState> states;
        private bool stateChanged;
        private List<TextureData> backgroundTextureData;
        private float lowestBackgroundTextureWidth;
        private float cameraUpdateDelay;
        #endregion

        #region Instance Properties
        public BattleState CurrentBattleState
        {
            get { return states.Peek(); }
        }

        public Party PlayerParty { get; private set; }
        public Party EnemyParty { get; private set; }

        public PartyBattleLayout PlayerPartyLayout
        {
            get { return PlayerParty.BattleLayout; }
        }
        public PartyBattleLayout EnemyPartyLayout
        {
            get { return EnemyParty.BattleLayout; }
        }

        public Encounter OverworldEncounter { get; private set; }
        public int PlayerPartyItemsUsed { get; private set; }
        public Camera Camera { get; private set; }
        public ConditionalWeakTable<PartyMember, Wrapper<BattleStates.ThinkActionType>> LastUsedThinkActionTypes { get; private set; }
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

            states = new Stack<BattleState>();
            states.Push(new BattleStates.Intro(this));
            stateChanged = true;

            OverworldEncounter = overworldEncounter;

            Camera = new Camera(Game1.ScreenSize);
            Camera.Position = Camera.Size / 2.0f;
            cameraUpdateDelay = 0.0f;

            repositionPartyMembers();
            updateCamera();
            Camera.Scale = Camera.TargetScale;
            updateCamera();
            Camera.Position = Camera.TargetPosition;

            generateBackground();
            generateBackgroundScenery();
            generateFloorScenery();

            whitePixelTextureData = ResourceManager.GetTextureData("white_pixel");
            arrowTextureData = ResourceManager.GetTextureData("arrow_down");
            borderTextureDataN = ResourceManager.GetTextureData("battle_ui/borders/n");
            borderTextureDataE = ResourceManager.GetTextureData("battle_ui/borders/e");
            borderTextureDataW = ResourceManager.GetTextureData("battle_ui/borders/w");
            borderTextureDataNE = ResourceManager.GetTextureData("battle_ui/borders/ne");
            borderTextureDataNW = ResourceManager.GetTextureData("battle_ui/borders/nw");
            CharacterClassHeadTextureData = new Dictionary<CharacterClass, TextureData> {
                { CharacterClass.Warrior, ResourceManager.GetTextureData("battle_ui/warrior_head") },
                { CharacterClass.Marksman, ResourceManager.GetTextureData("battle_ui/marksman_head") },
                { CharacterClass.Medic, ResourceManager.GetTextureData("battle_ui/medic_head") },
                { CharacterClass.Thief, ResourceManager.GetTextureData("battle_ui/thief_head") }
            };
            gamepadButtonTextureData = new Dictionary<InputButton, TextureData> {
                { InputButton.A, ResourceManager.GetTextureData("battle_ui/buttons/gamepad_a") },
                { InputButton.B, ResourceManager.GetTextureData("battle_ui/buttons/gamepad_b") },
                { InputButton.LeftTrigger, ResourceManager.GetTextureData("battle_ui/buttons/gamepad_lt") }
            };
            blankGamepadTextureData = ResourceManager.GetTextureData("battle_ui/buttons/gamepad_blank");

            PlayerPartyItemsUsed = 0;
            LastUsedThinkActionTypes = new ConditionalWeakTable<PartyMember, Wrapper<BattleStates.ThinkActionType>>();
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

        public void SetCameraUpdateDelay(float time)
        {
            if (time > cameraUpdateDelay)
                cameraUpdateDelay = time;
        }

        public void CalculateStartAndFinishIndexesForMenuList(int optionsCount, int maxVisibleOptions, int currentIndex, out int startIndex, out int finishIndex)
        {
            startIndex = 0;
            finishIndex = optionsCount - 1;
            if (optionsCount > maxVisibleOptions)
            {
                int remainder;
                int optionsAbove = Math.DivRem(maxVisibleOptions - 1, 2, out remainder);
                int optionsBelow = optionsAbove + remainder;
                startIndex = currentIndex - optionsAbove;
                finishIndex = currentIndex + optionsBelow;
                if (startIndex < 0)
                    finishIndex -= startIndex;
                else if (finishIndex >= optionsCount)
                    startIndex = optionsCount - maxVisibleOptions;
                startIndex = Math.Max(startIndex, 0);
                finishIndex = Math.Min(finishIndex, optionsCount - 1);
            }
        }

        public void DrawArrowOverPartyMember(PartyMember partyMember, Color color, Renderer renderer)
        {
            Rectangle boundingBox = partyMember.BattleEntity.GetBoundingBox();
            Vector2 position = new Vector2(boundingBox.X + (boundingBox.Width / 2) - (arrowTextureData.Width / 2), boundingBox.Y - arrowTextureData.Height);
            renderer.Draw(arrowTextureData, position, color);
        }

        public Vector2 DrawButtonWithText(InputButton button, string text, Vector2 position, Renderer renderer)
        {
            if (text != null && text.Length == 0)
                text = null;

            TextureData textureData = gamepadButtonTextureData[button];
            Vector2 minScale = Game1.MinScreenScaleFactor;
            Vector2 textSize = renderer.Font.MeasureString(text ?? "I", Font.DefaultSize * minScale.Y);
            Vector2 buttonScale = new Vector2((1.0f / textureData.Height) * textSize.Y * 1.1f);

            if (text != null)
            {
                float halfButtonWidth = textureData.Width * 0.5f * buttonScale.X;
                Vector2 backingSize = new Vector2((textSize.X * 1.1f) + halfButtonWidth, textureData.Height * buttonScale.Y);
                Vector2 backingPosition = position + new Vector2(halfButtonWidth, 0.0f);
                renderer.Draw(blankGamepadTextureData, backingPosition + new Vector2(backingSize.X - halfButtonWidth, 0.0f), Color.Gray, 0.0f, buttonScale, false);
                renderer.Draw(whitePixelTextureData, backingPosition, Color.Gray, 0.0f, backingSize, false);
                Vector2 textPosition = backingPosition + new Vector2(halfButtonWidth, 0.0f) + ((backingSize - new Vector2(halfButtonWidth / 2.0f, 0.0f) - textSize) / 2.0f);
                renderer.DrawText(text, textPosition, Color.White, 0.0f, Vector2.Zero, minScale);
            }

            renderer.Draw(textureData, position, Color.White, 0.0f, buttonScale, false);
            return textureData.Size * buttonScale;
        }

        protected override void update(Delta delta)
        {
            if (!CurrentBattleState.KeepPartyMembersStatic)
                delta.scaleDelta(MathHelper.Lerp(1.0f, 4.0f, Input.LeftTriggerAmount()));

            if (stateChanged)
            {
                stateChanged = false;
                CurrentBattleState.Start();
                Logger.Log(CurrentBattleState.GetType().Name + " battle state started");
            }

            CurrentBattleState.Update(delta);

            if (CurrentBattleState.KeepPartyMembersStatic)
                repositionPartyMembers();

            base.update(delta);

            foreach (PartyMember partyMember in PlayerParty)
                partyMember.Update(delta);
            foreach (PartyMember partyMember in EnemyParty)
                partyMember.Update(delta);

            if (CurrentBattleState.BattleStateRenderer != null)
                CurrentBattleState.BattleStateRenderer.Update(delta);

            if (cameraUpdateDelay > 0.0f)
            {
                cameraUpdateDelay -= delta.Time;
                Camera.Update(delta);
            }
            else
                updateCamera(() => Camera.Update(delta));
        }

        protected override void draw(Renderer renderer)
        {
            Game1.BackgroundColor = Game1.GrassColor;
            renderer.Camera = Camera;
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
                Camera.TargetScale = new Vector2(scaleX);
            else
                Camera.TargetScale = new Vector2(scaleY);

            if (midUpdateAction != null)
                midUpdateAction();

            float averageX = (lowestX + highestX) / 2.0f;
            float averageY = (lowestY + highestY) / 2.0f;
            Camera.TargetPosition = new Vector2(averageX, averageY);
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
                    list[i].BattleEntityIdlePosition = list[i].BattleEntity.Position = new Vector2(position.X - (200.0f * i), position.Y + i);
                position.X -= 150.0f;
                position.Y += 150.0f;
            });

            position = new Vector2(1200.0f, 400.0f);
            EnemyPartyLayout.ForEachList(list =>
            {
                for (int i = 0; i < list.Count; ++i)
                    list[i].BattleEntityIdlePosition = list[i].BattleEntity.Position = new Vector2(position.X + (200.0f * i), position.Y + i);
                position.X += 150.0f;
                position.Y += 150.0f;
            });
        }

        private void drawBackground(Renderer renderer)
        {
            const float scale = 0.5f;
            Rectangle cameraBoundingBox = Camera.GetBoundingBox();

            float textureWidth = lowestBackgroundTextureWidth * scale * Camera.Scale.X;
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
            Vector2 screenScaleFactor = new Vector2(MathHelper.Min(Game1.ScreenScaleFactor.X, Game1.ScreenScaleFactor.Y));
            Vector2 headPadding = new Vector2(30.0f) * screenScaleFactor;
            Vector2 barPadding = new Vector2(20.0f) * screenScaleFactor;
            Vector2 barSize = new Vector2(290, 30) * screenScaleFactor;

            Vector2 uiPosition = new Vector2(0.0f, Game1.ScreenSize.Y - (uiHeight * screenScaleFactor.Y));
            Vector2 uiSize = new Vector2(Game1.ScreenSize.X, uiHeight * screenScaleFactor.Y);
            renderer.Draw(whitePixelTextureData, uiPosition, UiColor, 0.0f, uiSize, false);
            drawGuiBorder(uiPosition, uiSize, renderer);
            
            Vector2 position = new Vector2(headPadding.X, uiPosition.Y + headPadding.Y);
            for (int i = 0; i < PlayerParty.Count; ++i)
            {
                drawPartyMemberUi(PlayerParty[i], position, headPadding, barPadding, barSize, renderer);
                position.X += (headPadding.X * 2) + barPadding.X + barSize.X + (CharacterClassHeadTextureData[PlayerParty[i].CharacterClass].Width * screenScaleFactor.X);
            }
        }

        private void drawGuiBorder(Vector2 uiPosition, Vector2 uiSize, Renderer renderer)
        {
            Vector2 screenScaleFactor = Game1.ScreenScaleFactor;
            float borderWidth = uiSize.X - (borderTextureDataE.Width * 2 * screenScaleFactor.X);
            Vector2 borderScaleX = new Vector2((1.0f / borderTextureDataN.Width) * borderWidth, screenScaleFactor.Y);
            Vector2 borderScaleY = new Vector2(screenScaleFactor.X, (1.0f / borderTextureDataW.Height) * uiSize.Y);
            Vector2 position = uiPosition - new Vector2(0.0f, borderTextureDataN.Height * screenScaleFactor.Y);
            renderer.Draw(borderTextureDataNW, position, Color.White, 0.0f, screenScaleFactor, false);
            renderer.Draw(borderTextureDataW, uiPosition, Color.White, 0.0f, borderScaleY, false);
            position.X += borderTextureDataNW.Width * screenScaleFactor.X;
            renderer.Draw(borderTextureDataN, position, Color.White, 0.0f, borderScaleX, false);
            position.X += borderWidth;
            renderer.Draw(borderTextureDataNE, position, Color.White, 0.0f, screenScaleFactor, false);
            position.Y = uiPosition.Y;
            renderer.Draw(borderTextureDataE, position, Color.White, 0.0f, borderScaleY, false);
        }

        private void drawPartyMemberUi(PartyMember partyMember, Vector2 position, Vector2 headPadding, Vector2 barPadding, Vector2 barSize, Renderer renderer)
        {
            Vector2 screenScaleFactor = Game1.ScreenScaleFactor;
            Vector2 minScale = new Vector2(MathHelper.Min(screenScaleFactor.X, screenScaleFactor.Y));
            Vector2 shadowOffset = new Vector2(-2f, 2f) * minScale;
            Color shadowColor = Color.Black * 0.4f;

            TextureData textureData = CharacterClassHeadTextureData[partyMember.CharacterClass];
            bool partyMemberThinking = partyMemberIsThinking(partyMember);

            if (partyMemberThinking)
            {
                renderer.Draw(textureData, position + (shadowOffset * 2.0f), shadowColor, 0.0f, minScale, false);
            }
            renderer.Draw(textureData, position, Color.White, 0.0f, minScale, false);
            
            if (partyMemberIsSelected(partyMember))
                renderer.Draw(arrowTextureData, position + new Vector2((textureData.Width / 2) - (arrowTextureData.Width / 2), -arrowTextureData.Height), Color.White, 0.0f, minScale, false);

            position.X += (textureData.Width * screenScaleFactor.X) + headPadding.X;

            Vector2 partyMemberNameSize = renderer.Font.MeasureString(partyMember.Name, Font.DefaultSize * minScale.Y);
            Vector2 partyMemberNamePosition = position + new Vector2(barSize.X / 2.0f, 20.0f * minScale.Y);
            if (partyMemberThinking)
                renderer.DrawText(partyMember.Name, partyMemberNamePosition + shadowOffset, shadowColor, 0.0f, partyMemberNameSize / 2.0f, minScale);
            renderer.DrawText(partyMember.Name, partyMemberNamePosition, Color.White, 0.0f, partyMemberNameSize / 2.0f, minScale);

            position.Y += partyMemberNameSize.Y + (20.0f * minScale.Y);
            float percentageHealth = partyMember.Health / (float)partyMember.MaxHealth;
            Color healthBarColor = percentageHealth > 0.5f ? Color.Lerp(Color.Yellow, Color.Green, (percentageHealth - 0.5f) / 0.5f) : Color.Lerp(Color.Red, Color.Yellow, percentageHealth / 0.5f);
            if (partyMemberThinking)
                drawBar(position + shadowOffset, barSize, 1.0f, shadowColor, false, renderer);
            drawBar(position, barSize, percentageHealth, healthBarColor, true, renderer);
            string barText = "HP: " + partyMember.Health.ToString() + "/" + partyMember.MaxHealth;
            Vector2 barTextSize = renderer.Font.MeasureString(barText, Font.DefaultSize * minScale.Y);
            renderer.DrawText(barText, position + (barSize / 2.0f), Color.White, 0.0f, barTextSize / 2.0f, minScale);

            position.Y += barSize.Y * 1.5f;
            partyMember.ForEachStatusEffect((statusEffect) => {
                float widthWithPadding = barSize.Y * 1.1f;
                Vector2 scale = new Vector2((1.0f / statusEffect.TextureData.Height) * barSize.Y);
                Vector2 nudge = new Vector2((widthWithPadding - (statusEffect.TextureData.Width * scale.X)) / 2.0f, 0.0f);
                renderer.Draw(statusEffect.TextureData, position + (statusEffect.TextureData.Origin * scale) + nudge, Color.White, 0.0f, scale, false);
                position.X += barSize.Y * 1.1f;
            });
        }

        private void drawBar(Vector2 position, Vector2 size, float percentage, Color color, bool drawBacking, Renderer renderer)
        {
            if (drawBacking)
                renderer.Draw(whitePixelTextureData, position, Color.Black, 0.0f, size, false);
            renderer.Draw(whitePixelTextureData, position, color, 0.0f, new Vector2(size.X * percentage, size.Y), false);
        }

        private bool partyMemberIsThinking(PartyMember partyMember)
        {
            BattleStates.Think thinkState = CurrentBattleState as BattleStates.Think;
            if (thinkState != null)
                return thinkState.CurrentPartyMember == partyMember;

            BattleStates.SelectTarget selectTargetState = CurrentBattleState as BattleStates.SelectTarget;
            if (selectTargetState != null)
                return selectTargetState.Actor == partyMember;

            BattleStates.MoveActor moveActorState = CurrentBattleState as BattleStates.MoveActor;
            if (moveActorState != null)
                return moveActorState.Actor == partyMember;

            return false;
        }

        private bool partyMemberIsSelected(PartyMember partyMember)
        {
            BattleStates.SelectTarget selectTargetState = CurrentBattleState as BattleStates.SelectTarget;
            if (selectTargetState != null)
                return selectTargetState.PotentialTarget == partyMember;

            return false;
        }
        #endregion
    }
}
