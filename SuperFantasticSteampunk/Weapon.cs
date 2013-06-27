using System;
using Microsoft.Xna.Framework.Graphics;
using Spine;

namespace SuperFantasticSteampunk
{
    class Weapon : EquippableItem
    {
        #region Instance Properties
        public WeaponData Data { get; private set; }
        public TextureData TextureData { get; private set; }

        public override string Name
        {
            get { return Data.Name; }
        }
        #endregion

        #region Constructors
        public Weapon(WeaponData weaponData, string fullName)
            : base(fullName)
        {
            if (weaponData == null)
                throw new Exception("WeaponData cannot be null");
            Data = weaponData;
            TextureData = ResourceManager.GetTextureData(Data.TextureName);
        }
        #endregion
    }
}
