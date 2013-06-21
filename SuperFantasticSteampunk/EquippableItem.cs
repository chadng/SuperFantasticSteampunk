using Microsoft.Xna.Framework;
using Spine;

namespace SuperFantasticSteampunk
{
    class EquippableItem
    {
        #region Constants
        private const float statusParticleTime = 0.15f;
        #endregion

        #region Instance Fields
        private float statusParticleTimer;
        private TextureData statusParticleTextureData;
        #endregion

        #region Instance Properties
        public Attributes Attributes { get; private set; }
        public RegionAttachment SkeletonRegionAttachment { get; set; }
        public Bone SkeletonBone { get; set; }
        #endregion

        #region Constructors
        public EquippableItem(string fullName)
        {
            Attributes = new Attributes(fullName);
            SkeletonRegionAttachment = null;
            SkeletonBone = null;
            statusParticleTimer = 0.0f;
            statusParticleTextureData = getTextureDataForStatus(Attributes.Status);
        }
        #endregion

        #region Instance Methods
        public void Update(Delta delta)
        {
            if (SkeletonRegionAttachment == null || SkeletonBone == null)
                return;
            updateForStatusAttribute(delta);
            updateForAffiliationAttribute(delta);
        }

        private void updateForStatusAttribute(Delta delta)
        {
            if (statusParticleTextureData == null)
                return;
            statusParticleTimer += delta.Time;
            if (statusParticleTimer >= statusParticleTime)
            {
                statusParticleTimer = 0.0f;
                emitStatusParticle();
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

        private void emitStatusParticle()
        {
            float preX = (float)(Game1.Random.NextDouble() * SkeletonRegionAttachment.Width) - SkeletonRegionAttachment.RegionOffsetX;
            float preY = (float)(((Game1.Random.NextDouble() * 0.5) + 0.5) * SkeletonRegionAttachment.Height) - SkeletonRegionAttachment.RegionOffsetY;
            float x = (preX * SkeletonBone.M00) + (preY * SkeletonBone.M01) + SkeletonBone.WorldX;
            float y = (preX * SkeletonBone.M10) + (preY * SkeletonBone.M11) + SkeletonBone.WorldY;
            Scene.AddEntity(new FloatingParticle(new Vector2(x, y), new Vector2(0.0f, -200.0f), new Vector2(0.1f), 0.6f, statusParticleTextureData));
        }
        #endregion
    }
}

