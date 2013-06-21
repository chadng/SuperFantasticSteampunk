using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk
{
    class FloatingParticle : Entity
    {
        #region Instance Fields
        private TextureData textureData;
        private float alphaSpeed;
        private float alpha;
        #endregion

        #region Constructors
        public FloatingParticle(Vector2 position, Vector2 velocity, Vector2 scale, float alphaTime, TextureData textureData)
            : base(position)
        {
            Velocity = velocity;
            Scale = scale;
            this.textureData = textureData;
            alphaSpeed = 1.0f / alphaTime;
            alpha = 1.0f;
            ZIndex = -1;
        }
        #endregion

        #region Instance Methods
        public override void Update(Delta delta)
        {
            base.Update(delta);
            alpha -= alphaSpeed * delta.Time;
            if (alpha <= 0.0f)
                Kill();
        }

        public override void Draw(Renderer renderer)
        {
            renderer.Draw(textureData, Position, new Color(1.0f, 1.0f, 1.0f, alpha), 0.0f, Scale);
        }
        #endregion
    }
}

