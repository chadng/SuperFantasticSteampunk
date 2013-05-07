using Microsoft.Xna.Framework.Graphics;

namespace SuperFantasticSteampunk
{
    class TextureData
    {
        #region Instance properties
        public string Name { get; private set; }
        public string FileName { get; private set; }
        public Texture2D Texture { get; private set; }
        public float OriginX { get; private set; }
        public float OriginY { get; private set; }
        public float Rotation { get; private set; }
        #endregion
    }
}
