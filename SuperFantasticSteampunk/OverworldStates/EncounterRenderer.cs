using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.OverworldStates
{
    class EncounterRenderer : OverworldStateRenderer
    {
        #region Instance Fields
        private TextureData pixelTextureData;
        #endregion

        #region Constructors
        public EncounterRenderer(OverworldState overworldState)
            : base(overworldState)
        {
            pixelTextureData = ResourceManager.GetTextureData("white_pixel");
        }
        #endregion

        #region Instance Methods
        public override void Update(GameTime gameTime)
        {
        }

        public override void BeforeDraw(Renderer renderer)
        {
        }

        public override void AfterDraw(Renderer renderer)
        {
            renderer.Draw(pixelTextureData, Vector2.Zero, Color.Black, 0.0f, Game1.ScreenSize);
        }
        #endregion
    }
}
