using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;

namespace SuperFantasticSteampunk.BattleStates
{
    class ThinkRendererMenuOption
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
        public ThinkRendererMenuOption(TextureData iconTextureData, Color color, int menuOptionIndex)
        {
            this.iconTextureData = iconTextureData;
            this.color = color;
            MenuOptionIndex = menuOptionIndex;
        }
        #endregion

        #region Instance Methods
        public void Draw(TextureData iconContainerTextureData, TextureData iconContainerGlowTextureData, Vector2 containerScale, Vector2 iconScale, Renderer renderer)
        {
            Vector2 iconPosition = Position + (new Vector2(iconContainerTextureData.Width - iconTextureData.Width, iconContainerTextureData.Height - iconTextureData.Height) * 0.5f * Game1.ScreenScaleFactor.X);
            if (iconContainerGlowTextureData != null)
                renderer.Draw(iconContainerGlowTextureData, Position - (new Vector2(iconContainerGlowTextureData.Width - iconContainerTextureData.Width) * 0.5f * containerScale.X), Color.WhiteSmoke, 0.0f, containerScale, false);
            renderer.Draw(iconContainerTextureData, Position, color, 0.0f, containerScale, false);
            renderer.Draw(iconTextureData, iconPosition, Color.White, 0.0f, iconScale, false);
        }
        #endregion
    }

    class ThinkRenderer : BattleStateRenderer
    {
        #region Constants
        private const float menuTransitionTime = 0.2f;

        private static readonly SortedDictionary<string, Color> menuOptionColors = new SortedDictionary<string, Color> {
            { "move", Color.Blue },
            { "attack", Color.Red },
            { "defend", Color.Green },
            { "item", Color.Yellow },
            { "run", Color.Pink }
        };
        #endregion

        #region Instance Fields
        private TextureData iconContainerTextureData;
        private TextureData iconContainerGlowTextureData;
        private float menuTransitionAngle;
        private List<ThinkRendererMenuOption> menuOptions;
        private List<ThinkRendererMenuOption> sortedMenuOptions;

        private readonly float anglePerOption;
        private readonly int halfOptionsLength;
        #endregion

        #region Instance Properties
        public bool IsTransitioningMenu
        {
            get { return menuTransitionAngle != 0.0f; }
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
            menuOptions = new List<ThinkRendererMenuOption>(Think.OuterMenuOptions.Length);
            sortedMenuOptions = new List<ThinkRendererMenuOption>(Think.OuterMenuOptions.Length);
            for (int i = 0; i < Think.OuterMenuOptions.Length; ++i)
            {
                string optionName = Think.OuterMenuOptions[i].ToLower();
                ThinkRendererMenuOption menuOption = new ThinkRendererMenuOption(ResourceManager.GetTextureData("battle_ui/" + optionName + "_icon"), menuOptionColors[optionName], i);
                menuOptions.Add(menuOption);
                sortedMenuOptions.Add(menuOption);
            }
            iconContainerTextureData = ResourceManager.GetTextureData("battle_ui/icon_container");
            iconContainerGlowTextureData = ResourceManager.GetTextureData("battle_ui/icon_container_glow");
            menuTransitionAngle = 0.0f;

            anglePerOption = MathHelper.TwoPi / Think.OuterMenuOptions.Length;
            halfOptionsLength = Think.OuterMenuOptions.Length / 2;
        }
        #endregion

        #region Instance Methods
        public void StartMenuTransition(int relativeOptionIndex)
        {
            menuTransitionAngle = anglePerOption * (relativeOptionIndex > 0 ? 1.0f : -1.0f);
        }

        public override void Update(Delta delta)
        {
            if (menuTransitionAngle != 0.0f)
            {
                bool inc = menuTransitionAngle < 0.0f;
                menuTransitionAngle += (anglePerOption / menuTransitionTime) * delta.Time * (inc ? 1.0f : -1.0f);
                if ((inc && menuTransitionAngle > 0.0f) || (!inc && menuTransitionAngle < 0.0f))
                    menuTransitionAngle = 0.0f;
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
                Vector2 ovalPosition = new Vector2((float)Math.Cos(currentAngle + menuTransitionAngle), (float)Math.Sin(currentAngle + menuTransitionAngle) / 2.0f);
                menuOptions[i].Position = startPosition + (ovalPosition * 75.0f * Game1.ScreenScaleFactor.X);
                if (++i >= Think.OuterMenuOptions.Length)
                    i = 0;
            }

            sortedMenuOptions.Sort((a, b) => a.Position.Y.CompareTo(b.Position.Y));
            foreach (ThinkRendererMenuOption menuOption in sortedMenuOptions)
                menuOption.Draw(iconContainerTextureData, menuOption.MenuOptionIndex == battleState.CurrentOuterMenuOptionIndex ? iconContainerGlowTextureData : null, containerScale, iconScale, renderer);
        }

        private void drawOptionNamesSubMenu(Renderer renderer)
        {
            Vector2 position = new Vector2(200, 100);
            for (int i = 0; i < battleState.MenuOptions.Count; ++i)
            {
                ThinkMenuOption menuOption = battleState.MenuOptions[i];
                string text = menuOption.Name;
                if (i > 0)
                    text += " x " + (menuOption.Amount < 0 ? "*" : menuOption.Amount.ToString());
                Color color;
                if (menuOption.Disabled)
                    color = i == battleState.CurrentOptionNameIndex ? Color.DarkRed : Color.Gray;
                else
                    color = i == battleState.CurrentOptionNameIndex ? Color.Blue : Color.White;
                renderer.DrawText(text, position, color, 0.0f, Vector2.Zero, Vector2.One);
                position.Y += 20.0f;
            }
        }
        #endregion
    }
}
