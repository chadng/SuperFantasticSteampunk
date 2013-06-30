using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;

namespace SuperFantasticSteampunk.BattleStates
{
    class ThinkRendererOuterMenuOption
    {
        #region Instance Fields
        private readonly TextureData iconTextureData;
        private readonly Color color;
        #endregion

        #region Instance Properties
        public Vector2 Position { get; set; }
        public Vector2 ScaledPosition { get; private set; }
        public int MenuOptionIndex { get; private set; }
        #endregion

        #region Constructors
        public ThinkRendererOuterMenuOption(TextureData iconTextureData, Color color, int menuOptionIndex)
        {
            this.iconTextureData = iconTextureData;
            this.color = color;
            MenuOptionIndex = menuOptionIndex;
        }
        #endregion

        #region Instance Methods
        public void Draw(TextureData iconContainerTextureData, TextureData iconContainerGlowTextureData, Vector2 containerScale, Vector2 iconScale, Vector2 scaleScale, Renderer renderer)
        {
            ScaledPosition = positionFromScale(iconContainerTextureData, containerScale, scaleScale);
            containerScale *= scaleScale;
            iconScale *= scaleScale;
            Vector2 iconPosition = ScaledPosition + (new Vector2(iconContainerTextureData.Width - iconTextureData.Width, iconContainerTextureData.Height - iconTextureData.Height) * scaleScale * 0.5f * Game1.ScreenScaleFactor.X);
            Color containerColor = color;
            Color iconColor = Color.White;
            if (iconContainerGlowTextureData == null)
            {
                containerColor = new Color(color.ToVector3() * 0.7f);
                iconColor = new Color(Color.White.ToVector3() * 0.7f);
            }
            else
                renderer.Draw(iconContainerGlowTextureData, ScaledPosition - (new Vector2(iconContainerGlowTextureData.Width - iconContainerTextureData.Width) * 0.5f * containerScale.X), Color.WhiteSmoke, 0.0f, containerScale, false);
            renderer.Draw(iconContainerTextureData, ScaledPosition, containerColor, 0.0f, containerScale, false);
            renderer.Draw(iconTextureData, iconPosition, iconColor, 0.0f, iconScale, false);
        }

        private Vector2 positionFromScale(TextureData containerTextureData, Vector2 containerScale, Vector2 scaleScale)
        {
            Vector2 originalDimensions = new Vector2(containerTextureData.Width, containerTextureData.Height) * containerScale;
            Vector2 scaledDimensions = originalDimensions * scaleScale;
            Vector2 difference = originalDimensions - scaledDimensions;
            return Position + (difference / 2.0f);
        }
        #endregion
    }

    class ThinkRenderer : BattleStateRenderer
    {
        #region Constants
        private const float outerMenuOptionAngleTransitionTime = 0.2f;
        private const float outerMenuOptionYScaleTransitionTime = 0.12f;
        private const float arrowMoveTime = 0.5f;

        private const float subMenuX = 800.0f;
        private const float subMenuY = 200.0f;
        private const float subMenuWidth = 500.0f;
        private const int maxVisibleSubMenuOptions = 10;
        private const float subMenuFontSize = 14.0f;
        private const float subMenuPadding = 20.0f;
        private const float fontHeightScale = 1.1f;

        private static readonly SortedDictionary<string, Color> menuOptionColors = new SortedDictionary<string, Color> {
            { "move", Color.Blue },
            { "attack", Color.Red },
            { "defend", Color.Green },
            { "item", Color.Yellow },
            { "run", Color.Pink }
        };

        private const int MELEE = 0;
        private const int RANGED = 1;
        private const int DEFEND = 2;
        private const int ITEM = 3;
        private static readonly string[] actionIconNames = { "attack", "ranged", "defend", "item" };
        #endregion

        #region Instance Fields
        private TextureData iconContainerTextureData;
        private TextureData iconContainerGlowTextureData;
        private TextureData[] actionIconTextureData;
        private float outerMenuOptionTransitionAngle;
        private float outerMenuOptionTransitionYScale;
        private bool incOuterMenuOptionYScale;
        private List<ThinkRendererOuterMenuOption> menuOptions;
        private List<ThinkRendererOuterMenuOption> sortedMenuOptions;
        private float arrowMoveTimer;

        private readonly float anglePerOption;
        private readonly int halfOptionsLength;
        private readonly TextureData whitePixelTextureData;
        #endregion

        #region Instance Properties
        public bool IsTransitioningMenu
        {
            get { return outerMenuOptionTransitionAngle != 0.0f || outerMenuOptionTransitionYScale != 1.0f; }
        }

        protected new Think battleState
        {
            get { return base.battleState as Think; }
        }

        private TextureData[] borderTextureData
        {
            get { return battleState.Battle.BorderTextureData; }
        }
        #endregion

        #region Constructors
        public ThinkRenderer(BattleState battleState)
            : base(battleState)
        {
            menuOptions = new List<ThinkRendererOuterMenuOption>(Think.OuterMenuOptions.Length);
            sortedMenuOptions = new List<ThinkRendererOuterMenuOption>(Think.OuterMenuOptions.Length);
            for (int i = 0; i < Think.OuterMenuOptions.Length; ++i)
            {
                string optionName = Think.OuterMenuOptions[i].ToLower();
                ThinkRendererOuterMenuOption menuOption = new ThinkRendererOuterMenuOption(ResourceManager.GetTextureData("battle_ui/icons/" + optionName), menuOptionColors[optionName], i);
                menuOptions.Add(menuOption);
                sortedMenuOptions.Add(menuOption);
            }
            iconContainerTextureData = ResourceManager.GetTextureData("battle_ui/icons/container");
            iconContainerGlowTextureData = ResourceManager.GetTextureData("battle_ui/icons/container_glow");
            actionIconTextureData = new TextureData[actionIconNames.Length];
            for (int i = 0; i < actionIconNames.Length; ++i)
                actionIconTextureData[i] = ResourceManager.GetTextureData("battle_ui/icons/" + actionIconNames[i]);
            ResetOuterMenuTransitions();

            anglePerOption = MathHelper.TwoPi / Think.OuterMenuOptions.Length;
            halfOptionsLength = Think.OuterMenuOptions.Length / 2;

            whitePixelTextureData = ResourceManager.GetTextureData("white_pixel");
        }
        #endregion

        #region Instance Methods
        public void StartOuterMenuOptionTransition(int relativeOptionIndex)
        {
            outerMenuOptionTransitionAngle = anglePerOption * (relativeOptionIndex > 0 ? 1.0f : -1.0f);
        }

        public void StartOuterMenuOptionSelectTransition()
        {
            incOuterMenuOptionYScale = false;
        }

        public void StartOuterMenuOptionDeselectTransition()
        {
            incOuterMenuOptionYScale = true;
        }

        public void ResetOuterMenuTransitions()
        {
            outerMenuOptionTransitionAngle = 0.0f;
            outerMenuOptionTransitionYScale = 1.0f;
            incOuterMenuOptionYScale = true;
            arrowMoveTimer = 0.0f;
        }

        public override void Update(Delta delta)
        {
            if (outerMenuOptionTransitionAngle != 0.0f)
            {
                bool inc = outerMenuOptionTransitionAngle < 0.0f;
                outerMenuOptionTransitionAngle += (anglePerOption / outerMenuOptionAngleTransitionTime) * delta.Time * (inc ? 1.0f : -1.0f);
                if ((inc && outerMenuOptionTransitionAngle > 0.0f) || (!inc && outerMenuOptionTransitionAngle < 0.0f))
                    outerMenuOptionTransitionAngle = 0.0f;
            }
            else if (incOuterMenuOptionYScale)
            {
                if (outerMenuOptionTransitionYScale < 1.0f)
                {
                    outerMenuOptionTransitionYScale += delta.Time / outerMenuOptionYScaleTransitionTime;
                    if (outerMenuOptionTransitionYScale > 1.0f)
                        outerMenuOptionTransitionYScale = 1.0f;
                }
            }
            else if (outerMenuOptionTransitionYScale > 0.0f)
            {
                outerMenuOptionTransitionYScale -= delta.Time / outerMenuOptionYScaleTransitionTime;
                if (outerMenuOptionTransitionYScale < 0.0f)
                {
                    outerMenuOptionTransitionYScale = 0.0f;
                }
            }

            if (battleState.CurrentThinkActionType == ThinkActionType.None)
                arrowMoveTimer = 0.0f;
            else
            {
                arrowMoveTimer += delta.Time;
                if (arrowMoveTimer >= arrowMoveTime * 2.0f)
                    arrowMoveTimer = 0.0f;
            }
        }

        public override void BeforeDraw(Renderer renderer)
        {
        }

        public override void AfterDraw(Renderer renderer)
        {
            drawPartyMemberActionIndicators(renderer);
            if (battleState.CurrentThinkActionType == ThinkActionType.None)
            {
                drawTextOverPreviouslyActedPartyMember(renderer);
                drawOuterMenu(renderer);
            }
            else
            {
                drawOuterMenu(renderer);
                drawOptionNamesSubMenu(renderer);
            }
        }

        private void drawPartyMemberActionIndicators(Renderer renderer)
        {
            Vector2 scale = 0.5f * Game1.MinScreenScaleFactor;
            foreach (ThinkAction action in battleState.Actions)
            {
                TextureData iconTextureData;
                switch (action.Type)
                {
                case ThinkActionType.Attack: iconTextureData = actionIconTextureData[action.Melee ? MELEE : RANGED]; break;
                case ThinkActionType.Defend: iconTextureData = actionIconTextureData[DEFEND]; break;
                case ThinkActionType.UseItem: iconTextureData = actionIconTextureData[ITEM]; break;
                default: iconTextureData = null; break;
                }

                if (iconTextureData == null)
                    continue;

                Rectangle boundingBox = action.Actor.BattleEntity.GetBoundingBox();
                Vector2 position = new Vector2(boundingBox.X + (boundingBox.Width / 2.0f) - (iconTextureData.Width * scale.X * 0.5f), boundingBox.Y - (iconTextureData.Height * scale.Y));
                position = battleState.Battle.Camera.TranslateVector(position);
                renderer.Draw(iconTextureData, position, Color.White, 0.0f, scale, false);
            }
        }

        private void drawOuterMenu(Renderer renderer)
        {
            double currentAngle = MathHelper.PiOver2 + (anglePerOption * halfOptionsLength);
            int startIndex = battleState.CurrentOuterMenuOptionIndex + halfOptionsLength;
            while (startIndex >= Think.OuterMenuOptions.Length)
                startIndex -= Think.OuterMenuOptions.Length;

            Vector2 containerScale = new Vector2(Game1.ScreenScaleFactor.X * 0.6f);
            Vector2 iconScale = new Vector2(Game1.ScreenScaleFactor.X * 0.5f);

            Vector2 startPosition = new Vector2(battleState.CurrentPartyMember.BattleEntity.GetCenter().X, battleState.CurrentPartyMember.BattleEntity.GetBoundingBox().Y);
            startPosition -= new Vector2(iconContainerTextureData.Width * containerScale.X * 0.5f, 100.0f * Game1.ScreenScaleFactor.X);
            startPosition = battleState.Battle.Camera.TranslateVector(startPosition);

            for (int i = startIndex, counter = 0; counter < Think.OuterMenuOptions.Length; ++counter, currentAngle += anglePerOption)
            {
                Vector2 ovalPosition = new Vector2((float)Math.Cos(currentAngle + outerMenuOptionTransitionAngle), (float)Math.Sin(currentAngle + outerMenuOptionTransitionAngle) / 2.0f);
                menuOptions[i].Position = startPosition + (ovalPosition * 75.0f * Game1.ScreenScaleFactor.X);
                if (++i >= Think.OuterMenuOptions.Length)
                    i = 0;
            }

            sortedMenuOptions.Sort((a, b) => a.Position.Y.CompareTo(b.Position.Y));
            Vector2 scaleScale = new Vector2(1.0f, outerMenuOptionTransitionYScale);
            foreach (ThinkRendererOuterMenuOption menuOption in sortedMenuOptions)
            {
                TextureData glowTextureData = null;
                Vector2 thisScaleScale = scaleScale;
                InputButton buttonType = InputButton.A;
                string buttonText = null;
                bool isSelected = menuOption.MenuOptionIndex == battleState.CurrentOuterMenuOptionIndex;
                if (isSelected)
                {
                    glowTextureData = iconContainerGlowTextureData;
                    thisScaleScale = Vector2.One;
                    if (battleState.CurrentThinkActionType != ThinkActionType.None || battleState.Paused)
                    {
                        buttonType = InputButton.B;
                        buttonText = battleState.Paused ? "Cancel" : "Back";
                    }
                    else
                        buttonText = Think.OuterMenuOptions[menuOption.MenuOptionIndex];
                }
                if (thisScaleScale.X != 0.0f && thisScaleScale.Y != 0.0f)
                    menuOption.Draw(iconContainerTextureData, glowTextureData, containerScale, iconScale, thisScaleScale, renderer);
                if (isSelected && outerMenuOptionTransitionAngle == 0.0f)
                {
                    Vector2 buttonPosition = menuOption.ScaledPosition + (iconContainerTextureData.Size * 0.8f * containerScale);
                    if (battleState.Paused)
                    {
                        Vector2 buttonSize = battleState.Battle.DrawButtonWithText(InputButton.A, "Confirm", buttonPosition, renderer);
                        buttonPosition.Y += buttonSize.Y * 1.1f;
                    }
                    battleState.Battle.DrawButtonWithText(buttonType, buttonText, buttonPosition, renderer);
                }
            }
        }

        private void drawTextOverPreviouslyActedPartyMember(Renderer renderer)
        {
            if (battleState.PreviouslyActedPartyMember == null || battleState.CurrentPartyMember == null || battleState.Paused)
                return;
            Rectangle boundingBox = battleState.PreviouslyActedPartyMember.BattleEntity.GetBoundingBox();
            Vector2 position = new Vector2(boundingBox.X + (boundingBox.Width / 2.0f), boundingBox.Bottom);
            position = battleState.Battle.Camera.TranslateVector(position);
            boundingBox = battleState.CurrentPartyMember.BattleEntity.GetBoundingBox();
            Vector2 currentPartyMemberPosition = new Vector2(boundingBox.X + (boundingBox.Width / 2.0f), boundingBox.Bottom);
            bool flip = currentPartyMemberPosition.X > position.X && currentPartyMemberPosition.Y > position.Y;
            battleState.Battle.DrawButtonWithText(InputButton.B, "Back to " + battleState.PreviouslyActedPartyMember.Name, position, renderer, flip);
        }

        private void drawOptionNamesSubMenu(Renderer renderer)
        {
            float subMenuHeight = getSubMenuHeight(Math.Min(battleState.MenuOptions.Count, maxVisibleSubMenuOptions), renderer);
            drawContainer(subMenuX, subMenuY, subMenuWidth, subMenuHeight, renderer);
            drawOptionNamesText(renderer);
        }

        private float getSubMenuHeight(int optionCount, Renderer renderer)
        {
            float fontHeight = renderer.Font.MeasureString("I", subMenuFontSize).Y;
            return fontHeight * fontHeightScale * optionCount;
        }

        private void drawOptionNamesText(Renderer renderer)
        {
            float fontHeight = renderer.Font.MeasureString("I", subMenuFontSize).Y;
            Vector2 scale = Game1.ScreenScaleFactor;
            Vector2 fontScale = new Vector2(subMenuFontSize / Font.DefaultSize) * Game1.ScreenScaleFactor.X;
            Vector2 position = new Vector2(subMenuX + subMenuPadding, subMenuY);
            float measureFontSize = Font.DefaultSize * fontScale.Y;
            Vector2 upArrowPosition = (position * scale) + new Vector2(subMenuWidth * Game1.ScreenScaleFactor.X * 0.5f, 0.0f);

            int startIndex;
            int finishIndex;
            battleState.Battle.CalculateStartAndFinishIndexesForMenuList(battleState.MenuOptions.Count, maxVisibleSubMenuOptions, battleState.CurrentOptionNameIndex, out startIndex, out finishIndex);

            for (int i = startIndex; i <= finishIndex; ++i)
            {
                ThinkMenuOption menuOption = battleState.MenuOptions[i];

                if (i == battleState.CurrentOptionNameIndex)
                {
                    Vector2 arrowPosition = (position - new Vector2(subMenuPadding, 0.0f)) * scale;
                    renderer.DrawText(">", arrowPosition, Color.White, 0.0f, Vector2.Zero, fontScale);
                    Vector2 arrowSize = renderer.Font.MeasureString(">", measureFontSize);
                    Vector2 buttonSize = new Vector2(renderer.Font.MeasureString("I", measureFontSize).Y);
                    Vector2 buttonPosition = arrowPosition - new Vector2(buttonSize.X + (arrowSize.X * 0.2f), (buttonSize.Y - (arrowSize.Y * 0.8f)) / 2.0f);
                    battleState.Battle.DrawButtonWithText(InputButton.A, null, buttonPosition, renderer);
                    Vector2 containerSize = renderer.Font.MeasureString(menuOption.Description, measureFontSize) / scale;
                    Vector2 containerPosition = position + new Vector2(subMenuWidth + (borderTextureData[Battle.E].Width * 0.5f), 0.0f);
                    drawContainer(containerPosition.X, containerPosition.Y, containerSize.X, containerSize.Y, renderer);
                    renderer.DrawText(menuOption.Description, containerPosition * scale, Color.White, 0.0f, Vector2.Zero, fontScale);
                }

                renderer.DrawText(menuOption.Name, position * scale, Color.White, 0.0f, Vector2.Zero, fontScale);

                string amountString = menuOption.Amount < 0 ? "~" : menuOption.Amount.ToString();
                Vector2 amountSize = renderer.Font.MeasureString(amountString, subMenuFontSize);
                renderer.DrawText(amountString, (position + new Vector2(subMenuWidth - (subMenuPadding * 2) - amountSize.X, 0.0f)) * scale, Color.White, 0.0f, Vector2.Zero, fontScale);

                position.Y += fontHeight * fontHeightScale;
            }

            float arrowFontSize = measureFontSize * 1.5f;
            Vector2 arrowFontScale = fontScale * 1.5f;
            Vector2 scrollArrowSize = renderer.Font.MeasureString("^", arrowFontSize);
            scrollArrowSize = new Vector2(scrollArrowSize.Y, scrollArrowSize.X);
            float yOffset = 10.0f * scale.Y;
            if (arrowMoveTimer <= arrowMoveTime)
                yOffset *= arrowMoveTimer / arrowMoveTime;
            else
                yOffset *= 1.0f - ((arrowMoveTimer - arrowMoveTime) / arrowMoveTime);
            if (startIndex > 0)
                renderer.DrawText("^", new Vector2(upArrowPosition.X, upArrowPosition.Y - (scrollArrowSize.Y / 2.0f) - yOffset), Color.White, 0.0f, scrollArrowSize / 2.0f, arrowFontScale);
            if (finishIndex < battleState.MenuOptions.Count - 1)
                renderer.DrawText("^", new Vector2(upArrowPosition.X, (position.Y * scale.Y) + (scrollArrowSize.Y / 2.0f) + yOffset), Color.White, 0.0f, scrollArrowSize / 2.0f, new Vector2(arrowFontScale.X, -arrowFontScale.Y));
        }

        private void drawContainer(float x, float y, float width, float height, Renderer renderer)
        {
            Vector2 scale = Game1.ScreenScaleFactor;
            renderer.Draw(whitePixelTextureData, new Vector2(x, y) * scale, Battle.UiColor, 0.0f, new Vector2(width, height) * scale, false);

            Vector2 halfScale = scale * 0.5f;
            TextureData textureData = borderTextureData[Battle.NW];
            renderer.Draw(textureData, new Vector2(x - (textureData.Width * 0.5f), y - (textureData.Height * 0.5f)) * scale, Color.White, 0.0f, halfScale, false);
            textureData = borderTextureData[Battle.NE];
            renderer.Draw(textureData, new Vector2(x + width, y - (textureData.Height * 0.5f)) * scale, Color.White, 0.0f, halfScale, false);
            textureData = borderTextureData[Battle.SE];
            renderer.Draw(textureData, new Vector2(x + width, y + height) * scale, Color.White, 0.0f, halfScale, false);
            textureData = borderTextureData[Battle.SW];
            renderer.Draw(textureData, new Vector2(x - (textureData.Width * 0.5f), y + height) * scale, Color.White, 0.0f, halfScale, false);

            textureData = borderTextureData[Battle.N];
            Vector2 xScale = new Vector2((1.0f / textureData.Width) * width, 0.5f) * scale;
            renderer.Draw(textureData, new Vector2(x, y - (textureData.Height * 0.5f)) * scale, Color.White, 0.0f, xScale, false);
            textureData = borderTextureData[Battle.S];
            renderer.Draw(textureData, new Vector2(x, y + height) * scale, Color.White, 0.0f, xScale, false);

            textureData = borderTextureData[Battle.W];
            Vector2 yScale = new Vector2(0.5f, (1.0f / textureData.Height) * height) * scale;
            renderer.Draw(textureData, new Vector2(x - (textureData.Width * 0.5f), y) * scale, Color.White, 0.0f, yScale, false);
            textureData = borderTextureData[Battle.E];
            renderer.Draw(textureData, new Vector2(x + width, y) * scale, Color.White, 0.0f, yScale, false);
        }
        #endregion
    }
}
