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
        private const float maxAffiliationTintAlpha = 0.8f;
        private const float affiliationTintAlphaSpeed = 0.5f;
        #endregion

        #region Instance Fields
        private readonly TextureData statusParticleTextureData;
        private float statusParticleTimer;
        private readonly Color affiliationTint;
        private float affiliationTintAlpha;
        private bool incAffiliationTintAlpha;
        #endregion

        #region Instance Properties
        public abstract string Name { get; }
        public Attributes Attributes { get; private set; }
        #endregion

        #region Constructors
        public ItemWithAttributes(Attributes attributes)
        {
            Attributes = attributes;
            statusParticleTextureData = getTextureDataForStatus(Attributes.Status);
            statusParticleTimer = 0.0f;
            affiliationTint = getColorForAffiliation(Attributes.Affiliation);
            affiliationTintAlpha = 0.0f;
            incAffiliationTintAlpha = true;
        }

        public ItemWithAttributes(string fullName)
            : this(new Attributes(fullName))
        {
        }
        #endregion

        #region Instance Methods
        public string GetFullName()
        {
            return Attributes.ToString(Name);
        }

        public virtual void Update(Delta delta)
        {
            updateForStatusAttribute(delta);
            updateForAffiliationAttribute(delta);
        }

        protected abstract void emitStatusParticle(particleEmissionCallback callback);

        protected abstract void updateAffiliationTint(Color color, float alpha);

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
            if (incAffiliationTintAlpha)
            {
                affiliationTintAlpha += affiliationTintAlphaSpeed * delta.Time;
                if (affiliationTintAlpha >= maxAffiliationTintAlpha)
                {
                    affiliationTintAlpha = maxAffiliationTintAlpha;
                    incAffiliationTintAlpha = false;
                }
            }
            else
            {
                affiliationTintAlpha -= affiliationTintAlphaSpeed * delta.Time;
                if (affiliationTintAlpha <= 0.0f)
                {
                    affiliationTintAlpha = 0.0f;
                    incAffiliationTintAlpha = true;
                }
            }

            updateAffiliationTint(affiliationTint, affiliationTintAlpha);
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

        private Color getColorForAffiliation(Affiliation affiliation)
        {
            switch (affiliation)
            {
            case Affiliation.Light: return Color.Gold;
            case Affiliation.Darkness: return Color.Black;
            case Affiliation.Doom: return Color.DarkRed;
            default: return Color.White;
            }
        }

        private void emitStatusParticleCallback(float x, float y)
        {
            Scene.AddEntity(new FloatingParticle(new Vector2(x, y), new Vector2(0.0f, -200.0f), new Vector2(0.1f), 0.6f, statusParticleTextureData));
        }
        #endregion
    }
}
