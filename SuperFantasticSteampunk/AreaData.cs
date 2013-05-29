using System.Collections.Generic;

namespace SuperFantasticSteampunk
{
    class AreaData
    {
        #region Instance Properties
        public string Name { get; private set; }
        public string EnemyNames { get; private set; }
        public string ScenerySpriteNames { get; private set; }
        public string BattleBackgroundTextureNames { get; private set; }
        public string BattleScenerySpriteNames { get; private set; }
        #endregion

        #region Instance Methods
        public List<string> EnemyNamesToList()
        {
            return splitStringIntoList(EnemyNames);
        }

        public List<string> ScenerySpriteNamesToList()
        {
            return splitStringIntoList(ScenerySpriteNames);
        }

        public List<string> BattleBackgroundTextureNamesToList()
        {
            return splitStringIntoList(BattleBackgroundTextureNames);
        }

        public List<string> BattleScenerySpriteNamesToList()
        {
            return splitStringIntoList(BattleScenerySpriteNames);
        }

        private List<string> splitStringIntoList(string str)
        {
            if (str == null || str.Length == 0)
                return new List<string>();
            return new List<string>(str.TrimEnd(';').Split(';'));
        }
        #endregion
    }
}
