using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.BattleStates
{
    class IntroOutroRenderer : BattleStateRenderer
    {
        #region Instance Fields
        private TextureData pixelTextureData;
        #endregion

        #region Constructors
        public IntroOutroRenderer(BattleState battleState)
            : base(battleState)
        {
            pixelTextureData = ResourceManager.GetTextureData("white_pixel");
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
            Intro intro = battleState as Intro;
            float time = intro == null ? (battleState as Outro).Time : intro.Time;

            Color color = new Color(0.0f, 0.0f, 0.0f, 1.0f - (time / Intro.FadeTimeInSeconds));
            renderer.Draw(pixelTextureData, Vector2.Zero, color, 0.0f, Game1.ScreenSize, false);
        }
        #endregion
    }
}
