using Microsoft.Xna.Framework;
using Spine;

namespace SuperFantasticSteampunk
{
    class EquippableItem : ItemWithAttributes
    {
        #region Instance Fields
        private Slot skeletonSlot;
        private Color originalSkeletonSlotColor;
        #endregion

        #region Instance Properties
        public RegionAttachment SkeletonRegionAttachment { get; set; }
        public Bone SkeletonBone { get; set; }
        public Slot SkeletonSlot
        {
            get { return skeletonSlot; }
            set
            {
                skeletonSlot = value;
                if (skeletonSlot != null)
                    originalSkeletonSlotColor = new Color(skeletonSlot.R, skeletonSlot.G, skeletonSlot.B, skeletonSlot.A);
            }
        }
        #endregion

        #region Constructors
        public EquippableItem(string fullName)
            : base(fullName)
        {
            SkeletonRegionAttachment = null;
            SkeletonBone = null;
            SkeletonSlot = null;
        }
        #endregion

        #region Instance Methods
        public override void Update(Delta delta)
        {
            if (SkeletonRegionAttachment == null || SkeletonBone == null || SkeletonSlot == null)
                return;
            base.Update(delta);
        }

        public void Finish()
        {
            if (skeletonSlot != null)
            {
                Vector4 colorVector = originalSkeletonSlotColor.ToVector4();
                skeletonSlot.R = colorVector.X;
                skeletonSlot.G = colorVector.Y;
                skeletonSlot.B = colorVector.Z;
                skeletonSlot.A = colorVector.W;
            }
        }

        protected override void emitStatusParticle(particleEmissionCallback callback)
        {
            float preX = MathHelper.Lerp(SkeletonRegionAttachment.Offset[0], SkeletonRegionAttachment.Offset[4], (float)Game1.Random.NextDouble());
            float preY = MathHelper.Lerp(SkeletonRegionAttachment.Offset[1], SkeletonRegionAttachment.Offset[5], (float)(Game1.Random.NextDouble()));
            float x = (preX * SkeletonBone.M00) + (preY * SkeletonBone.M01) + SkeletonBone.WorldX;
            float y = (preX * SkeletonBone.M10) + (preY * SkeletonBone.M11) + SkeletonBone.WorldY;
            callback(x, y);
        }

        protected override void updateAffiliationTint(Color color, float alpha)
        {
            Vector4 colorVector = Color.Lerp(originalSkeletonSlotColor, color, alpha).ToVector4();
            SkeletonSlot.R = colorVector.X;
            SkeletonSlot.G = colorVector.Y;
            SkeletonSlot.B = colorVector.Z;
            SkeletonSlot.A = colorVector.W;
        }
        #endregion
    }
}

