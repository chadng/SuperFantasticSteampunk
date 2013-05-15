using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk
{
    class FloatingText : Entity
    {
        #region Constants
        private float floatTimeInSeconds = 5.0f;
        private float floatSpeed = 100.0f;
        #endregion

        #region Instance Fields
        private string text;
        private float time;
        #endregion

        #region Constructors
        public FloatingText(string text, Color color, Vector2 position)
            : base(position)
        {
            this.text = text;
            Tint = color;
            time = 0.0f;
            Velocity = new Vector2(0.0f, -floatSpeed);
            ZIndex = -1;
        }
        #endregion

        #region Instance Methods
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            time += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (time > floatTimeInSeconds)
            {
                Tint = Color.Transparent;
                Kill();
            }
            else
            {
                Color newColor = Tint;
                newColor *= 1.0f - (time / floatTimeInSeconds);
                Tint = newColor;
            }
        }

        public override void Draw(Renderer renderer)
        {
            renderer.DrawText(text, Position, Tint, 0.0f, Vector2.Zero, new Vector2(5.0f));
        }
        #endregion
    }
}
