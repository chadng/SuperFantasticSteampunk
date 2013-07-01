using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk
{
    class SpriteData
    {
        #region Instance Properties
        public string Name { get; private set; }
        public string TextureName { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int PaddingX { get; private set; }
        public int PaddingY { get; private set; }
        public int OffsetX { get; private set; }
        public int OffsetY { get; private set; }
        public int OriginX { get; private set; }
        public int OriginY { get; private set; }
        public string AnimationData { get; private set; }
        public Dictionary<string, SpriteAnimation> Animations { get; private set; }
        #endregion

        #region Instance Methods
        public void Init()
        {
            TextureData textureData = ResourceManager.GetTextureData(TextureName);
            if (textureData == null)
                throw new Exception("Texture data '" + TextureName + "' does not exist for sprite '" + Name + "'");
            if (Width <= 0)
            {
                Width = textureData.Width;
                if (OriginX <= 0)
                    OriginX = (int)textureData.OriginX;
            }
            if (Height <= 0)
            {
                Height = textureData.Height;
                if (OriginY <= 0)
                    OriginY = (int)textureData.OriginY;
            }
            populateAnimationsFromAnimationData();
        }

        private void populateAnimationsFromAnimationData()
        {
            Animations = new Dictionary<string, SpriteAnimation>();

            if (AnimationData == null || AnimationData.Length == 0)
                return;

            string[] animationStrings = AnimationData.Split(';');
            foreach (string animationString in animationStrings)
            {
                if (animationString.Length == 0)
                    continue;

                int colonIndex = animationString.IndexOf(':');
                string name = animationString.Substring(0, colonIndex);
                string[] intStrings = animationString.Substring(colonIndex + 1).Split(',');
                List<int> frames = new List<int>(intStrings.Length);
                foreach (string str in intStrings)
                    frames.Add(int.Parse(str.Trim()));

                Animations.Add(name, new SpriteAnimation(name, frames.ToArray()));
            }
        }
        #endregion
    }
}
