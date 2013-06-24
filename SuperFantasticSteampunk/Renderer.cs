using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;

namespace SuperFantasticSteampunk
{
    enum RendererState { NoneBegan, SpriteBatchBegan, SkeletonRendererBegan }

    class Renderer
    {
        #region Constants
        public static readonly Color DefaultTint = Color.White;
        private readonly Effect defaultShader;
        #endregion

        #region Instance Properties
        public Font Font { get; set; }
        public Camera Camera { get; set; }
        public Color Tint { get; set; }
        public Effect CurrentShader { get; private set; }
        #endregion

        #region Instance Fields
        private SpriteBatch spriteBatch;
        private SkeletonRenderer skeletonRenderer;
        private RendererState state;
        #endregion

        #region Constructors
        public Renderer(GraphicsDevice graphicsDevice, Effect defaultShader)
        {
            this.defaultShader = defaultShader;
            Font = null;
            Camera = null;
            ResetTint();
            spriteBatch = new SpriteBatch(graphicsDevice);
            skeletonRenderer = new SkeletonRenderer(graphicsDevice);
            skeletonRenderer.BlendState = BlendState.NonPremultiplied;
            state = RendererState.NoneBegan;
            CurrentShader = null;
        }
        #endregion

        #region Instance Methods
        public void SetShader(Effect shader)
        {
            if (shader != CurrentShader)
            {
                End();
                CurrentShader = shader ?? defaultShader;
            }
        }

        public void Draw(Sprite sprite, Vector2 position, Color color, float rotation, Vector2 scale)
        {
            beginSpriteBatch();
            translatePosition(ref position);
            updateColor(ref color);
            updateScale(ref scale);
            Vector2 origin = new Vector2(sprite.Data.OriginX, sprite.Data.OriginY);
            spriteBatch.Draw(sprite.Texture, position, sprite.GetSourceRectangle(), color, rotation, origin, scale, SpriteEffects.None, 0.0f);
        }

        public void Draw(TextureData textureData, Vector2 position, Color color, float rotation, Vector2 scale, bool cameraTransform = true)
        {
            beginSpriteBatch();
            if (cameraTransform)
            {
                translatePosition(ref position);
                updateScale(ref scale);
            }
            updateColor(ref color);
            Rectangle sourceRect = new Rectangle(0, 0, textureData.Texture.Width, textureData.Texture.Height);
            Vector2 origin = new Vector2(textureData.OriginX, textureData.OriginY);
            spriteBatch.Draw(textureData.Texture, position, sourceRect, color, rotation, origin, scale, SpriteEffects.None, 0.0f);
        }

        public void Draw(TextureData textureData, Vector2 position, Color color)
        {
            Draw(textureData, position, color, 0.0f, new Vector2(1.0f));
        }

        public void Draw(Skeleton skeleton, Color color)
        {
            beginSkeletonRenderer();

            Vector4 colorBefore = new Vector4(skeleton.R, skeleton.G, skeleton.B, skeleton.A);
            updateColor(ref color);
            tintSkeleton(skeleton, color);

            if (Camera != null)
            {
                float previousX = skeleton.RootBone.X;
                float previousY = skeleton.RootBone.Y;
                float previousScaleX = skeleton.RootBone.ScaleX;
                float previousScaleY = skeleton.RootBone.ScaleY;
                Vector2 position = Camera.TranslateVector(new Vector2(previousX, previousY));
                skeleton.RootBone.X = position.X;
                skeleton.RootBone.Y = position.Y;
                skeleton.RootBone.ScaleX *= Camera.Scale.X;
                skeleton.RootBone.ScaleY *= Camera.Scale.Y;
                skeleton.UpdateWorldTransform();
                skeletonRenderer.Draw(skeleton);
                skeleton.RootBone.X = previousX;
                skeleton.RootBone.Y = previousY;
                skeleton.RootBone.ScaleX = previousScaleX;
                skeleton.RootBone.ScaleY = previousScaleY;
                skeleton.UpdateWorldTransform();
            }
            else
                skeletonRenderer.Draw(skeleton);

            skeleton.R = colorBefore.X;
            skeleton.G = colorBefore.Y;
            skeleton.B = colorBefore.Z;
            skeleton.A = colorBefore.W;
        }

        public void DrawText(string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, bool cameraTransform = false)
        {
            beginSpriteBatch();
            if (cameraTransform)
            {
                translatePosition(ref position);
                updateScale(ref scale);
            }
            float fontSize;
            SpriteFont spriteFont = Font.GetBestSizeSpriteFont(Font.DefaultSize * scale.Y, out fontSize);
            spriteBatch.DrawString(spriteFont, text, new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y)), color, rotation, origin, Vector2.One, SpriteEffects.None, 0.0f);
        }

        public void End()
        {
            if (state == RendererState.SpriteBatchBegan)
                spriteBatch.End();
            else if (state == RendererState.SkeletonRendererBegan)
                skeletonRenderer.End();
            state = RendererState.NoneBegan;
        }

        public void ResetTint()
        {
            Tint = DefaultTint;
        }

        private void beginSpriteBatch()
        {
            if (state != RendererState.SpriteBatchBegan)
            {
                if (state == RendererState.SkeletonRendererBegan)
                    skeletonRenderer.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, CurrentShader);
                state = RendererState.SpriteBatchBegan;
            }
        }

        private void beginSkeletonRenderer()
        {
            if (state != RendererState.SkeletonRendererBegan)
            {
                if (state == RendererState.SpriteBatchBegan)
                    spriteBatch.End();
                skeletonRenderer.Begin(CurrentShader);
                state = RendererState.SkeletonRendererBegan;
            }
        }

        private void translatePosition(ref Vector2 position)
        {
            if (Camera != null)
                position = Camera.TranslateVector(position);
        }

        private void updateColor(ref Color color)
        {
            if (Tint == DefaultTint)
                return;

            color = new Color(color.ToVector4() * Tint.ToVector4());
        }

        private void updateScale(ref Vector2 scale)
        {
            if (Camera != null)
                scale *= Camera.Scale;
        }

        private void tintSkeleton(Skeleton skeleton, Color color)
        {
            if (color == Color.White)
                return;

            Vector4 vectorColor = color.ToVector4();
            skeleton.R = vectorColor.X;
            skeleton.G = vectorColor.Y;
            skeleton.B = vectorColor.Z;
            skeleton.A = vectorColor.W;
        }
        #endregion
    }
}
