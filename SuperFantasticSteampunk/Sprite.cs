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
        private SortedDictionary<string, int[]> animations;
        private int currentAnimationFrameIndex;
        private TextureData textureData;
        #endregion

        #region Instance Properties
        public SpriteData Data { get; private set; }
        public int[] CurrentAnimation { get; set; }
        public float Time { get; set; }

        public float OriginX
        {
            get { return textureData.OriginX; }
        }

        public float OriginY
        {
            get { return textureData.OriginY; }
        }

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
            animations = new SortedDictionary<string, int[]>();
            currentAnimationFrameIndex = 0;
            CurrentAnimation = null;
            Time = 0.0f;
        }
        #endregion

        #region Instance Methods
        public void AddAnimation(string name, int[] animation)
        {
            animations.Add(name, animation);
        }

        public void SetAnimation(string name)
        {
            SetAnimation(animations[name]);
        }

        public void SetAnimation(int[] animation)
        {
            CurrentAnimation = animation;
            Time = 0.0f;
            currentAnimationFrameIndex = 0;
        }

        public void Update(GameTime gameTime)
        {
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
            int textureHeight = textureData.Height - Data.OffsetY;
            int boundingWidth = Data.Width + (Data.PaddingX * 2);

            int framesPerRow = textureWidth / boundingWidth;
            int frame = CurrentAnimation == null ? 0 : CurrentAnimation[currentAnimationFrameIndex];

            int row = frame / framesPerRow;
            int column = frame % framesPerRow;

            return new Rectangle((column * Data.Width) + Data.PaddingX, (row * Data.Height) + Data.PaddingY, Data.Width, Data.Height);
        }
        #endregion
    }
}
