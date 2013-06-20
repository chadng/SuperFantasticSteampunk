﻿using System;
using Microsoft.Xna.Framework.Graphics;

namespace SuperFantasticSteampunk
{
    class Weapon
    {
        #region Instance Properties
        public WeaponData Data { get; private set; }
        public TextureData TextureData { get; private set; }
        public Attributes Attributes { get; private set; }
        #endregion

        #region Constructors
        public Weapon(WeaponData weaponData, string fullName)
        {
            if (weaponData == null)
                throw new Exception("WeaponData cannot be null");
            Data = weaponData;
            TextureData = ResourceManager.GetTextureData(Data.TextureName);
            Attributes = new Attributes(fullName);
        }
        #endregion
    }
}
