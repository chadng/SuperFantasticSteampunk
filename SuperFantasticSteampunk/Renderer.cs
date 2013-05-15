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
        #endregion

        #region Instance Properties
        public SpriteFont SpriteFont { get; set; }
        public Camera Camera { get; set; }
        public Color Tint { get; set; }
        #endregion

        #region Instance Fields
        private SpriteBatch spriteBatch;
        private SkeletonRenderer skeletonRenderer;
        private RendererState state;
        #endregion

        #region Constructors
        public Renderer(GraphicsDevice graphicsDevice)
        {
            SpriteFont = null;
            Camera = null;
            ResetTint();
            spriteBatch = new SpriteBatch(graphicsDevice);
            skeletonRenderer = new SkeletonRenderer(graphicsDevice);
            skeletonRenderer.BlendState = BlendState.NonPremultiplied;
            state = RendererState.NoneBegan;
        }
        #endregion

        #region Instance Methods
        public void Draw(Sprite sprite, Vector2 position, Color color, float rotation, Vector2 scale)
        {
            beginSpriteBatch();
            translatePosition(ref position);
            updateColor(ref color);
            Vector2 origin = new Vector2(sprite.Data.OriginX, sprite.Data.OriginY);
            spriteBatch.Draw(sprite.Texture, position, sprite.GetSourceRectangle(), color, rotation, origin, scale, SpriteEffects.None, 0.0f);
        }

        public void Draw(TextureData textureData, Vector2 position, Color color, float rotation, Vector2 scale)
        {
            beginSpriteBatch();
            translatePosition(ref position);
            updateColor(ref color);
            Rectangle sourceRect = new Rectangle(0, 0, textureData.Texture.Width, textureData.Texture.Height);
            Vector2 origin = new Vector2(textureData.OriginX, textureData.OriginY);
            spriteBatch.Draw(textureData.Texture, position, sourceRect, color, rotation, origin, scale, SpriteEffects.None, 0.0f);
        }

        public void Draw(TextureData textureData, Vector2 position, Color color)
        {
            translatePosition(ref position);
            updateColor(ref color);
            Draw(textureData, position, color, 0.0f, new Vector2(1.0f));
        }

        public void Draw(Skeleton skeleton)
        {
            beginSkeletonRenderer();

            Vector4 colorBefore = new Vector4(skeleton.R, skeleton.G, skeleton.B, skeleton.A);
            tintSkeleton(skeleton);

            if (Camera != null)
            {
                float previousX = skeleton.RootBone.X;
                float previousY = skeleton.RootBone.Y;
                Vector2 position = Camera.TranslateVector(new Vector2(previousX, previousY));
                skeleton.RootBone.X = position.X;
                skeleton.RootBone.Y = position.Y;
                skeletonRenderer.Draw(skeleton);
                skeleton.RootBone.X = previousX;
                skeleton.RootBone.Y = previousY;
            }
            else
                skeletonRenderer.Draw(skeleton);

            skeleton.R = colorBefore.X;
            skeleton.G = colorBefore.Y;
            skeleton.B = colorBefore.Z;
            skeleton.A = colorBefore.W;
        }

        public void DrawText(string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale)
        {
            beginSpriteBatch();
            spriteBatch.DrawString(SpriteFont, text, position, color, rotation, origin, scale, SpriteEffects.None, 0.0f);
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
                spriteBatch.Begin();
                state = RendererState.SpriteBatchBegan;
            }
        }

        private void beginSkeletonRenderer()
        {
            if (state != RendererState.SkeletonRendererBegan)
            {
                if (state == RendererState.SpriteBatchBegan)
                    spriteBatch.End();
                skeletonRenderer.Begin();
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

        private void tintSkeleton(Skeleton skeleton)
        {
            if (Tint == DefaultTint)
                return;

            Vector4 vectorTint = Tint.ToVector4();
            skeleton.R = vectorTint.X;
            skeleton.G = vectorTint.Y;
            skeleton.B = vectorTint.Z;
            skeleton.A = vectorTint.W;
        }
        #endregion
    }
}
