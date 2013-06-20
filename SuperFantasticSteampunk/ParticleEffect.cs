using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk
{
    class Particle
    {
        #region Constants
        private const float radialVelocity = MathHelper.Pi;
        #endregion

        #region Instance Fields
        private readonly float lifeTime;
        private float time;
        private Vector2 position;
        private Vector2 velocity;
        private float gravity;
        private float scale;
        private bool rotate;
        private float rotation;
        #endregion

        #region Instance Properties
        public bool Alive
        {
            get { return time < lifeTime; }
        }
        #endregion

        #region Constructors
        public Particle(Vector2 position, float speed, float gravity, float maxScale, float lifeTime, bool rotate)
        {
            time = 0.0f;
            this.position = position;
            this.gravity = gravity;
            this.lifeTime = lifeTime + (float)((Game1.Random.NextDouble() * 0.5) - 0.25);
            scale = (float)(maxScale * (1.0 - (Game1.Random.NextDouble() * 0.4)));
            velocity = new Vector2((float)((Game1.Random.NextDouble() - 0.5) * 2.0), (float)((Game1.Random.NextDouble() - 0.5) * 2.0)) * speed;
            this.rotate = rotate;
            rotation = rotate ? (float)(Game1.Random.NextDouble() * MathHelper.TwoPi) : 0.0f;
        }
        #endregion

        #region Instance Methods
        public void Update(Delta delta)
        {
            time += delta.Time;
            velocity.Y += gravity * delta.Time;
            position += velocity * delta.Time;
            if (rotate)
                rotation += radialVelocity * delta.Time;
        }

        public void Draw(Renderer renderer, TextureData textureData)
        {
            float alpha = 1.0f - (time / lifeTime);
            renderer.Draw(textureData, position, Color.White * alpha, rotation, new Vector2(scale));
        }
        #endregion
    }

    class ParticleEffect : Entity
    {
        #region Instance Fields
        TextureData textureData;
        private Particle[] particles;
        #endregion

        #region Constructors
        public ParticleEffect(Vector2 position, int particleCount, TextureData textureData, float particleSpeed, float particleGravity, float maxScale, float lifeTime, bool rotate)
            : base(position)
        {
            this.textureData = textureData;
            particles = new Particle[particleCount];
            for (int i = 0; i < particleCount; ++i)
                particles[i] = new Particle(position, particleSpeed, particleGravity, maxScale, lifeTime, rotate);
        }
        #endregion

        #region Instance Methods
        public override void Update(Delta delta)
        {
            base.Update(delta);
            foreach (Particle particle in particles)
            {
                if (particle.Alive)
                    particle.Update(delta);
            }
        }

        public override void Draw(Renderer renderer)
        {
            foreach (Particle particle in particles)
            {
                if (particle.Alive)
                    particle.Draw(renderer, textureData);
            }
        }
        #endregion
    }
}
