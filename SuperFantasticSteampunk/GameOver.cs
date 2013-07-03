using System;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk
{
    class GameOver : Scene
    {
        #region Constants
        private const float gameOverAlphaTime = 3.0f;
        private const string gameOverText = "G A M E  O V E R";
        #endregion

        #region Instance Fields
        private readonly TextureData whitePixelTextureData;
        private float gameOverAlphaTimer;
        #endregion

        #region Constructors
        public GameOver()
        {
            whitePixelTextureData = ResourceManager.GetTextureData("white_pixel");
            gameOverAlphaTimer = 0.0f;
        }
        #endregion

        #region Instance Methods
        protected override void update(Delta delta)
        {
            base.update(delta);
            if (gameOverAlphaTimer < gameOverAlphaTime)
            {
                gameOverAlphaTimer += delta.Time;
                if (gameOverAlphaTimer > gameOverAlphaTime)
                    gameOverAlphaTimer = gameOverAlphaTime;
            }
            else if (Input.AnyActionButton())
                Game1.RestartGame();
        }

        protected override void draw(Renderer renderer)
        {
            base.draw(renderer);
            renderer.Draw(whitePixelTextureData, Vector2.Zero, Color.Black, 0.0f, Game1.ScreenSize, false);
            Vector2 minScale = Game1.MinScreenScaleFactor;
            Vector2 fontScale = 4f * minScale;
            Vector2 textSize = renderer.Font.MeasureString(gameOverText, fontScale.Y * Font.DefaultSize);
            float alpha = gameOverAlphaTimer / gameOverAlphaTime;
            renderer.DrawText(gameOverText, Game1.ScreenSize / 2.0f, new Color(1.0f, 1.0f, 1.0f, alpha), 0.0f, textSize / 2.0f, fontScale);
        }
        #endregion
    }
}
