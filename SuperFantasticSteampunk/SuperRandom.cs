using System;
using System.Security.Cryptography;

namespace SuperFantasticSteampunk
{
    class SuperRandom
    {
        #region Instance Fields
        RandomNumberGenerator randomNumberGenerator;
        byte[] bytes;
        #endregion

        #region Constructors
        public SuperRandom()
        {
            randomNumberGenerator = RandomNumberGenerator.Create();
            bytes = new byte[4];
        }
        #endregion

        #region Instance Methods
        public double NextDouble()
        {
            randomNumberGenerator.GetBytes(bytes);
            UInt32 randomInt = BitConverter.ToUInt32(bytes, 0);
            return (double)randomInt / UInt32.MaxValue;
        }

        public int Next(int maxValue)
        {
            double percentage = NextDouble();
            return (int)Math.Round((maxValue - 1) * percentage);
        }
        #endregion
    }
}
