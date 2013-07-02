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
            Vector2 textSize = Vector2.Zero;
            for (int i = 0; i < Menu.MenuOptions.Length; ++i)
            {
                string text = Menu.MenuOptions[i];
                textSize = renderer.Font.MeasureString(text, fontSize);
                renderer.DrawText(text, position, Color.White, 0.0f, Vector2.Zero, minScale);

                if (i == overworldState.CurrentMenuOptionIndex)
                {
                    Vector2 arrowSize = renderer.Font.MeasureString(">", fontSize);
                    renderer.DrawText(">", position - new Vector2(arrowSize.X, 0.0f), Color.White, 0.0f, Vector2.Zero, minScale);
                }

                position.Y += textSize.Y * 1.1f;
            }
            position.Y += textSize.Y * 2.2f;
            renderer.DrawText("TIPS:", position, Color.White, 0.0f, Vector2.Zero, minScale);
            position.Y += textSize.Y * 1.1f;
            renderer.DrawText("- Move your party around for strategic layouts", position, Color.White, 0.0f, Vector2.Zero, minScale);
            position.Y += textSize.Y * 1.1f;
            renderer.DrawText("- The front line defends the party members at the back", position, Color.White, 0.0f, Vector2.Zero, minScale);
            position.Y += textSize.Y * 1.1f;
            renderer.DrawText("- Shields will give extra defence, and will be returned to the inventory if unscathed during a round", position, Color.White, 0.0f, Vector2.Zero, minScale);
            position.Y += textSize.Y * 1.1f;
            renderer.DrawText("- You can only use ranged weapons when you're not in the front line", position, Color.White, 0.0f, Vector2.Zero, minScale);
            position.Y += textSize.Y * 1.1f;
            renderer.DrawText("- The more items you use, the more you win back at the end of a battle", position, Color.White, 0.0f, Vector2.Zero, minScale);
            position.Y += textSize.Y * 1.1f;
            renderer.DrawText("- Don't be afraid to use items. You'll win more items than you started with", position, Color.White, 0.0f, Vector2.Zero, minScale);
            position.Y += textSize.Y * 1.1f;
            renderer.DrawText("- Try to work out what the items do to build a decent strategy", position, Color.White, 0.0f, Vector2.Zero, minScale);
        }
        #endregion
    }
}
