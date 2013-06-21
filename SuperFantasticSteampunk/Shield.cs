using System;
using Microsoft.Xna.Framework.Graphics;
using Spine;

namespace SuperFantasticSteampunk
{
    class Shield : EquippableItem
    {
        #region Instance Properties
        public ShieldData Data { get; private set; }
        public TextureData TextureData { get; private set; }
        #endregion

        #region Constructors
        public Shield(ShieldData shieldData, string fullName)
            : base(fullName)
        {
            if (shieldData == null)
                throw new Exception("ShieldData cannot be null");
            Data = shieldData;
            TextureData = ResourceManager.GetTextureData(Data.TextureName);
        }
        #endregion
    }
}
