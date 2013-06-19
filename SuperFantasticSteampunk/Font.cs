using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SuperFantasticSteampunk
{
    class Font
    {
        #region Constants
        public const int DefaultSize = 10;
        #endregion

        #region Instance Fields
        private List<KeyValuePair<int, SpriteFont>> spriteFonts;
        private int largestSize;
        private int smallestSize;
        #endregion

        #region Instance Properties
        public string Name { get; private set; }
        #endregion

        #region Constructors
        public Font(string name, ContentManager contentManager)
        {
            Name = name;
            spriteFonts = new List<KeyValuePair<int, SpriteFont>>();
            largestSize = 0;
            smallestSize = -1;
            loadSpriteFonts(contentManager);
        }
        #endregion

        #region Instance Methods
        public SpriteFont GetBestSizeSpriteFont(float size, out float finalSize)
        {
            if (size <= smallestSize)
            {
                finalSize = smallestSize;
                return spriteFonts[0].Value;
            }
            if (size >= largestSize)
            {
                finalSize = largestSize;
                return spriteFonts[spriteFonts.Count - 1].Value;
            }

            int previousSpriteFontSize = 0;
            SpriteFont previousSpriteFont = null;
            foreach (KeyValuePair<int, SpriteFont> keyValuePair in spriteFonts)
            {
                if (size == keyValuePair.Key)
                {
                    finalSize = keyValuePair.Key;
                    return keyValuePair.Value;
                }

                if (size < keyValuePair.Key)
                {
                    if (previousSpriteFont == null || size > previousSpriteFontSize)
                    {
                        finalSize = keyValuePair.Key;
                        return keyValuePair.Value;
                    }
                }

                previousSpriteFontSize = keyValuePair.Key;
                previousSpriteFont = keyValuePair.Value;
            }

            finalSize = 0;
            return null;
        }

        public Vector2 MeasureString(string str, float size)
        {
            float finalSize;
            SpriteFont spriteFont = GetBestSizeSpriteFont(size, out finalSize);
            float scale = size / finalSize;
            return spriteFont.MeasureString(str) * scale;
        }

        private void loadSpriteFonts(ContentManager contentManager)
        {
            string fontDirectory = contentManager.RootDirectory + "/Fonts/" + Name + "/";
            foreach (string filePath in Directory.EnumerateFiles(fontDirectory, "*.xnb"))
            {
                string fontNameAndSize = filePath.Replace(fontDirectory, "").Replace(".xnb", "");
                string fontSizeString = fontNameAndSize.Replace(Name, "");
                int fontSize = int.Parse(fontSizeString);
                SpriteFont spriteFont = contentManager.Load<SpriteFont>("Fonts/" + Name + "/" + fontNameAndSize);
                spriteFonts.Add(new KeyValuePair<int, SpriteFont>(fontSize, spriteFont));
                if (fontSize > largestSize)
                    largestSize = fontSize;
                if (smallestSize < 0 || fontSize < smallestSize)
                    smallestSize = fontSize;
            }
            spriteFonts.Sort((a, b) => a.Key.CompareTo(b.Key));
        }
        #endregion
    }
}
