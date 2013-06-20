using System;
using Microsoft.Xna.Framework.Graphics;

namespace SuperFantasticSteampunk
{
    class Shield
    {
        #region Instance Properties
        public ShieldData Data { get; private set; }
        public TextureData TextureData { get; private set; }
        public Attributes Attributes { get; private set; }
        #endregion

        #region Constructors
        public Shield(ShieldData shieldData, string fullName)
        {
            if (shieldData == null)
                throw new Exception("ShieldData cannot be null");
            Data = shieldData;
            TextureData = ResourceManager.GetTextureData(Data.TextureName);
            Attributes = new Attributes(fullName);
        }
        #endregion
    }
}
