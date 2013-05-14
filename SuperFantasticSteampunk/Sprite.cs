using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperFantasticSteampunk
{
    class Sprite
    {
        #region Constants
        private const int framesPerSecond = 5;
        #endregion

        #region Instance Fields
        private int currentAnimationFrameIndex;
        private TextureData textureData;
        #endregion

        #region Instance Properties
        public SpriteData Data { get; private set; }
        public int[] CurrentAnimation { get; set; }
        public float Time { get; set; }

        public Texture2D Texture
        {
            get { return textureData.Texture; }
        }
        #endregion

        #region Constructors
        public Sprite(SpriteData spriteData)
        {
            if (spriteData == null)
                throw new Exception("SpriteData cannot be null");
            Data = spriteData;
            textureData = ResourceManager.GetTextureData(spriteData.TextureName);
            currentAnimationFrameIndex = 0;
            CurrentAnimation = null;
            Time = 0.0f;
        }
        #endregion

        #region Instance Methods
        public void SetAnimation(string name)
        {
            SetAnimation(Data.Animations[name]);
        }

        public void SetAnimation(int[] animation)
        {
            CurrentAnimation = animation;
            Time = 0.0f;
            currentAnimationFrameIndex = 0;
        }

        public void Update(GameTime gameTime)
        {
            if (CurrentAnimation == null)
                return;

            Time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Time >= 1.0f / framesPerSecond)
            {
                if (++currentAnimationFrameIndex >= CurrentAnimation.Length)
                    currentAnimationFrameIndex = 0;
                Time = 0.0f;
            }
        }

        public Rectangle GetSourceRectangle()
        {
            int textureWidth = textureData.Width - Data.OffsetX;
            int boundingWidth = Data.Width + (Data.PaddingX * 2);
            int boundingHeight = Data.Height + (Data.PaddingY * 2);

            int framesPerRow = textureWidth / boundingWidth;
            int frame = CurrentAnimation == null ? 0 : CurrentAnimation[currentAnimationFrameIndex];

            int row = frame / framesPerRow;
            int column = frame % framesPerRow;

            return new Rectangle((column * boundingWidth) + Data.PaddingX + Data.OffsetX, (row * boundingHeight) + Data.PaddingY + Data.OffsetY, Data.Width, Data.Height);
        }
        #endregion
    }
}
