using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;

namespace SuperFantasticSteampunk
{
    enum RendererState { NoneBegan, SpriteBatchBegan, SkeletonRendererBegan }

    class Renderer
    {
        #region Instance Properties
        public SpriteFont SpriteFont { get; set; }
        #endregion

        #region Instance Fields
        private SpriteBatch spriteBatch;
        private SkeletonRenderer skeletonRenderer;
        private RendererState state;
        #endregion

        #region Constructors
        public Renderer(GraphicsDevice graphicsDevice)
        {
            spriteBatch = new SpriteBatch(graphicsDevice);
            skeletonRenderer = new SkeletonRenderer(graphicsDevice);
            skeletonRenderer.BlendState = BlendState.NonPremultiplied;
            state = RendererState.NoneBegan;
        }
        #endregion

        #region Instance Methods
        public void Draw(TextureData textureData, Vector2 position, Color color, float rotation, Vector2 scale)
        {
            beginSpriteBatch();
            Rectangle sourceRect = new Rectangle(0, 0, textureData.Texture.Width, textureData.Texture.Height);
            Vector2 origin = new Vector2(textureData.OriginX, textureData.OriginY);
            spriteBatch.Draw(textureData.Texture, position, sourceRect, color, rotation, origin, scale, SpriteEffects.None, 0.0f);
        }

        public void Draw(TextureData textureData, Vector2 position, Color color)
        {
            Draw(textureData, position, color, 0.0f, new Vector2(1.0f));
        }

        public void Draw(Skeleton skeleton)
        {
            beginSkeletonRenderer();
            skeletonRenderer.Draw(skeleton);
        }

        public void DrawText(string text, Vector2 position, Color color)
        {
            beginSpriteBatch();
            spriteBatch.DrawString(SpriteFont, text, position, color);
        }

        public void End()
        {
            if (state == RendererState.SpriteBatchBegan)
                spriteBatch.End();
            else if (state == RendererState.SkeletonRendererBegan)
                skeletonRenderer.End();
            state = RendererState.NoneBegan;
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
        #endregion
    }
}
