using System.Collections.Generic;

namespace SuperFantasticSteampunk
{
    using InventoryItemList = List<KeyValuePair<string, int>>;

    class Inventory : SortedDictionary<string, int>
    {
        #region Instance Properties
        public string LastUsedItemKey { get; private set; }
        #endregion

        #region Constructors
        public Inventory()
        {
            LastUsedItemKey = null;
        }
        #endregion

        #region Instance Methods
        public void UseItem(string itemName)
        {
            if (ContainsKey(itemName))
            {
                --this[itemName];
                if (this[itemName] <= 0)
                {
                    Remove(itemName);
                    LastUsedItemKey = null;
                }
                else
                    LastUsedItemKey = itemName;
            }
        }

        public InventoryItemList GetOrderedItems()
        {
            InventoryItemList result = new List<KeyValuePair<string, int>>(Count);
            if (LastUsedItemKey != null)
                result.Add(new KeyValuePair<string, int>(LastUsedItemKey, this[LastUsedItemKey]));

            foreach (KeyValuePair<string, int> item in this)
            {
                if (item.Key != LastUsedItemKey)
                    result.Add(item);
            }
            return result;
        }
        #endregion
    }
}
