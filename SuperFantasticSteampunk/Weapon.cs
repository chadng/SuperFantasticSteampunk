using System;

namespace SuperFantasticSteampunk
{
    class Weapon
    {
        #region Instance Properties
        public WeaponData WeaponData { get; private set; }
        #endregion

        #region Constructors
        public Weapon(WeaponData weaponData)
        {
            if (weaponData == null)
                throw new Exception("WeaponData cannot be null");
            WeaponData = weaponData;
        }
        #endregion
    }
}
