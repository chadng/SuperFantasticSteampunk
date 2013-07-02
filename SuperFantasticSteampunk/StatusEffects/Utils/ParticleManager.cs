using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.StatusEffects.Utils
{
    class ParticleManager
    {
        #region Instance Fields
        private readonly float particleTime;
        private readonly TextureData textureData;
        private readonly string text;
        private readonly Color textColor;
        private float particleTimer;
        #endregion

        #region Constructors
        public ParticleManager(float particleTime, TextureData textureData)
        {
            this.particleTime = particleTime;
            this.textureData = textureData;
            text = null;
        }

        public ParticleManager(float particleTime, string text, Color textColor)
        {
            this.particleTime = particleTime;
            this.text = text;
            this.textColor = textColor;
            textureData = null;
        }
        #endregion

        #region Instance Methods
        public void Reset()
        {
            particleTimer = 0.0f;
        }

        public void Update(PartyMember partyMember, Delta delta)
        {
            particleTimer += delta.Time;
            if (particleTimer >= particleTime)
            {
                particleTimer = 0.0f;
                Rectangle boundingBox = partyMember.BattleEntity.GetBoundingBox();
                float x = boundingBox.X + (boundingBox.Width / 2) + Game1.Random.Next(boundingBox.Width / 2) - (boundingBox.Width / 4);
                float y = boundingBox.Y + (boundingBox.Height / 2) + (Game1.Random.Next(boundingBox.Height / 4) - (boundingBox.Height / 8));
                if (textureData != null)
                    Scene.AddEntity(new FloatingParticle(new Vector2(x, y), new Vector2(0.0f, -200.0f), new Vector2(0.3f), 1.4f, textureData));
                else if (text != null)
                    Scene.AddEntity(new FloatingText(text, textColor, new Vector2(x, y), 3.0f, true));
            }
        }
        #endregion
    }
}
