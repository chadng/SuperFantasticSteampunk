using System;
using System.Collections.Generic;
using System.Text;

namespace SuperFantasticSteampunk
{
    enum Handling { NoHandling, Light, Heavy, Uncomfortable }
    enum Enhancement { NoEnhancement, Piercing, Explosive, Relentless, Inaccurate, Spiky }
    enum Status { NoStatus, Poisonous, Shocking, Scary }
    enum Affiliation { NoAffiliation, Light, Darkness, Doom }

    class Attributes
    {
        #region Constants
        private static readonly Dictionary<Handling, int> weaponHandlingChances = new Dictionary<Handling, int> {
            { Handling.NoHandling, 4 },
            { Handling.Light, 2 },
            { Handling.Heavy, 2 },
            { Handling.Uncomfortable, 1 }
        };

        private static readonly Dictionary<Handling, int> shieldHandlingChances = new Dictionary<Handling, int> {
            { Handling.NoHandling, 3 },
            { Handling.Light, 2 },
            { Handling.Heavy, 1 }
        };

        private static readonly Dictionary<Enhancement, int> weaponEnhancementChances = new Dictionary<Enhancement, int> {
            { Enhancement.NoEnhancement, 4 },
            { Enhancement.Piercing, 1 },
            { Enhancement.Explosive, 1 },
            { Enhancement.Relentless, 2 },
            { Enhancement.Inaccurate, 2 }
        };

        private static readonly Dictionary<Enhancement, int> shieldEnhancementChances = new Dictionary<Enhancement, int> {
            { Enhancement.NoEnhancement, 4 },
            { Enhancement.Spiky, 1 }
        };

        private static readonly Dictionary<Status, int> statusChances = new Dictionary<Status, int> {
            { Status.NoStatus, 4 },
            { Status.Poisonous, 2 },
            { Status.Shocking, 2 },
            { Status.Scary, 1 }
        };

        private static readonly Dictionary<Affiliation, int> affiliationChances = new Dictionary<Affiliation, int> {
            { Affiliation.NoAffiliation, 24 },
            { Affiliation.Light, 6 },
            { Affiliation.Darkness, 6 },
            { Affiliation.Doom, 1 }
        };

        private static readonly List<Handling> weaponHandlings;
        private static readonly List<Handling> shieldHandlings;
        private static readonly List<Enhancement> weaponEnhancements;
        private static readonly List<Enhancement> shieldEnhancements;
        private static readonly List<Status> statuses;
        private static readonly List<Affiliation> affiliations;

        private static readonly SortedDictionary<string, Handling> stringToHandlingMap;
        private static readonly SortedDictionary<string, Enhancement> stringToEnhancementMap;
        private static readonly SortedDictionary<string, Status> stringToStatusMap;
        private static readonly SortedDictionary<string, Affiliation> stringToAffiliationMap;
        #endregion

        #region Static Constructors
        static Attributes()
        {
            populateAttributesList(out weaponHandlings, weaponHandlingChances);
            populateAttributesList(out shieldHandlings, shieldHandlingChances);
            populateAttributesList(out weaponEnhancements, weaponEnhancementChances);
            populateAttributesList(out shieldEnhancements, shieldEnhancementChances);
            populateAttributesList(out statuses, statusChances);
            populateAttributesList(out affiliations, affiliationChances);
            populateStringToAttributeMap(out stringToHandlingMap);
            populateStringToAttributeMap(out stringToEnhancementMap);
            populateStringToAttributeMap(out stringToStatusMap);
            populateStringToAttributeMap(out stringToAffiliationMap);
        }
        #endregion

        #region Static Methods
        private static void populateAttributesList<T>(out List<T> list, Dictionary<T, int> dictionary)
        {
            list = new List<T>();
            Dictionary<T, int> tempDictionary = new Dictionary<T, int>(dictionary);
            List<T> keys = new List<T>(tempDictionary.Keys);
            while (tempDictionary.Count > 0)
            {
                T key = keys.Sample();
                if (tempDictionary[key] >= 1)
                {
                    list.Add(key);
                    --tempDictionary[key];
                }
                if (tempDictionary[key] <= 0)
                {
                    tempDictionary.Remove(key);
                    keys.Remove(key);
                }
            }
        }

        private static void populateStringToAttributeMap<T>(out SortedDictionary<string, T> map)
        {
            map = new SortedDictionary<string, T>();
            bool isNone = true;
            foreach (T attribute in Enum.GetValues(typeof(T)))
            {
                if (isNone)
                {
                    isNone = false;
                    continue;
                }
                map.Add(attribute.ToString().ToLower(), attribute);
            }
        }
        #endregion

        #region Instance Properties
        public Handling Handling { get; private set; }
        public Enhancement Enhancement { get; private set; }
        public Status Status { get; private set; }
        public Affiliation Affiliation { get; private set; }
        #endregion

        #region Constructors
        public Attributes(WeaponData weaponData)
        {
            generateAttributes(true, weaponData.BlacklistedAttributesToList(), weaponData.ForceAttributes);
        }

        public Attributes(ShieldData shieldData)
        {
            generateAttributes(false, shieldData.BlacklistedAttributesToList(), shieldData.ForceAttributes);
        }

        public Attributes(string str)
        {
            populateFromString(str);
        }
        #endregion

        #region Instance Methods
        public string ToString(string itemName)
        {
            StringBuilder result = new StringBuilder();
            if (Handling != Handling.NoHandling)
            {
                result.Append(Handling.ToString());
                result.Append(' ');
            }
            if (Enhancement != Enhancement.NoEnhancement)
            {
                result.Append(Enhancement.ToString());
                result.Append(' ');
            }
            if (Status != Status.NoStatus)
            {
                result.Append(Status.ToString());
                result.Append(' ');
            }
            result.Append(itemName);
            if (Affiliation != Affiliation.NoAffiliation)
            {
                result.Append(" of ");
                result.Append(Affiliation.ToString());
            }

            string resultString = result.ToString();
            return resultString.ToLower().ToUpperFirstChar();
        }

        private void generateAttributes(bool isWeapon, List<string> blacklistedAttributes, bool forceNotEmpty)
        {
            do
            {
                Handling = (isWeapon ? weaponHandlings : shieldHandlings).Sample();
                Enhancement = (isWeapon ? weaponEnhancements : shieldEnhancements).Sample();
                Status = statuses.Sample();
                Affiliation = affiliations.Sample();
            } while (attributesInList(blacklistedAttributes) || (forceNotEmpty && empty()));
        }

        private bool attributesInList(List<string> list)
        {
            if (list.Contains(Handling.ToString().ToLower()))
                return true;
            if (list.Contains(Enhancement.ToString().ToLower()))
                return true;
            if (list.Contains(Status.ToString().ToLower()))
                return true;
            if (list.Contains(Affiliation.ToString().ToLower()))
                return true;
            return false;
        }

        private bool empty()
        {
            if (Handling != Handling.NoHandling)
                return false;
            if (Enhancement != Enhancement.NoEnhancement)
                return false;
            if (Status != Status.NoStatus)
                return false;
            if (Affiliation != Affiliation.NoAffiliation)
                return false;
            return true;
        }

        private void populateFromString(string str)
        {
            reset();
            string[] parts = str.ToLower().Split(' ');
            if (parts.Length <= 1)
                return;
            if (parts.Length == 2)
            {
                Handling = tryConvertString(parts[0], stringToHandlingMap);
                Enhancement = tryConvertString(parts[0], stringToEnhancementMap);
                Status = tryConvertString(parts[0], stringToStatusMap);
                return;
            }

            int itemNameIndex = parts.Length - 1;
            if (parts.IndexOf("of") >= 0)
            {
                Affiliation = tryConvertString(parts[parts.Length - 1], stringToAffiliationMap);
                if (parts.Length == 3)
                    return;
                itemNameIndex = parts.Length - 3;
            }

            for (int i = 0; i < itemNameIndex; ++i)
            {
                if (Handling == Handling.NoHandling)
                    Handling = tryConvertString(parts[i], stringToHandlingMap);
                if (Enhancement == Enhancement.NoEnhancement)
                    Enhancement = tryConvertString(parts[i], stringToEnhancementMap);
                if (Status == Status.NoStatus)
                    Status = tryConvertString(parts[i], stringToStatusMap);
            }
        }

        private T tryConvertString<T>(string str, IDictionary<string, T> map)
        {
            T result;
            if (map.TryGetValue(str, out result))
                return result;
            return default(T);
        }

        private void reset()
        {
            Handling = Handling.NoHandling;
            Enhancement = Enhancement.NoEnhancement;
            Status = Status.NoStatus;
            Affiliation = Affiliation.NoAffiliation;
        }
        #endregion
    }
}
