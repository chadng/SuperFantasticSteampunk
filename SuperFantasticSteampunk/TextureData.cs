using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperFantasticSteampunk
{
    class TextureData
    {
        #region Instance properties
        public string Name { get; private set; }
        public Texture2D Texture { get; private set; }
        public float OriginX { get; private set; }
        public float OriginY { get; private set; }
        public float ScaleX { get; private set; }
        public float ScaleY { get; private set; }
        public float Rotation { get; private set; }

        public int Width
        {
            get { return Texture.Width; }
        }

        public int Height
        {
            get { return Texture.Height; }
        }

        public Vector2 Size
        {
            get { return new Vector2(Width, Height); }
        }

        public Vector2 Origin
        {
            get { return new Vector2(OriginX, OriginY); }
        }

        public Vector2 Scale
        {
            get { return new Vector2(ScaleX, ScaleY); }
        }
        #endregion

        #region Instance Methods
        public void Init()
        {
            if (ScaleX == default(float))
                ScaleX = 1.0f;
            if (ScaleY == default(float))
                ScaleY = 1.0f;
        }
        #endregion
    }
}
