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
            this.Add("test1", 1);
        }
        #endregion

        #region Instance Methods
        public void UseItem(string itemName)
        {
            if (ContainsKey(itemName))
            {
                --this[itemName];
                if (this[itemName] == 0) // not <= because negative is infinite
                {
                    Remove(itemName);
                    LastUsedItemKey = null;
                }
                else
                {
                    LastUsedItemKey = itemName;
                    if (this[itemName] < 0)
                        this[itemName] = -1;
                }
            }
        }

        public void AddItem(string itemName)
        {
            if (ContainsKey(itemName))
            {
                if (this[itemName] > 0) // Negative is infinite so increment will mess it up
                    ++this[itemName];
            }
            else
                Add(itemName, 1);
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
