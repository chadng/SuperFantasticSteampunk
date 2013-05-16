using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk
{
    class Particle
    {

        #region Instance Fields
        private readonly float lifeTime;
        private float time;
        private Vector2 position;
        private Vector2 velocity;
        private float gravity;
        private float scale;
        #endregion

        #region Instance Properties
        public bool Alive
        {
            get { return time < lifeTime; }
        }
        #endregion

        #region Constructors
        public Particle(Vector2 position, float speed, float gravity, float maxScale, float lifeTime)
        {
            time = 0.0f;
            this.position = position;
            this.gravity = gravity;
            this.lifeTime = lifeTime + (float)((Game1.Random.NextDouble() * 0.5) - 0.25);
            scale = (float)(maxScale * (1.0 - (Game1.Random.NextDouble() * 0.4)));
            velocity = new Vector2((float)((Game1.Random.NextDouble() - 0.5) * 2.0), (float)((Game1.Random.NextDouble() - 0.5) * 2.0)) * speed;
        }
        #endregion

        #region Instance Methods
        public void Update(GameTime gameTime)
        {
            float deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;
            time += deltaT;
            velocity.Y += gravity * deltaT;
            position += velocity * deltaT;
        }

        public void Draw(Renderer renderer, TextureData textureData)
        {
            float alpha = 1.0f - (time / lifeTime);
            renderer.Draw(textureData, position, Color.White * alpha, 0.0f, new Vector2(scale));
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
        public ParticleEffect(Vector2 position, int particleCount, TextureData textureData, float particleSpeed, float particleGravity, float maxScale, float lifeTime)
            : base(position)
        {
            this.textureData = textureData;
            particles = new Particle[particleCount];
            for (int i = 0; i < particleCount; ++i)
                particles[i] = new Particle(position, particleSpeed, particleGravity, maxScale, lifeTime);
        }
        #endregion

        #region Instance Methods
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            foreach (Particle particle in particles)
            {
                if (particle.Alive)
                    particle.Update(gameTime);
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
