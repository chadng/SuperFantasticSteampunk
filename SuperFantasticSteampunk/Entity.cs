using Microsoft.Xna.Framework;
using Spine;

namespace SuperFantasticSteampunk
{
    class Entity
    {
        #region Instance Fields
        private Skeleton skeleton;
        private AnimationState animationState;
        #endregion

        #region Instance Properties
        public float X
        {
            get { return skeleton.RootBone.X; }
            set { skeleton.RootBone.X = value; }
        }

        public float Y
        {
            get { return skeleton.RootBone.Y; }
            set { skeleton.RootBone.Y = value; }
        }
        #endregion

        #region Constructors
        public Entity(string skeletonName, float x, float y)
        {
            skeleton = ResourceManager.GetSkeleton(skeletonName);
            animationState = new AnimationState(new AnimationStateData(skeleton.Data));
            skeleton.RootBone.X = x;
            skeleton.RootBone.Y = y;
        }
        #endregion

        #region Instance Methods
        public virtual void Kill()
        {
            Scene.RemoveEntity(this);
        }

        public virtual void Update(GameTime gameTime)
        {
            animationState.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            animationState.Apply(skeleton);
            skeleton.UpdateWorldTransform();
        }

        public virtual void Draw(SkeletonRenderer skeletonRenderer)
        {
            skeletonRenderer.Draw(skeleton);
        }
        #endregion
    }
}
