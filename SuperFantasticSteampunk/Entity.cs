using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;

namespace SuperFantasticSteampunk
{
    class Entity
    {
        #region Instance Fields
        private TextureData textureData;
        #endregion

        #region Instance Properties
        public Skeleton Skeleton { get; private set; }
        public AnimationState AnimationState { get; private set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Scale { get; set; }
        public float Rotation { get; set; }
        public float AngularVelocity { get; set; }
        public Color Tint { get; set; }
        #endregion

        #region Constructors
        public Entity(Skeleton skeleton, Vector2 position)
            : this((TextureData)null, position)
        {
            Skeleton = skeleton;
            AnimationState = new AnimationState(new AnimationStateData(skeleton.Data));
        }

        public Entity(TextureData textureData, Vector2 position)
        {
            this.textureData = textureData;
            Position = position;
            Velocity = new Vector2(0.0f);
            Scale = new Vector2(1.0f);
            Rotation = 0.0f;
            AngularVelocity = 0.0f;
            Tint = Color.White;
        }
        #endregion

        #region Instance Methods
        public void SetSkeletonAttachment(string slotName, string attachmentName, TextureData textureData = null)
        {
            if (Skeleton == null || Skeleton.FindSlot(slotName) == null)
                return;

            if (Skeleton.GetAttachment(slotName, attachmentName) == null)
            {
                if (textureData != null)
                {
                    addSkeletonAttachment(slotName, attachmentName, textureData);
                    Skeleton.SetAttachment(slotName, attachmentName);
                }
            }
            else
                Skeleton.SetAttachment(slotName, attachmentName);
        }

        public virtual void Kill()
        {
            Scene.RemoveEntity(this);
        }

        public virtual void Update(GameTime gameTime)
        {
            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Rotation += AngularVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (Skeleton != null)
                updateSkeleton(gameTime);
        }

        public virtual void Draw(Renderer renderer)
        {
            if (Skeleton != null)
                renderer.Draw(Skeleton);
            else
                renderer.Draw(textureData, Position, Tint);
        }

        private void updateSkeleton(GameTime gameTime)
        {
            Skeleton.RootBone.X = Position.X;
            Skeleton.RootBone.Y = Position.Y;
            Skeleton.RootBone.ScaleX = Scale.X;
            Skeleton.RootBone.ScaleY = Scale.Y;
            Skeleton.RootBone.Rotation = Rotation;
            AnimationState.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            AnimationState.Apply(Skeleton);
            Skeleton.UpdateWorldTransform();
        }

        private void addSkeletonAttachment(string slotName, string attachmentName, TextureData textureData)
        {
            RegionAttachment regionAttachment = new RegionAttachment(attachmentName);

            regionAttachment.RendererObject = textureData.Texture;
            regionAttachment.Width = regionAttachment.RegionWidth = regionAttachment.RegionOriginalWidth = textureData.Texture.Width;
            regionAttachment.Height = regionAttachment.RegionHeight = regionAttachment.RegionOriginalHeight = textureData.Texture.Height;
            regionAttachment.RegionOffsetX = textureData.OriginX;
            regionAttachment.RegionOffsetY = textureData.OriginY;
            regionAttachment.Rotation = textureData.Rotation;
            regionAttachment.SetUVs(0, 0, 1, 1, false);
            regionAttachment.UpdateOffset();

            Skeleton.Data.FindSkin("default").AddAttachment(Skeleton.FindSlotIndex(slotName), attachmentName, regionAttachment);
        }
        #endregion
    }
}
