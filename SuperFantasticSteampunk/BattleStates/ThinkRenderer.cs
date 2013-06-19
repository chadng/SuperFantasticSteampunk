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
        private TextureData iconTextureData;
        private Color color;
        #endregion

        #region Instance Properties
        public Vector2 Position { get; set; }
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
            Vector2 position = positionFromScale(iconContainerTextureData, containerScale, scaleScale);
            containerScale *= scaleScale;
            iconScale *= scaleScale;
            Vector2 iconPosition = position + (new Vector2(iconContainerTextureData.Width - iconTextureData.Width, iconContainerTextureData.Height - iconTextureData.Height) * scaleScale * 0.5f * Game1.ScreenScaleFactor.X);
            Color containerColor = color;
            Color iconColor = Color.White;
            if (iconContainerGlowTextureData == null)
            {
                containerColor = new Color(color.ToVector3() * 0.7f);
                iconColor = new Color(Color.White.ToVector3() * 0.7f);
            }
            else
                renderer.Draw(iconContainerGlowTextureData, position - (new Vector2(iconContainerGlowTextureData.Width - iconContainerTextureData.Width) * 0.5f * containerScale.X), Color.WhiteSmoke, 0.0f, containerScale, false);
            renderer.Draw(iconContainerTextureData, position, containerColor, 0.0f, containerScale, false);
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

        private const float subMenuX = 1000.0f;
        private const float subMenuY = 200.0f;
        private const float subMenuWidth = 200.0f;
        private const float subMenuHeight = 500.0f;

        private const int N = 0;
        private const int NE = 1;
        private const int E = 2;
        private const int SE = 3;
        private const int S = 4;
        private const int SW = 5;
        private const int W = 6;
        private const int NW = 7;

        private static readonly SortedDictionary<string, Color> menuOptionColors = new SortedDictionary<string, Color> {
            { "move", Color.Blue },
            { "attack", Color.Red },
            { "defend", Color.Green },
            { "item", Color.Yellow },
            { "run", Color.Pink }
        };

        private static readonly string[] directions = new string[] { "n", "ne", "e", "se", "s", "sw", "w", "nw" };
        #endregion

        #region Instance Fields
        private TextureData iconContainerTextureData;
        private TextureData iconContainerGlowTextureData;
        private float outerMenuOptionTransitionAngle;
        private float outerMenuOptionTransitionYScale;
        private bool incOuterMenuOptionYScale;
        private List<ThinkRendererOuterMenuOption> menuOptions;
        private List<ThinkRendererOuterMenuOption> sortedMenuOptions;

        private readonly float anglePerOption;
        private readonly int halfOptionsLength;
        private readonly TextureData whitePixelTextureData;
        private readonly TextureData[] borderTextureData;
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
                ThinkRendererOuterMenuOption menuOption = new ThinkRendererOuterMenuOption(ResourceManager.GetTextureData("battle_ui/" + optionName + "_icon"), menuOptionColors[optionName], i);
                menuOptions.Add(menuOption);
                sortedMenuOptions.Add(menuOption);
            }
            iconContainerTextureData = ResourceManager.GetTextureData("battle_ui/icon_container");
            iconContainerGlowTextureData = ResourceManager.GetTextureData("battle_ui/icon_container_glow");
            ResetOuterMenuTransitions();

            anglePerOption = MathHelper.TwoPi / Think.OuterMenuOptions.Length;
            halfOptionsLength = Think.OuterMenuOptions.Length / 2;

            whitePixelTextureData = ResourceManager.GetTextureData("white_pixel");
            borderTextureData = new TextureData[directions.Length];
            for (int i = 0; i < directions.Length; ++i)
                borderTextureData[i] = ResourceManager.GetTextureData("battle_ui/borders/" + directions[i]);
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
                    outerMenuOptionTransitionYScale = 0.0f;
            }
        }

        public override void BeforeDraw(Renderer renderer)
        {
            // Draw elements under party members
        }

        public override void AfterDraw(Renderer renderer)
        {
            drawOuterMenu(renderer);
            if (battleState.CurrentThinkActionType != ThinkActionType.None)
                drawOptionNamesSubMenu(renderer);
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
                if (menuOption.MenuOptionIndex == battleState.CurrentOuterMenuOptionIndex)
                {
                    glowTextureData = iconContainerGlowTextureData;
                    thisScaleScale = Vector2.One;
                }
                menuOption.Draw(iconContainerTextureData, glowTextureData, containerScale, iconScale, thisScaleScale, renderer);
            }
        }

        private void drawOptionNamesSubMenu(Renderer renderer)
        {
            drawContainer(subMenuX, subMenuY, subMenuWidth, subMenuHeight, renderer);
            drawOptionNamesText(renderer);
        }

        private void drawOptionNamesText(Renderer renderer)
        {
            const float subMenuPadding = 20.0f;
            const float fontSize = 14.0f;
            float fontHeight = renderer.Font.MeasureString("I", fontSize).Y;
            Vector2 scale = Game1.ScreenScaleFactor;
            Vector2 fontScale = new Vector2(fontSize / Font.DefaultSize) * scale.X;
            Vector2 position = new Vector2(subMenuX + subMenuPadding, subMenuY + subMenuPadding);
            for (int i = 0; i < battleState.MenuOptions.Count; ++i)
            {
                ThinkMenuOption menuOption = battleState.MenuOptions[i];

                if (i == battleState.CurrentOptionNameIndex)
                {
                    renderer.DrawText(">", (position - new Vector2(subMenuPadding, 0.0f)) * scale, Color.White, 0.0f, Vector2.Zero, fontScale);
                    Vector2 containerSize = renderer.Font.MeasureString(menuOption.Description, fontSize);
                    drawContainer((position.X + subMenuWidth), position.Y, containerSize.X, (containerSize.Y / scale.Y) * scale.X, renderer);
                    renderer.DrawText(menuOption.Description, (position + new Vector2(subMenuWidth, 0.0f)) * scale, Color.White, 0.0f, Vector2.Zero, fontScale);
                }

                renderer.DrawText(menuOption.Name, position * scale, Color.White, 0.0f, Vector2.Zero, fontScale);

                string amountString = menuOption.Amount < 0 ? "~" : menuOption.Amount.ToString();
                Vector2 amountSize = renderer.Font.MeasureString(amountString, fontSize);
                renderer.DrawText(amountString, (position + new Vector2(subMenuWidth - (subMenuPadding * 2) - amountSize.X, 0.0f)) * scale, Color.White, 0.0f, Vector2.Zero, fontScale);

                position.Y += fontHeight * 1.1f;
            }
        }

        private void drawContainer(float x, float y, float width, float height, Renderer renderer)
        {
            Vector2 scale = Game1.ScreenScaleFactor;
            renderer.Draw(whitePixelTextureData, new Vector2(x, y) * scale, new Color(124, 63, 18), 0.0f, new Vector2(width, height) * scale, false);

            TextureData textureData = borderTextureData[NW];
            renderer.Draw(textureData, new Vector2(x - textureData.Width, y - textureData.Height) * scale, Color.White, 0.0f, scale, false);
            textureData = borderTextureData[NE];
            renderer.Draw(textureData, new Vector2(x + width, y - textureData.Height) * scale, Color.White, 0.0f, scale, false);
            textureData = borderTextureData[SE];
            renderer.Draw(textureData, new Vector2(x + width, y + height) * scale, Color.White, 0.0f, scale, false);
            textureData = borderTextureData[SW];
            renderer.Draw(textureData, new Vector2(x - textureData.Width, y + height) * scale, Color.White, 0.0f, scale, false);

            textureData = borderTextureData[N];
            Vector2 xScale = new Vector2((1.0f / textureData.Width) * width, 1.0f) * scale;
            renderer.Draw(textureData, new Vector2(x, y - textureData.Height) * scale, Color.White, 0.0f, xScale, false);
            textureData = borderTextureData[S];
            renderer.Draw(textureData, new Vector2(x, y + height) * scale, Color.White, 0.0f, xScale, false);

            textureData = borderTextureData[W];
            Vector2 yScale = new Vector2(1.0f, (1.0f / textureData.Height) * height) * scale;
            renderer.Draw(textureData, new Vector2(x - textureData.Width, y) * scale, Color.White, 0.0f, yScale, false);
            textureData = borderTextureData[E];
            renderer.Draw(textureData, new Vector2(x + width, y) * scale, Color.White, 0.0f, yScale, false);
        }
        #endregion
    }
}
