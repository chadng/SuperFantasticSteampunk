using System.Collections.Generic;

namespace SuperFantasticSteampunk
{
    class ShieldData
    {
        #region Instance properties
        public string Name { get; private set; }
        public string Description { get; private set; }
        public CharacterClass CharacterClass { get; private set; }
        public int Defence { get; private set; }
        public Rarity Rarity { get; private set; }
        public string TextureName { get; private set; }
        public bool ForceAttributes { get; private set; }
        public string BlacklistedAttributes { get; private set; }
        public Script Script { get; private set; }
        #endregion

        #region Instance Methods
        public List<string> BlacklistedAttributesToList()
        {
            if (BlacklistedAttributes == null || BlacklistedAttributes.Length == 0)
                return new List<string>();
            return new List<string>(BlacklistedAttributes.Replace(" ", "").ToLower().Split(';'));
        }
        #endregion
    }
}
