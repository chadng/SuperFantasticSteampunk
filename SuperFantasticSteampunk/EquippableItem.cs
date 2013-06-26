using Microsoft.Xna.Framework;
using Spine;

namespace SuperFantasticSteampunk
{
    class EquippableItem : ItemWithAttributes
    {
        #region Instance Properties
        public RegionAttachment SkeletonRegionAttachment { get; set; }
        public Bone SkeletonBone { get; set; }
        #endregion

        #region Constructors
        public EquippableItem(string fullName)
            : base(fullName)
        {
            SkeletonRegionAttachment = null;
            SkeletonBone = null;
        }
        #endregion

        #region Instance Methods
        public override void Update(Delta delta)
        {
            if (SkeletonRegionAttachment == null || SkeletonBone == null)
                return;
            base.Update(delta);
        }

        protected override void emitStatusParticle(particleEmissionCallback callback)
        {
            float preX = MathHelper.Lerp(SkeletonRegionAttachment.Offset[0], SkeletonRegionAttachment.Offset[4], (float)Game1.Random.NextDouble());
            float preY = MathHelper.Lerp(SkeletonRegionAttachment.Offset[1], SkeletonRegionAttachment.Offset[5], (float)(Game1.Random.NextDouble()));
            float x = (preX * SkeletonBone.M00) + (preY * SkeletonBone.M01) + SkeletonBone.WorldX;
            float y = (preX * SkeletonBone.M10) + (preY * SkeletonBone.M11) + SkeletonBone.WorldY;
            callback(x, y);
        }
        #endregion
    }
}

