using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
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
        private static SortedDictionary<string, ShieldData> shieldDataDictionary;
        private static SortedDictionary<string, PartyMemberData> partyMemberDataDictionary;
        private static SortedDictionary<string, TextureData> textureDataDictionary;
        private static SortedDictionary<string, SpriteFont> spriteFontDictionary;
        #endregion

        #region Static Methods
        public static void Initialize(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            skeletonDataDictionary = new SortedDictionary<string, SkeletonData>();
            populateSkeletonDataDictionary(contentManager, graphicsDevice);
            weaponDataDictionary = new SortedDictionary<string, WeaponData>();
            populateWeaponDataDictionary(contentManager);
            shieldDataDictionary = new SortedDictionary<string, ShieldData>();
            populateShieldDataDictionary(contentManager);
            partyMemberDataDictionary = new SortedDictionary<string, PartyMemberData>();
            populatePartyMemberDataDictionary(contentManager);
            textureDataDictionary = new SortedDictionary<string, TextureData>();
            populateTextureDataDictionary(contentManager);
            spriteFontDictionary = new SortedDictionary<string, SpriteFont>();
            populateSpriteFontDictionary(contentManager);
        }

        public static void UnloadContent()
        {
            foreach (var pair in textureDataDictionary)
                pair.Value.Texture.Dispose();
        }

        public static Skeleton GetNewSkeleton(string name)
        {
            SkeletonData skeletonData;
            if (skeletonDataDictionary.TryGetValue(name, out skeletonData))
                return new Skeleton(skeletonData);
            return null;
        }

        public static Weapon GetNewWeapon(string name)
        {
            WeaponData weaponData = GetWeaponData(name);
            if (weaponData == null)
                return null;
            else
                return new Weapon(weaponData);
        }

        public static WeaponData GetWeaponData(string name)
        {
            WeaponData weaponData;
            if (weaponDataDictionary.TryGetValue(name, out weaponData))
                return weaponData;
            return null;
        }

        public static Shield GetNewShield(string name)
        {
            ShieldData shieldData = GetShieldData(name);
            if (shieldData == null)
                return null;
            else
                return new Shield(shieldData);
        }

        public static ShieldData GetShieldData(string name)
        {
            ShieldData shieldData;
            if (shieldDataDictionary.TryGetValue(name, out shieldData))
                return shieldData;
            return null;
        }

        public static PartyMemberData GetPartyMemberData(string name)
        {
            PartyMemberData partyMemberData;
            if (partyMemberDataDictionary.TryGetValue(name, out partyMemberData))
                return partyMemberData;
            return null;
        }

        public static TextureData GetTextureData(string name)
        {
            TextureData textureData;
            if (textureDataDictionary.TryGetValue(name, out textureData))
                return textureData;
            return null;
        }

        public static SpriteFont GetSpriteFont(string name)
        {
            SpriteFont spriteFont;
            if (spriteFontDictionary.TryGetValue(name, out spriteFont))
                return spriteFont;
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

        private static void populateShieldDataDictionary(ContentManager contentManager)
        {
            List<Dictionary<string, object>> shieldDataList = loadItemData(contentManager.RootDirectory + @"\Items\Shields.txt");
            foreach (var data in shieldDataList)
            {
                ShieldData shieldData = newObjectFromItemData<ShieldData>(data);
                shieldDataDictionary.Add(shieldData.Name, shieldData);
                Logger.Log("Loaded shield '" + shieldData.Name + "'");
            }
        }

        private static void populatePartyMemberDataDictionary(ContentManager contentManager)
        {
            List<Dictionary<string, object>> partyMemberDataList = loadItemData(contentManager.RootDirectory + @"\Items\PartyMembers.txt");
            foreach (var data in partyMemberDataList)
            {
                PartyMemberData partyMemberData = newObjectFromItemData<PartyMemberData>(data);
                partyMemberDataDictionary.Add(partyMemberData.Name, partyMemberData);
                Logger.Log("Loaded party member '" + partyMemberData.Name + "'");
            }
        }

        private static void populateTextureDataDictionary(ContentManager contentManager)
        {
            List<Dictionary<string, object>> textureDataList = loadItemData(contentManager.RootDirectory + @"\Textures\Textures.txt");
            foreach (var data in textureDataList)
            {
                TextureData textureData = newObjectFromItemData<TextureData>(data);
                typeof(TextureData).GetProperty("Texture").SetValue(textureData, contentManager.Load<Texture2D>(@"Textures\" + textureData.FileName), null);
                textureDataDictionary.Add(textureData.Name, textureData);
                Logger.Log("Loaded texture data '" + textureData.Name + "'");
            }
        }


        private static List<Dictionary<string, object>> loadItemData(string itemsFilePath)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            if (!File.Exists(itemsFilePath))
            {
                Logger.Log("Could not find file '" + itemsFilePath + "' to load item data from");
                return result;
            }

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
            case "int": return parseItemData<int>(valueString);
            case "float": return parseItemData<float>(valueString);
            case "WeaponType": return parseItemData<WeaponType>(valueString);
            case "WeaponUseAgainst": return parseItemData<WeaponUseAgainst>(valueString);
            case "CharacterClass": return parseItemData<CharacterClass>(valueString);
            }
            return null;
        }

        private static object parseItemData<T>(string valueString)
        {
            TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(T));
            if (typeConverter != null)
                return typeConverter.ConvertFromString(valueString);
            return default(T);
        }

        private static void populateSpriteFontDictionary(ContentManager contentManager)
        {
            string spriteFontDirectory = contentManager.RootDirectory + @"\Fonts\";
            IEnumerable<string> spriteFontFileNames = Directory.EnumerateFiles(spriteFontDirectory, "*.xnb");
            foreach (string spriteFontFileName in spriteFontFileNames)
            {
                string spriteFontName = spriteFontFileName.Replace(spriteFontDirectory, "").Replace(".xnb", "");
                spriteFontDictionary.Add(spriteFontName, contentManager.Load<SpriteFont>(@"Fonts\" + spriteFontName));
                Logger.Log("Loaded font '" + spriteFontName + "'");
            }
        }
        #endregion
    }
}
