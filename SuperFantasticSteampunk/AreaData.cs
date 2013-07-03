using System.Collections.Generic;

namespace SuperFantasticSteampunk
{
    class TileData
    {
        #region Instance Properties
        public string TextureDataName { get; private set; }
        public int TileSize { get; private set; }
        #endregion

        #region Constructors
        public TileData(string textureDataName, int tileWidth)
        {
            TextureDataName = textureDataName;
            TileSize = tileWidth;
        }
        #endregion
    }

    class AreaData
    {
        #region Instance Fields
        private List<TileData> overworldTileDataCache;
        #endregion

        #region Instance Properties
        public string Name { get; private set; }
        public string EnemyNames { get; private set; }
        public string BossNames { get; private set; }
        public string OverworldScenerySpriteNames { get; private set; }
        public string OverworldTileTextureNames { get; private set; }
        public string BattleBackgroundTextureNames { get; private set; }
        public string BattleBackgroundScenerySpriteNames { get; private set; }
        public string BattleFloorScenerySpriteNames { get; private set; }
        #endregion

        #region Instance Methods
        public List<string> EnemyNamesToList()
        {
            return splitStringIntoList(EnemyNames);
        }
        public List<string> BossNamesToList()
        {
            return splitStringIntoList(BossNames);
        }

        public List<string> OverworldScenerySpriteNamesToList()
        {
            return splitStringIntoList(OverworldScenerySpriteNames);
        }

        public List<TileData> OverworldTileTextureNamesToList()
        {
            if (overworldTileDataCache != null)
                return overworldTileDataCache;

            List<string> tileTextureStrings = splitStringIntoList(OverworldTileTextureNames);
            overworldTileDataCache = new List<TileData>(tileTextureStrings.Count);
            foreach (string tileTextureString in tileTextureStrings)
            {
                string[] parts = tileTextureString.Split(':');
                string textureDataName = parts[0];
                int tileWidth = int.Parse(parts[1]);
                overworldTileDataCache.Add(new TileData(textureDataName, tileWidth));
            }

            return overworldTileDataCache;
        }

        public List<string> BattleBackgroundTextureNamesToList()
        {
            return splitStringIntoList(BattleBackgroundTextureNames);
        }

        public List<string> BattleBackgroundScenerySpriteNamesToList()
        {
            return splitStringIntoList(BattleBackgroundScenerySpriteNames);
        }

        public List<string> BattleFloorScenerySpriteNamesToList()
        {
            return splitStringIntoList(BattleFloorScenerySpriteNames);
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
