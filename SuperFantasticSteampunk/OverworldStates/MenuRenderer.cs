using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.OverworldStates
{
    class MenuRenderer : OverworldStateRenderer
    {
        #region Instance Fields
        private TextureData whitePixelTextureData;
        #endregion

        #region Instance Properties
        protected new Menu overworldState
        {
            get { return base.overworldState as Menu; }
        }
        #endregion

        #region Constructors
        public MenuRenderer(OverworldState overworldState)
            : base(overworldState)
        {
            whitePixelTextureData = ResourceManager.GetTextureData("white_pixel");
        }
        #endregion

        #region Instance Methods
        public override void Update(Delta delta)
        {
        }

        public override void BeforeDraw(Renderer renderer)
        {
        }

        public override void AfterDraw(Renderer renderer)
        {
            renderer.Draw(whitePixelTextureData, Vector2.Zero, Color.Black, 0.0f, Game1.ScreenSize, false);
            drawMenuOptions(renderer);
        }

        private void drawMenuOptions(Renderer renderer)
        {
            Vector2 minScale = Game1.MinScreenScaleFactor;
            float fontSize = Font.DefaultSize * minScale.Y;
            Vector2 position = new Vector2(400.0f) * minScale;
            for (int i = 0; i < Menu.MenuOptions.Length; ++i)
            {
                string text = Menu.MenuOptions[i];
                Vector2 textSize = renderer.Font.MeasureString(text, fontSize);
                renderer.DrawText(text, position, Color.White, 0.0f, Vector2.Zero, minScale);

                if (i == overworldState.CurrentMenuOptionIndex)
                {
                    Vector2 arrowSize = renderer.Font.MeasureString(">", fontSize);
                    renderer.DrawText(">", position - new Vector2(arrowSize.X, 0.0f), Color.White, 0.0f, Vector2.Zero, minScale);
                }

                position.Y += textSize.Y * 1.1f;
            }
        }
        #endregion
    }
}
