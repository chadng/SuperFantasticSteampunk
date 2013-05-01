using System.Collections.Generic;

namespace SuperFantasticSteampunk
{
    using InventoryItem = KeyValuePair<string, int>;

    class Inventory : SortedDictionary<string, int>
    {
        #region Instance Properties
        public string LastUsedItemKey { get; private set; }
        #endregion

        #region Constructors
        public Inventory()
        {
            LastUsedItemKey = null;
            this.Add("test1", 3);
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

        public List<InventoryItem> GetSortedItems()
        {
            List<InventoryItem> result = new List<InventoryItem>(Count);
            if (LastUsedItemKey != null)
                result.Add(new InventoryItem(LastUsedItemKey, this[LastUsedItemKey]));

            foreach (InventoryItem item in this)
            {
                if (item.Key != LastUsedItemKey)
                    result.Add(item);
            }

            result.Sort();
            return result;
        }
        #endregion
    }
}
