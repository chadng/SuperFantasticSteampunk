using System;
using System.Collections.Generic;

namespace SuperFantasticSteampunk
{
    class Area
    {
        #region Constants
        private const int enemiesPerArea = 5;
        #endregion

        #region Instance Properties
        public AreaData Data { get; private set; }
        public List<string> EnemyNames { get; private set; }
        public List<string> BossNames { get; private set; }
        public List<string> ScenerySpriteNames { get; private set; }
        #endregion

        #region Constructors
        public Area(AreaData data)
        {
            if (data == null)
                throw new Exception("AreaData cannot be null");

            Data = data;
            ScenerySpriteNames = Data.OverworldScenerySpriteNamesToList();
            BossNames = Data.BossNamesToList();
            populateEnemyNames();
        }
        #endregion

        #region Instance Methods
        private void populateEnemyNames()
        {
            List<string> allEnemyNames = Data.EnemyNamesToList();
            if (allEnemyNames.Count <= enemiesPerArea)
                EnemyNames = allEnemyNames;
            else
            {
                EnemyNames = new List<string>(enemiesPerArea);
                while (EnemyNames.Count < enemiesPerArea)
                {
                    string name = allEnemyNames.Sample();
                    allEnemyNames.Remove(name);
                    EnemyNames.Add(name);
                }
            }
        }
        #endregion
    }
}
