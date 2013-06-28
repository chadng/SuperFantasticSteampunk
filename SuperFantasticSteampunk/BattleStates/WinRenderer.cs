using System;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.BattleStates
{
    class WinRenderer : BattleStateRenderer
    {
        #region Constants
        public const float MaxOverlayAlpha = 0.8f;
        private const float overlayAlphaTime = 1.0f;
        #endregion

        #region Instance Fields
        private readonly TextureData whitePixelTextureData;
        private readonly Vector2 minScale;
        private readonly int visibleItemCount;
        private float overlayAlphaTimer;
        #endregion

        #region Instance Properties
        public int VisibleItemCount
        {
            get { return visibleItemCount; }
        }

        protected new Win battleState
        {
            get { return base.battleState as Win; }
        }

        public bool FadeInFinished
        {
            get { return overlayAlphaTimer >= overlayAlphaTime; }
        }
        #endregion

        #region Constructors
        public WinRenderer(BattleState battleState)
            : base(battleState)
        {
            whitePixelTextureData = ResourceManager.GetTextureData("white_pixel");
            minScale = new Vector2(Math.Min(Game1.ScreenScaleFactor.X, Game1.ScreenScaleFactor.Y));
            visibleItemCount = (int)Math.Floor(10 * minScale.Y);
        }
        #endregion

        #region Instance Methods
        public override void Update(Delta delta)
        {
            if (overlayAlphaTimer < overlayAlphaTime)
            {
                overlayAlphaTimer += delta.Time;
                if (overlayAlphaTimer > overlayAlphaTime)
                    overlayAlphaTimer = overlayAlphaTime;
            }
        }

        public override void BeforeDraw(Renderer renderer)
        {
        }

        public override void AfterDraw(Renderer renderer)
        {
            float alpha = MaxOverlayAlpha * (overlayAlphaTimer / overlayAlphaTime);
            drawOverlay(alpha, renderer);
            alpha = overlayAlphaTimer / overlayAlphaTime;
            Vector2 winMessageEndPosition = drawWinMessage(alpha, renderer);
            drawWonItemsText(winMessageEndPosition, alpha, renderer);
        }

        private void drawOverlay(float alpha, Renderer renderer)
        {
            renderer.Draw(whitePixelTextureData, Vector2.Zero, new Color(0.0f, 0.0f, 0.0f, alpha), 0.0f, Game1.ScreenSize, false);
        }

        private Vector2 drawWinMessage(float alpha, Renderer renderer)
        {
            Vector2 fontScale = new Vector2(Font.DefaultSize * 3.0f * minScale.Y);
            Vector2 textSize = renderer.Font.MeasureString(battleState.WinMessage, fontScale.X);
            Vector2 position = new Vector2(Game1.ScreenSize.X / 2.0f, textSize.Y * 2);
            renderer.DrawText(battleState.WinMessage, position, new Color(1.0f, 1.0f, 1.0f, alpha), 0.0f, textSize / 2.0f, fontScale / Font.DefaultSize);

            string spoilMessage = "Here's what you won:";
            fontScale = new Vector2(Font.DefaultSize * 1.5f * minScale.Y);
            textSize = renderer.Font.MeasureString(spoilMessage, fontScale.X);
            position += new Vector2(0.0f, textSize.Y * 2.5f);
            renderer.DrawText(spoilMessage, position, new Color(1.0f, 1.0f, 1.0f, alpha), 0.0f, textSize / 2.0f, fontScale / Font.DefaultSize);

            return position + new Vector2(0.0f, textSize.Y * 2.5f);
        }

        private void drawWonItemsText(Vector2 position, float alpha, Renderer renderer)
        {
            Color textColor = new Color(1.0f, 1.0f, 1.0f, alpha);
            float characterClassHeadPadding = 10.0f * minScale.X;

            int startIndex;
            int finishIndex;
            battleState.Battle.CalculateStartAndFinishIndexesForMenuList(battleState.ItemsWon.Count, visibleItemCount, battleState.CurrentItemIndex, out startIndex, out finishIndex);
            
            float fontSize = 14.0f * minScale.Y;
            float arrowFontSize = 18.0f * minScale.Y;
            Vector2 fontScale = new Vector2(fontSize / Font.DefaultSize);
            Vector2 arrowFontScale = new Vector2(arrowFontSize / Font.DefaultSize);
            Vector2 arrowSize = renderer.Font.MeasureString("^", arrowFontSize);
            arrowSize = new Vector2(arrowSize.Y, arrowSize.X);

            Vector2 upArrowPosition = Vector2.Zero;
            for (int i = startIndex; i <= finishIndex; ++i)
            {
                string text = battleState.ItemsWon[i].Item1;
                Vector2 textSize = renderer.Font.MeasureString(text, fontSize);
                renderer.DrawText(text, position, textColor, 0.0f, textSize / 2.0f, fontScale);

                CharacterClass characterClass = battleState.ItemsWon[i].Item2;
                if (characterClass != CharacterClass.Enemy)
                {
                    TextureData textureData = battleState.Battle.CharacterClassHeadTextureData[characterClass];
                    Vector2 scale = new Vector2((1.0f / textureData.Height) * textSize.Y);
                    renderer.Draw(textureData, position - new Vector2((textSize.X / 2.0f) + characterClassHeadPadding + (textureData.Width * scale.X), textSize.Y / 2.0f), Color.White, 0.0f, scale, false);
                }

                float yPositionIncrement = textSize.Y * 1.5f;
                if (i == startIndex)
                    upArrowPosition = new Vector2(position.X, position.Y - yPositionIncrement);
                position.Y += yPositionIncrement;
            }

            if (startIndex > 0)
                renderer.DrawText("^", upArrowPosition, textColor, 0.0f, arrowSize / 2.0f, arrowFontScale);
            if (finishIndex < battleState.ItemsWon.Count - 1)
                renderer.DrawText("^", position, textColor, 0.0f, arrowSize / 2.0f, new Vector2(arrowFontScale.X, -arrowFontScale.Y));
        }
        #endregion
    }
}
