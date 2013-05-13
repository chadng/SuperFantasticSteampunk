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
        #endregion
    }
}
