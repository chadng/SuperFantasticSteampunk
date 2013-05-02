using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;

namespace SuperFantasticSteampunk
{
    class Entity
    {
        #region Instance Fields
        private Texture2D texture;
        private AnimationState animationState;
        #endregion

        #region Instance Properties
        public Skeleton Skeleton { get; private set; }
        public Vector2 Position { get; set; }
        public Color Tint { get; set; }
        #endregion

        #region Constructors
        public Entity(Skeleton skeleton, Vector2 position)
            : this((Texture2D)null, position)
        {
            Skeleton = skeleton;
            animationState = new AnimationState(new AnimationStateData(skeleton.Data));
        }

        public Entity(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            Position = position;
            Tint = Color.White;
        }
        #endregion

        #region Instance Methods
        public virtual void Kill()
        {
            Scene.RemoveEntity(this);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (Skeleton != null)
                updateSkeleton(gameTime);
        }

        public virtual void Draw(Renderer renderer)
        {
            if (Skeleton != null)
                renderer.Draw(Skeleton);
            else
                renderer.Draw(texture, Position, Tint);
        }

        private void updateSkeleton(GameTime gameTime)
        {
            Skeleton.RootBone.X = Position.X;
            Skeleton.RootBone.Y = Position.Y;
            animationState.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            animationState.Apply(Skeleton);
            Skeleton.UpdateWorldTransform();
        }
        #endregion
    }
}
