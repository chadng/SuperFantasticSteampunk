using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.OverworldStates
{
    class EncounterIntroRenderer : OverworldStateRenderer
    {
        #region Instance Fields
        private TextureData pixelTextureData;
        #endregion

        #region Instance Properties
        protected new EncounterIntro overworldState
        {
            get { return base.overworldState as EncounterIntro; }
        }
        #endregion

        #region Constructors
        public EncounterIntroRenderer(OverworldState overworldState)
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
            Color color = Color.Black * (overworldState.Time / EncounterIntro.FadeTimeInSeconds);
            renderer.Draw(pixelTextureData, Vector2.Zero, color, 0.0f, Game1.ScreenSize);
        }
        #endregion
    }
}
