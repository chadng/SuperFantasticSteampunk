namespace SuperFantasticSteampunk
{
    enum Rarity { Common, Uncommon, Rare, Never }

    class ItemData
    {
        #region Instance properties
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Rarity Rarity { get; private set; }
        public string TextureName { get; private set; }
        public Script Script { get; private set; }
        #endregion
    }
}
