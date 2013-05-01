using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Spine;

namespace SuperFantasticSteampunk
{
    static class ResourceManager
    {
        #region Static Fields
        private static SortedDictionary<string, SkeletonData> skeletonDataDictionary;
        private static SortedDictionary<string, WeaponData> weaponDataDictionary;
        #endregion

        #region Static Methods
        public static void Initialize(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            skeletonDataDictionary = new SortedDictionary<string, SkeletonData>();
            populateSkeletonDataDictionary(contentManager, graphicsDevice);
            weaponDataDictionary = new SortedDictionary<string, WeaponData>();
            populateWeaponDataDictionary(contentManager);
        }

        public static Skeleton GetSkeleton(string name)
        {
            SkeletonData skeletonData;
            if (skeletonDataDictionary.TryGetValue(name, out skeletonData))
                return new Skeleton(skeletonData);
            return null;
        }

        public static Weapon GetWeapon(string name)
        {
            WeaponData weaponData;
            if (weaponDataDictionary.TryGetValue(name, out weaponData))
                return new Weapon(weaponData);
            return null;
        }

        public static WeaponData GetWeaponData(string name)
        {
            WeaponData weaponData;
            if (weaponDataDictionary.TryGetValue(name, out weaponData))
                return weaponData;
            return null;
        }

        private static void populateSkeletonDataDictionary(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            string skeletonDirectory = contentManager.RootDirectory + @"\Skeletons\";
            IEnumerable<string> atlasFileNames = Directory.EnumerateFiles(skeletonDirectory, "*.atlas");
            foreach (string atlasFileName in atlasFileNames)
            {
                string skeletonName = atlasFileName.Replace(skeletonDirectory, "").Replace(".atlas", "");
                skeletonDataDictionary.Add(skeletonName, loadSkeletonData(skeletonDirectory + skeletonName, graphicsDevice));
                Logger.Log("Loaded skeleton '" + skeletonName + "'");
            }
        }

        private static SkeletonData loadSkeletonData(string skeletonPath, GraphicsDevice graphicsDevice)
        {
            Atlas atlas = new Atlas(skeletonPath + ".atlas", new XnaTextureLoader(graphicsDevice));
            SkeletonJson skeletonJson = new SkeletonJson(atlas);
            return skeletonJson.ReadSkeletonData(skeletonPath + ".json");
        }

        private static void populateWeaponDataDictionary(ContentManager contentManager)
        {
            List<Dictionary<string, object>> weaponDataList = loadItemData(contentManager.RootDirectory + @"\Items\Weapons.txt");
            foreach (var data in weaponDataList)
            {
                WeaponData weaponData = newObjectFromItemData<WeaponData>(data);
                weaponDataDictionary.Add(weaponData.Name, weaponData);
                Logger.Log("Loaded weapon '" + weaponData.Name + "'");
            }
        }

        private static List<Dictionary<string, object>> loadItemData(string itemsFilePath)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            if (!File.Exists(itemsFilePath))
                return result;

            Dictionary<string, object> currentData = new Dictionary<string,object>();
            foreach (string line in File.ReadAllLines(itemsFilePath))
            {
                if (line.Length == 0)
                {
                    if (currentData.Count > 0)
                    {
                        result.Add(currentData);
                        currentData = new Dictionary<string,object>();
                    }
                }
                else
                {
                    KeyValuePair<string, object> pair = parseItemDataLine(line);
                    currentData.Add(pair.Key, pair.Value);
                }
            }
            if (currentData.Count > 0)
                result.Add(currentData);

            return result;
        }

        private static T newObjectFromItemData<T>(Dictionary<string, object> data)
        {
            T result = Activator.CreateInstance<T>();
            foreach (KeyValuePair<string, object> property in data)
            {
                System.Reflection.PropertyInfo propertyInfo = typeof(T).GetProperty(property.Key);
                if (propertyInfo != null)
                    propertyInfo.SetValue(result, property.Value, null);
            }
            return result;
        }

        private static KeyValuePair<string, object> parseItemDataLine(string line)
        {
            int firstColonIndex = line.IndexOf(':');
            int secondColonIndex = line.IndexOf(':', firstColonIndex + 1);
            string key = line.Substring(0, firstColonIndex);
            string valueType = line.Substring(firstColonIndex + 1, secondColonIndex - firstColonIndex - 1);
            string valueString = line.Substring(secondColonIndex + 1);
            return new KeyValuePair<string, object>(key, parseItemDataValue(valueType, valueString));
        }

        private static object parseItemDataValue(string typeName, string valueString)
        {
            switch (typeName)
            {
            case "string": return (object)valueString;
            case "int": return parseItemDataInt(valueString);
            case "WeaponType": return parseItemDataEnum<WeaponType>(valueString);
            case "WeaponUseAgainst": return parseItemDataEnum<WeaponUseAgainst>(valueString);
            case "CharacterClass": return parseItemDataEnum<CharacterClass>(valueString);
            }
            return null;
        }

        private static object parseItemDataInt(string valueString)
        {
            int result;
            if (int.TryParse(valueString, out result))
                return (object)result;
            return (object)0;
        }

        private static object parseItemDataEnum<T>(string valueString) where T : struct
        {
            T result;
            if (Enum.TryParse<T>(valueString, out result))
                return (object)result;
            return (object)0;
        }
        #endregion
    }
}
