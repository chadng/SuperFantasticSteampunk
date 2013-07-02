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
        private ConditionalWeakTable<PartyMember, Inventory> enemyPartyMemberInventories;
        #endregion

        #region Constructors
        public Inventory(bool isEnemyParty)
        {
            lastUsedItemKeys = new ConditionalWeakTable<PartyMember, string>();
            enemyPartyMemberInventories = isEnemyParty ? new ConditionalWeakTable<PartyMember, Inventory>() : null;
        }
        #endregion

        #region Instance Methods
        public void UseItem(string itemName, PartyMember user)
        {
            if (enemyPartyMemberInventories != null)
            {
                Inventory innerInventory;
                if (enemyPartyMemberInventories.TryGetValue(user, out innerInventory))
                    innerInventory.UseItem(itemName, user);
            }
            else
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
        }

        public void AddItem(string itemName, PartyMember partyMember, bool infinite = false)
        {
            if (enemyPartyMemberInventories != null)
            {
                Inventory innerInventory;
                if (!enemyPartyMemberInventories.TryGetValue(partyMember, out innerInventory))
                {
                    innerInventory = new Inventory(false);
                    enemyPartyMemberInventories.Add(partyMember, innerInventory);
                }
                innerInventory.AddItem(itemName, partyMember, infinite);
            }
            else
            {
                itemName = itemName.ToUpperFirstChar();
                if (ContainsKey(itemName))
                {
                    if (infinite)
                        this[itemName] = -1;
                    else if (this[itemName] > 0) // Negative is infinite so increment will mess it up
                        ++this[itemName];
                }
                else
                    Add(itemName, infinite ? -1 : 1);
            }
        }

        public List<InventoryItem> GetSortedItems(PartyMember partyMember)
        {
            if (enemyPartyMemberInventories != null)
            {
                Inventory innerInventory;
                if (enemyPartyMemberInventories.TryGetValue(partyMember, out innerInventory))
                    return innerInventory.GetSortedItems(partyMember);
                return new List<InventoryItem>();
            }
            else
            {
                List<InventoryItem> result = new List<InventoryItem>(Count);

                string lastUsedItemKey = null;
                bool pushLastUsedItemKeyToTop = lastUsedItemKeys.TryGetValue(partyMember, out lastUsedItemKey) && ContainsKey(lastUsedItemKey);

                foreach (InventoryItem item in this)
                {
                    if (item.Key != lastUsedItemKey) // if pushLastUsedItemKeyToTop is false, lastUsedItemKey will be null anyway
                        result.Add(item);
                }

                result.Sort((a, b) => a.Value < 0 || b.Value < 0 ? a.Value.CompareTo(b.Value) : String.Compare(a.Key, b.Key));
                if (pushLastUsedItemKeyToTop)
                    result.Insert(0, new InventoryItem(lastUsedItemKey, this[lastUsedItemKey]));

                return result;
            }
        }
        #endregion
    }
}
