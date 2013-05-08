using System;

namespace SuperFantasticSteampunk
{
    class Item
    {
        #region Instance Properties
        public ItemData Data { get; private set; }
        public TextureData TextureData { get; private set; }
        #endregion

        #region Constructors
        public Item(ItemData itemData)
        {
            if (itemData == null)
                throw new Exception("ItemData cannot be null");
            Data = itemData;
            TextureData = ResourceManager.GetTextureData(Data.TextureName);
        }
        #endregion
    }
}
