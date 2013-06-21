using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Spine;

namespace SuperFantasticSteampunk
{
    static class ResourceManager
    {
        #region Static Fields
        private static SortedDictionary<string, SkeletonData> skeletonDataDictionary;
        private static SortedDictionary<string, Rectangle> skeletonBoundingBoxDictionary;
        private static SortedDictionary<string, WeaponData> weaponDataDictionary;
        private static SortedDictionary<string, ShieldData> shieldDataDictionary;
        private static SortedDictionary<string, ItemData> itemDataDictionary;
        private static SortedDictionary<string, PartyMemberData> partyMemberDataDictionary;
        private static SortedDictionary<string, SpriteData> spriteDataDictionary;
        private static SortedDictionary<string, TextureData> textureDataDictionary;
        private static SortedDictionary<string, Font> fontDictionary;
        private static SortedDictionary<string, AreaData> areaDataDictionary;
        private static SortedDictionary<string, Effect> shaderDictionary;
        #endregion

        #region Static Properties
        public static List<string> PartyMemberTitles { get; private set; }
        public static List<string> PartyMemberForenames { get; private set; }
        public static List<string> PartyMemberSurnames { get; private set; }
        #endregion

        #region Static Methods
        public static void Initialize(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            skeletonDataDictionary = new SortedDictionary<string, SkeletonData>();
            skeletonBoundingBoxDictionary = new SortedDictionary<string, Rectangle>();
            populateSkeletonDataDictionary(contentManager, graphicsDevice);
            weaponDataDictionary = new SortedDictionary<string, WeaponData>();
            populateWeaponDataDictionary(contentManager);
            shieldDataDictionary = new SortedDictionary<string, ShieldData>();
            populateShieldDataDictionary(contentManager);
            itemDataDictionary = new SortedDictionary<string, ItemData>();
            populateItemDataDictionary(contentManager);
            partyMemberDataDictionary = new SortedDictionary<string, PartyMemberData>();
            populatePartyMemberDataDictionary(contentManager);
            spriteDataDictionary = new SortedDictionary<string, SpriteData>();
            populateSpriteDataDictionary(contentManager);
            textureDataDictionary = new SortedDictionary<string, TextureData>();
            populateTextureDataDictionary(contentManager);
            fontDictionary = new SortedDictionary<string, Font>();
            populateFontDictionary(contentManager);
            areaDataDictionary = new SortedDictionary<string, AreaData>();
            populateAreaDataDictionary(contentManager);
            shaderDictionary = new SortedDictionary<string, Effect>();
            populateShaderDictionary(contentManager);

            PartyMemberTitles = new List<string>(File.ReadAllLines(contentManager.RootDirectory + "/Titles.txt"));
            PartyMemberForenames = new List<string>(File.ReadAllLines(contentManager.RootDirectory + "/Forenames.txt"));
            PartyMemberSurnames = new List<string>(File.ReadAllLines(contentManager.RootDirectory + "/Surnames.txt"));
        }

        public static void UnloadContent()
        {
            foreach (var pair in textureDataDictionary)
                pair.Value.Texture.Dispose();
            skeletonDataDictionary.Clear();
            weaponDataDictionary.Clear();
            shieldDataDictionary.Clear();
            itemDataDictionary.Clear();
            partyMemberDataDictionary.Clear();
            spriteDataDictionary.Clear();
            textureDataDictionary.Clear();
            fontDictionary.Clear();
            areaDataDictionary.Clear();

            PartyMemberTitles.Clear();
            PartyMemberForenames.Clear();
            PartyMemberSurnames.Clear();
        }

        public static Skeleton GetNewSkeleton(string name)
        {
            if (name == null)
                return null;

            SkeletonData skeletonData;
            if (skeletonDataDictionary.TryGetValue(name, out skeletonData))
                return new Skeleton(skeletonData);
            return null;
        }

        public static Rectangle GetSkeletonBoundingBox(string name)
        {
            if (name == null)
                return new Rectangle();

            Rectangle rectangle;
            if (skeletonBoundingBoxDictionary.TryGetValue(name, out rectangle))
                return rectangle;
            return new Rectangle();
        }

        public static Weapon GetNewWeapon(string name)
        {
            if (name == null)
                return null;

            WeaponData weaponData = GetWeaponData(name);
            if (weaponData == null)
                return null;
            else
                return new Weapon(weaponData, name);
        }

        public static WeaponData GetWeaponData(string name)
        {
            name = getItemDataNameFromFullName(name, weaponDataDictionary.Keys);
            if (name == null)
                return null;

            WeaponData weaponData;
            if (weaponDataDictionary.TryGetValue(name, out weaponData))
                return weaponData;
            return null;
        }

        public static List<WeaponData> GetWeaponDataWhere(Predicate<WeaponData> predicate)
        {
            return getDataWhere(weaponDataDictionary, predicate);
        }

        public static Shield GetNewShield(string name)
        {
            if (name == null)
                return null;

            ShieldData shieldData = GetShieldData(name);
            if (shieldData == null)
                return null;
            else
                return new Shield(shieldData, name);
        }

        public static ShieldData GetShieldData(string name)
        {
            name = getItemDataNameFromFullName(name, shieldDataDictionary.Keys);
            if (name == null)
                return null;

            ShieldData shieldData;
            if (shieldDataDictionary.TryGetValue(name, out shieldData))
                return shieldData;
            return null;
        }

        public static List<ShieldData> GetShieldDataWhere(Predicate<ShieldData> predicate)
        {
            return getDataWhere(shieldDataDictionary, predicate);
        }

        public static Item GetNewItem(string name)
        {
            if (name == null)
                return null;

            ItemData itemData = GetItemData(name);
            if (itemData == null)
                return null;
            else
                return new Item(itemData);
        }

        public static ItemData GetItemData(string name)
        {
            if (name == null)
                return null;

            ItemData itemData;
            if (itemDataDictionary.TryGetValue(name, out itemData))
                return itemData;
            return null;
        }

        public static List<ItemData> GetItemDataWhere(Predicate<ItemData> predicate)
        {
            return getDataWhere(itemDataDictionary, predicate);
        }

        public static PartyMember GetNewPartyMember(string name)
        {
            if (name == null)
                return null;

            PartyMemberData partyMemberData = GetPartyMemberData(name);
            if (partyMemberData == null)
                return null;
            else
                return new PartyMember(partyMemberData);
        }

        public static PartyMemberData GetPartyMemberData(string name)
        {
            if (name == null)
                return null;

            PartyMemberData partyMemberData;
            if (partyMemberDataDictionary.TryGetValue(name, out partyMemberData))
                return partyMemberData;
            return null;
        }

        public static Sprite GetNewSprite(string name)
        {
            if (name == null)
                return null;

            SpriteData spriteData = GetSpriteData(name);
            if (spriteData == null)
                return null;
            else
                return new Sprite(spriteData);
        }

        public static SpriteData GetSpriteData(string name)
        {
            if (name == null)
                return null;

            SpriteData spriteData;
            if (spriteDataDictionary.TryGetValue(name, out spriteData))
                return spriteData;
            return null;
        }

        public static TextureData GetTextureData(string name)
        {
            if (name == null)
                return null;

            TextureData textureData;
            if (textureDataDictionary.TryGetValue(name, out textureData))
                return textureData;
            return null;
        }

        public static Font GetFont(string name)
        {
            if (name == null)
                return null;

            Font font;
            if (fontDictionary.TryGetValue(name, out font))
                return font;
            return null;
        }

        public static Area GetNewArea(string name)
        {
            if (name == null)
                return null;

            AreaData areaData = GetAreaData(name);
            if (areaData == null)
                return null;
            else
                return new Area(areaData);
        }

        public static AreaData GetAreaData(string name)
        {
            if (name == null)
                return null;

            AreaData areaData;
            if (areaDataDictionary.TryGetValue(name, out areaData))
                return areaData;
            return null;
        }

        public static Effect GetShader(string name)
        {
            if (name == null)
                return null;

            Effect shader;
            if (shaderDictionary.TryGetValue(name, out shader))
                return shader;
            return null;
        }

        public static object ParseItemDataValue(string typeName, string valueString)
        {
            switch (typeName)
            {
            case "string": return (object)valueString;
            case "int": return ParseItemData<int>(valueString);
            case "float": return ParseItemData<float>(valueString);
            case "bool": return ParseItemData<bool>(valueString);
            case "WeaponType": return ParseItemData<WeaponType>(valueString);
            case "WeaponUseAgainst": return ParseItemData<WeaponUseAgainst>(valueString);
            case "CharacterClass": return ParseItemData<CharacterClass>(valueString);
            case "StatusEffectType": return ParseItemData<StatusEffectType>(valueString);
            case "OverworldMovementType": return ParseItemData<OverworldMovementType>(valueString);
            case "Rarity": return ParseItemData<Rarity>(valueString);
            case "Script": return valueString.Length == 0 ? null : new Script(valueString);
            }
            return null;
        }

        public static object ParseItemData<T>(string valueString)
        {
            TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(T));
            if (typeConverter != null)
                return typeConverter.ConvertFromString(valueString);
            return default(T);
        }

        private static void populateSkeletonDataDictionary(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            string skeletonDirectory = contentManager.RootDirectory + "/Skeletons/";
            IEnumerable<string> atlasFileNames = Directory.GetFiles(skeletonDirectory, "*.atlas");
            foreach (string atlasFileName in atlasFileNames)
            {
                string skeletonName = atlasFileName.Replace(skeletonDirectory, "").Replace(".atlas", "");
                SkeletonData skeletonData = loadSkeletonData(skeletonDirectory + skeletonName, graphicsDevice);
                skeletonDataDictionary.Add(skeletonName, skeletonData);
                skeletonBoundingBoxDictionary.Add(skeletonName, skeletonData.GenerateBoundingBox());
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
            List<Dictionary<string, object>> weaponDataList = loadItemData(contentManager.RootDirectory + "/Items/Weapons.txt");
            foreach (var data in weaponDataList)
            {
                WeaponData weaponData = newObjectFromItemData<WeaponData>(data);
                weaponDataDictionary.Add(weaponData.Name, weaponData);
                Logger.Log("Loaded weapon '" + weaponData.Name + "'");
            }
        }

        private static void populateShieldDataDictionary(ContentManager contentManager)
        {
            List<Dictionary<string, object>> shieldDataList = loadItemData(contentManager.RootDirectory + "/Items/Shields.txt");
            foreach (var data in shieldDataList)
            {
                ShieldData shieldData = newObjectFromItemData<ShieldData>(data);
                shieldDataDictionary.Add(shieldData.Name, shieldData);
                Logger.Log("Loaded shield '" + shieldData.Name + "'");
            }
        }

        private static void populateItemDataDictionary(ContentManager contentManager)
        {
            List<Dictionary<string, object>> itemDataList = loadItemData(contentManager.RootDirectory + "/Items/Items.txt");
            foreach (var data in itemDataList)
            {
                ItemData itemData = newObjectFromItemData<ItemData>(data);
                itemDataDictionary.Add(itemData.Name, itemData);
                Logger.Log("Loaded item '" + itemData.Name + "'");
            }
        }

        private static void populatePartyMemberDataDictionary(ContentManager contentManager)
        {
            List<Dictionary<string, object>> partyMemberDataList = loadItemData(contentManager.RootDirectory + "/Items/PartyMembers.txt");
            foreach (var data in partyMemberDataList)
            {
                PartyMemberData partyMemberData = newObjectFromItemData<PartyMemberData>(data);
                partyMemberDataDictionary.Add(partyMemberData.Name, partyMemberData);
                Logger.Log("Loaded party member '" + partyMemberData.Name + "'");
            }
        }

        private static void populateSpriteDataDictionary(ContentManager contentManager)
        {
            List<Dictionary<string, object>> spriteDataList = loadItemData(contentManager.RootDirectory + "/Items/Sprites.txt");
            foreach (var data in spriteDataList)
            {
                SpriteData spriteData = newObjectFromItemData<SpriteData>(data);
                spriteData.PopulateAnimationsFromAnimationData();
                spriteDataDictionary.Add(spriteData.Name, spriteData);
                Logger.Log("Loaded sprite '" + spriteData.Name + "'");
            }
        }

        private static void populateTextureDataDictionary(ContentManager contentManager)
        {
            string textureDirectory = contentManager.RootDirectory + "/Textures/";
            foreach (string filePath in Directory.GetFiles(textureDirectory, "*.png", SearchOption.AllDirectories))
            {
                string textureName = filePath.Replace(textureDirectory, "").Replace(".png", "").Replace('\\', '/');
                TextureData textureData = new TextureData();
                typeof(TextureData).GetProperty("Name").SetValue(textureData, textureName, null);
                typeof(TextureData).GetProperty("Texture").SetValue(textureData, contentManager.Load<Texture2D>(filePath.Replace(contentManager.RootDirectory + "/", "")), null);
                if (textureName.StartsWith("particles/"))
                {
                    typeof(TextureData).GetProperty("OriginX").SetValue(textureData, textureData.Width / 2, null);
                    typeof(TextureData).GetProperty("OriginY").SetValue(textureData, textureData.Height / 2, null);
                }
                textureDataDictionary.Add(textureName, textureData);
                Logger.Log("Loaded texture data '" + textureName + "'");
            }

            List<Dictionary<string, object>> textureDataList = loadItemData(textureDirectory + "Textures.txt");
            foreach (var data in textureDataList)
            {
                TextureData newTextureData = newObjectFromItemData<TextureData>(data);
                TextureData oldTextureData = textureDataDictionary[newTextureData.Name];
                typeof(TextureData).GetProperty("Texture").SetValue(newTextureData, oldTextureData.Texture, null);
                textureDataDictionary[newTextureData.Name] = newTextureData;
                Logger.Log("Updated texture data '" + newTextureData.Name + "'");
            }
        }

        private static void populateAreaDataDictionary(ContentManager contentManager)
        {
            List<Dictionary<string, object>> areaDataList = loadItemData(contentManager.RootDirectory + "/Items/Areas.txt");
            foreach (var data in areaDataList)
            {
                AreaData areaData = newObjectFromItemData<AreaData>(data);
                areaDataDictionary.Add(areaData.Name, areaData);
                Logger.Log("Loaded area '" + areaData.Name + "'");
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

            Dictionary<string, object> currentData = new Dictionary<string, object>();
            bool blockOpened = false;
            StringBuilder blockText = new StringBuilder();

            foreach (string line in File.ReadAllLines(itemsFilePath))
            {
                if (line.Length == 0)
                {
                    if (currentData.Count > 0)
                    {
                        result.Add(currentData);
                        currentData = new Dictionary<string, object>();
                    }
                }
                else if (blockOpened)
                {
                    if (line[0] == '}')
                    {
                        KeyValuePair<string, object> pair = parseItemDataLine(blockText.ToString());
                        currentData.Add(pair.Key, pair.Value);
                        blockText.Clear();
                        blockOpened = false;
                    }
                    else
                        blockText.Append(line.Trim());
                }
                else if (line[line.Length - 1] == '{')
                {
                    blockText.Append(line.Substring(0, line.Length - 1));
                    blockOpened = true;
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
            return new KeyValuePair<string, object>(key, ParseItemDataValue(valueType, valueString));
        }

        private static void populateFontDictionary(ContentManager contentManager)
        {
            string fontDirectory = contentManager.RootDirectory + "/Fonts/";
            foreach (string directoryName in Directory.GetDirectories(fontDirectory))
            {
                string fontName = directoryName.Replace(fontDirectory, "");
                fontDictionary.Add(fontName, new Font(fontName, contentManager));
                Logger.Log("Loaded font '" + fontName + "'");
            }
        }

        private static List<T> getDataWhere<T>(IDictionary<string, T> dictionary, Predicate<T> predicate)
        {
            List<T> result = new List<T>();
            foreach (KeyValuePair<string, T> keyValuePair in dictionary)
            {
                if (predicate(keyValuePair.Value))
                    result.Add(keyValuePair.Value);
            }
            return result;
        }

        private static void populateShaderDictionary(ContentManager contentManager)
        {
            string shaderDirectory = contentManager.RootDirectory + "/Shaders/";
            foreach (string filePath in Directory.GetFiles(shaderDirectory, "*.mgfxo"))
            {
                string shaderName = filePath.Replace(shaderDirectory, "").Replace(".mgfxo", "");
                shaderDictionary.Add(shaderName, contentManager.Load<Effect>(filePath.Replace(contentManager.RootDirectory + "/", "")));
                Logger.Log("Loaded shader '" + shaderName + "'");
            }
        }

        private static string getItemDataNameFromFullName(string name, IEnumerable<string> allKeys)
        {
            if (name == null)
                return null;

            List<string> allNames = new List<string>(allKeys);

            string[] parts = name.Split(' ');
            if (parts.Length == 0)
                return null;
            if (parts.Length == 1)
                return allNames.IndexOf(parts[0]) >= 0 ? parts[0] : null;
            if (parts.Length == 2)
                return allNames.IndexOf(parts[1]) >= 0 ? parts[1] : null;
            if (parts.IndexOf("of") < 0)
                return allNames.IndexOf(parts[parts.Length - 1]) >= 0 ? parts[parts.Length - 1] : null;

            return allNames.IndexOf(parts[parts.Length - 3]) >= 0 ? parts[parts.Length - 3] : null;
        }
        #endregion
    }
}
