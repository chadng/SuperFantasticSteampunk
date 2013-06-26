using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk
{
    abstract class ItemWithAttributes
    {
        #region Types
        protected delegate void particleEmissionCallback(float x, float y);
        #endregion

        #region Constants
        private const float statusParticleTime = 0.15f;
        #endregion

        #region Instance Fields
        private float statusParticleTimer;
        #endregion

        #region Instance Properties
        public Attributes Attributes { get; private set; }
        protected TextureData statusParticleTextureData { get; private set; }
        #endregion

        #region Constructors
        public ItemWithAttributes(Attributes attributes)
        {
            Attributes = attributes;
            statusParticleTimer = 0.0f;
            statusParticleTextureData = getTextureDataForStatus(Attributes.Status);
        }

        public ItemWithAttributes(string fullName)
            : this(new Attributes(fullName))
        {
        }
        #endregion

        #region Instance Methods
        public virtual void Update(Delta delta)
        {
            updateForStatusAttribute(delta);
            updateForAffiliationAttribute(delta);
        }

        protected abstract void emitStatusParticle(particleEmissionCallback callback);

        private void updateForStatusAttribute(Delta delta)
        {
            if (statusParticleTextureData == null)
                return;
            statusParticleTimer += delta.Time;
            if (statusParticleTimer >= statusParticleTime)
            {
                statusParticleTimer = 0.0f;
                emitStatusParticle(emitStatusParticleCallback);
            }
        }

        private void updateForAffiliationAttribute(Delta delta)
        {

        }

        private TextureData getTextureDataForStatus(Status status)
        {
            string searchString;
            switch (status)
            {
            case Status.Poisonous: searchString = "particles/poison_bubble"; break;
            case Status.Shocking: searchString = "particles/paralysis"; break;
            case Status.Scary: searchString = "particles/fear"; break;
            default: searchString = null; break;
            }
            return ResourceManager.GetTextureData(searchString);
        }

        private void emitStatusParticleCallback(float x, float y)
        {
            Scene.AddEntity(new FloatingParticle(new Vector2(x, y), new Vector2(0.0f, -200.0f), new Vector2(0.1f), 0.6f, statusParticleTextureData));
        }
        #endregion
    }
}
