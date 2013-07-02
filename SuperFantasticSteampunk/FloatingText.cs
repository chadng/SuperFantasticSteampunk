using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk
{
    class FloatingText : Entity
    {
        #region Constants
        private float floatTimeInSeconds = 1.5f;
        private float floatSpeed = 100.0f;
        #endregion

        #region Instance Fields
        private string text;
        private float time;
        private Vector2 scale;
        private bool center;
        private Vector2 textOrigin;
        #endregion

        #region Constructors
        public FloatingText(string text, Color color, Vector2 position, float scale, bool center)
            : base(position)
        {
            this.text = text;
            this.scale = new Vector2(scale);
            this.center = center;
            Tint = color;
            time = 0.0f;
            Velocity = new Vector2(0.0f, -floatSpeed);
            ZIndex = -1;
            textOrigin = new Vector2(-1.0f);
        }
        #endregion

        #region Instance Methods
        public override void Update(Delta delta)
        {
            base.Update(delta);

            time += delta.Time;
            if (time > floatTimeInSeconds)
            {
                Tint = Color.Transparent;
                Kill();
            }
        }

        public override void Draw(Renderer renderer)
        {
            if (textOrigin.X < 0.0f)
                calculateTextOrigin(renderer);
            renderer.DrawText(text, Position, new Color(Tint.R, Tint.G, Tint.B, (byte)(255 * (1.0f - (time / floatTimeInSeconds)))), 0.0f, textOrigin, scale, true);
        }

        private void calculateTextOrigin(Renderer renderer)
        {
            if (center)
                textOrigin = renderer.Font.MeasureString(text, Font.DefaultSize * scale.X) / 2;
            else
                textOrigin = Vector2.Zero;
        }
        #endregion
    }
}
