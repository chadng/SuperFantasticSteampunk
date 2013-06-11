using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;

namespace SuperFantasticSteampunk
{
    class UpdateExtension
    {
        #region Instance Properties
        public Action<UpdateExtension, GameTime> Action { get; private set; }
        public bool Active { get; set; }
        #endregion

        #region Constructors
        public UpdateExtension(Action<UpdateExtension, GameTime> action)
        {
            Action = action;
            Active = true;
        }
        #endregion
    }

    class Entity
    {
        #region Instance Fields
        private TextureData shadowTextureData;
        private Rectangle skeletonBoundingBox;
        #endregion

        #region Instance Properties
        public Skeleton Skeleton { get; private set; }
        public Sprite Sprite { get; private set; }
        public AnimationState AnimationState { get; private set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration { get; set; }
        public Vector2 Scale { get; set; }
        public float Rotation { get; set; }
        public float AngularVelocity { get; set; }
        public Color Tint { get; set; }
        public int ZIndex { get; set; }
        public float Altitude { get; set; }
        public Bone ShadowFollowBone { get; set; }
        public bool Alive { get; private set; }
        public Nullable<Rectangle> CustomBoundingBox { get; set; }
        public List<UpdateExtension> UpdateExtensions { get; private set; }
        #endregion

        #region Constructors
        public Entity(Vector2 position)
        {
            ResetManipulation();
            Position = position;
            ZIndex = 0;
            Altitude = 0.0f;
            Alive = true;
            UpdateExtensions = new List<UpdateExtension>();
        }

        public Entity(Skeleton skeleton, Vector2 position)
            : this(position)
        {
            Skeleton = skeleton;
            skeletonBoundingBox = ResourceManager.GetSkeletonBoundingBox(skeleton.Data.Name);
            AnimationState = new AnimationState(new AnimationStateData(skeleton.Data));
            shadowTextureData = ResourceManager.GetTextureData("shadow");
        }

        public Entity(Sprite sprite, Vector2 position)
            : this(position)
        {
            Sprite = sprite;
        }
        #endregion

        #region Instance Methods
        public void ResetManipulation(params string[] exclude)
        {
            if (!exclude.Contains("Position"))
                Position = Vector2.Zero;
            if (!exclude.Contains("Velocity"))
                Velocity = Vector2.Zero;
            if (!exclude.Contains("Acceleration"))
                Acceleration = Vector2.Zero;
            if (!exclude.Contains("Scale"))
                Scale = new Vector2(1.0f);
            if (!exclude.Contains("Rotation"))
                Rotation = 0.0f;
            if (!exclude.Contains("AngularVelocity"))
                AngularVelocity = 0.0f;
            if (!exclude.Contains("Tint"))
                Tint = Color.White;
        }

        public void SetSkeletonAttachment(string slotName, string attachmentName, TextureData textureData = null, bool forceNoTextureData = false)
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
                else if (forceNoTextureData)
                {
                    Skeleton.Data.FindSkin("default").AddAttachment(Skeleton.FindSlotIndex(slotName), attachmentName, new RegionAttachment(attachmentName));
                    Skeleton.SetAttachment(slotName, attachmentName);
                }
            }
            else
                Skeleton.SetAttachment(slotName, attachmentName);
        }

        public bool CollidesWith(Entity other)
        {
            return GetBoundingBox().Intersects(other.GetBoundingBox());
        }

        public bool CollidesWith(Map map)
        {
            Rectangle bounds = GetBoundingBox();

            Point topLeftTileCoord = new Point(bounds.Left / Map.TileSize, bounds.Top / Map.TileSize);
            Point bottomRightTileCoord = new Point(bounds.Right / Map.TileSize, bounds.Bottom / Map.TileSize);

            for (int x = topLeftTileCoord.X; x <= bottomRightTileCoord.X; ++x)
            {
                for (int y = topLeftTileCoord.Y; y <= bottomRightTileCoord.Y; ++y)
                {
                    if (map.CollisionMap[x, y])
                    {
                        Rectangle tileRect = new Rectangle(x * Map.TileSize, y * Map.TileSize, Map.TileSize, Map.TileSize);
                        if (tileRect.EIntersects(bounds))
                            return true;
                    }
                }
            }

            return false;
        }

        public Rectangle GetBoundingBox()
        {
            return GetBoundingBoxAt(Position);
        }

        public Rectangle GetBoundingBoxAt(Vector2 position)
        {
            if (CustomBoundingBox != null)
                return transformBoundingBox(CustomBoundingBox.Value, position);

            if (Sprite != null)
            {
                return new Rectangle(
                    (int)(position.X - (Sprite.Data.OriginX * Scale.X)),
                    (int)(position.Y - (Sprite.Data.OriginY * Scale.Y)),
                    (int)(Sprite.Data.Width * Scale.X),
                    (int)(Sprite.Data.Height * Scale.Y)
                );
            }

            if (Skeleton != null)
                return transformBoundingBox(skeletonBoundingBox, position);
            
            return new Rectangle((int)position.X, (int)position.Y, 0, 0);
        }

        public virtual void Kill()
        {
            Alive = false;
        }

        public virtual void Update(GameTime gameTime)
        {
            float deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Velocity += Acceleration * deltaT;
            Position += Velocity * deltaT;
            Rotation += AngularVelocity * deltaT;

            if (Skeleton != null)
                updateSkeleton(gameTime);
            else if (Sprite != null)
                Sprite.Update(gameTime);
            
            if (UpdateExtensions.Count > 0)
            {
                foreach (UpdateExtension updateExtension in UpdateExtensions)
                {
                    if (updateExtension.Active)
                        updateExtension.Action(updateExtension, gameTime);
                }
                UpdateExtensions.RemoveAll(ue => !ue.Active);
            }
        }

        public virtual void Draw(Renderer renderer)
        {
            if (Skeleton != null)
            {
                Vector2 shadowPosition = Position;
                Vector2 shadowScale = new Vector2(1.5f);
                if (ShadowFollowBone != null)
                {
                    shadowPosition.X = ShadowFollowBone.WorldX;
                    shadowScale += new Vector2(ShadowFollowBone.WorldY - Skeleton.RootBone.WorldY) / 400.0f;
                }
                renderer.Draw(shadowTextureData, shadowPosition, Color.White * 0.5f, 0.0f, shadowScale * Scale);
                
                if (Altitude == 0.0f)
                    renderer.Draw(Skeleton);
                else
                {
                    Skeleton.RootBone.Y -= Altitude;
                    Skeleton.UpdateWorldTransform();
                    renderer.Draw(Skeleton);
                    Skeleton.RootBone.Y += Altitude;
                }
            }
            else if (Sprite != null)
                renderer.Draw(Sprite, Position, Tint, Rotation, Scale);
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

        private Rectangle transformBoundingBox(Rectangle boundingBox, Vector2 position)
        {
            return new Rectangle(
                (int)(position.X + (boundingBox.Left * Scale.X)),
                (int)(position.Y + (boundingBox.Top * Scale.Y)),
                (int)(boundingBox.Width * Scale.X),
                (int)(boundingBox.Height * Scale.X)
            );
        }
        #endregion
    }
}
