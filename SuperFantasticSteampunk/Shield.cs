using System;

namespace SuperFantasticSteampunk
{
    class Shield
    {
        #region Instance Properties
        public ShieldData ShieldData { get; private set; }
        #endregion

        #region Constructors
        public Shield(ShieldData shieldData)
        {
            if (shieldData == null)
                throw new Exception("ShieldData cannot be null");
            ShieldData = shieldData;
        }
        #endregion
    }
}
