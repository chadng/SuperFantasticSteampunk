using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SuperFantasticSteampunk
{
    using InventoryItem = KeyValuePair<string, int>;

    class Inventory : SortedDictionary<string, int>
    {
        #region Instance Fields
        private ConditionalWeakTable<PartyMember, string> lastUsedItemKeys;
        #endregion

        #region Constructors
        public Inventory()
        {
            lastUsedItemKeys = new ConditionalWeakTable<PartyMember, string>();
            this.Add("test1", 5);
            this.Add("test2", 5);
            this.Add("default", -1);
        }
        #endregion

        #region Instance Methods
        public void UseItem(string itemName, PartyMember user)
        {
            if (ContainsKey(itemName))
            {
                if (--this[itemName] == 0) // not <= because negative is infinite
                    Remove(itemName);
                else if (this[itemName] < 0)
                    this[itemName] = -1;
                lastUsedItemKeys.AddOrReplace(user, itemName);
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

        public List<InventoryItem> GetSortedItems(PartyMember partyMember)
        {
            List<InventoryItem> result = new List<InventoryItem>(Count);

            string lastUsedItemKey = null;
            bool pushLastUsedItemKeyToTop = lastUsedItemKeys.TryGetValue(partyMember, out lastUsedItemKey) && ContainsKey(lastUsedItemKey);

            foreach (InventoryItem item in this)
            {
                if (item.Key != lastUsedItemKey) // if pushLastUsedItemKeyToTop is false, lastUsedItemKey will be null anyway
                    result.Add(item);
            }

            result.Sort((a, b) => String.Compare(a.Key, b.Key));
            if (pushLastUsedItemKeyToTop)
                result.Insert(0, new InventoryItem(lastUsedItemKey, this[lastUsedItemKey]));

            return result;
        }
        #endregion
    }
}
