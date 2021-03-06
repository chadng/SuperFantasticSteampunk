﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperFantasticSteampunk
{
    class SpriteAnimation
    {
        #region Instance Properties
        public string Name { get; private set; }
        public int[] Data { get; private set; }
        #endregion

        #region Constructors
        public SpriteAnimation(string name, int[] data)
        {
            Name = name;
            Data = data;
        }
        #endregion
    }

    class Sprite
    {
        #region Constants
        private const int framesPerSecond = 5;
        #endregion

        #region Instance Fields
        private int currentAnimationFrameIndex;
        #endregion

        #region Instance Properties
        public SpriteData Data { get; private set; }
        public SpriteAnimation CurrentAnimation { get; set; }
        public TextureData TextureData { get; private set; }
        public float Time { get; set; }
        #endregion

        #region Constructors
        public Sprite(SpriteData spriteData)
        {
            if (spriteData == null)
                throw new Exception("SpriteData cannot be null");
            Data = spriteData;
            TextureData = ResourceManager.GetTextureData(spriteData.TextureName);
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

        public void SetAnimation(SpriteAnimation animation)
        {
            CurrentAnimation = animation;
            Time = 0.0f;
            currentAnimationFrameIndex = 0;
        }

        public void Update(Delta delta)
        {
            if (CurrentAnimation == null)
                return;

            Time += delta.Time;
            if (Time >= 1.0f / framesPerSecond)
            {
                if (++currentAnimationFrameIndex >= CurrentAnimation.Data.Length)
                    currentAnimationFrameIndex = 0;
                Time = 0.0f;
            }
        }

        public Rectangle GetSourceRectangle()
        {
            int textureWidth = TextureData.Width - Data.OffsetX;
            int boundingWidth = Data.Width + (Data.PaddingX * 2);
            int boundingHeight = Data.Height + (Data.PaddingY * 2);

            int framesPerRow = textureWidth / boundingWidth;
            int frame = CurrentAnimation == null ? 0 : CurrentAnimation.Data[currentAnimationFrameIndex];

            int row = frame / framesPerRow;
            int column = frame % framesPerRow;

            return new Rectangle((column * boundingWidth) + Data.PaddingX + Data.OffsetX, (row * boundingHeight) + Data.PaddingY + Data.OffsetY, Data.Width, Data.Height);
        }
        #endregion
    }
}
