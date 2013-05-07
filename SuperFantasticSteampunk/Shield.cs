using System;
using Microsoft.Xna.Framework.Graphics;

namespace SuperFantasticSteampunk
{
    class Shield
    {
        #region Instance Properties
        public ShieldData Data { get; private set; }
        public TextureData TextureData { get; private set; }
        #endregion

        #region Constructors
        public Shield(ShieldData shieldData)
        {
            if (shieldData == null)
                throw new Exception("ShieldData cannot be null");
            Data = shieldData;
            TextureData = ResourceManager.GetTextureData(Data.TextureName);
        }
        #endregion
    }
}
