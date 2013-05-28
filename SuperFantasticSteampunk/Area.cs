using System;
using System.Collections.Generic;

namespace SuperFantasticSteampunk
{
    class Area
    {
        #region Instance Properties
        public AreaData Data { get; private set; }
        public List<string> EnemyNames { get; private set; }
        public List<string> ScenerySpriteNames { get; private set; }
        #endregion

        #region Constructors
        public Area(AreaData data)
        {
            if (data == null)
                throw new Exception("AreaData cannot be null");

            Data = data;
            EnemyNames = new List<string>(data.EnemyNames.TrimEnd(';').Split(';'));
            ScenerySpriteNames = new List<string>(data.ScenerySpriteNames.TrimEnd(';').Split(';'));
        }
        #endregion
    }
}
