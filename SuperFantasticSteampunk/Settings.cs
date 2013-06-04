using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace SuperFantasticSteampunk
{
    class Settings
    {
        #region Instance Fields
        private SortedDictionary<string, object> settingsDictionary;
        #endregion

        #region Constructors
        public Settings(string fileName, ContentManager contentManager)
        {
            settingsDictionary = new SortedDictionary<string, object>();
            loadSettingsFromFile(fileName, contentManager);
        }
        #endregion

        #region Instance Methods
        public T GetSetting<T>(string settingName)
        {
            object setting;
            if (settingsDictionary.TryGetValue(settingName, out setting))
                return (T)setting;
            return default(T);
        }

        private void loadSettingsFromFile(string fileName, ContentManager contentManager)
        {
            settingsDictionary.Clear();
            string filePath = contentManager.RootDirectory + "/" + fileName;
            if (File.Exists(filePath))
            {
                string[] fileLines = File.ReadAllLines(filePath);
                foreach (string line in fileLines)
                {
                    KeyValuePair<string, object> result = parseSettingLine(line);
                    if (result.Value != null)
                        settingsDictionary.Add(result.Key, result.Value);
                }
            }
            else
                Logger.Log("File '" + fileName + "' does not exist");
        }

        private KeyValuePair<string, object> parseSettingLine(string line)
        {
            string resultKey = null;
            object resultValue = null;

            string[] parts = line.Split(':');
            if (parts.Length == 2)
            {

                switch (parts[0])
                {
                case "screen_width":
                case "screen_height":
                    resultValue = ResourceManager.ParseItemData<int>(parts[1]);
                    break;
                case "fullscreen":
                    resultValue = ResourceManager.ParseItemData<bool>(parts[1]);
                    break;
                }

                if (resultValue != null)
                    resultKey = parts[0];
            }

            return new KeyValuePair<string, object>(resultKey, resultValue);
        }
        #endregion
    }
}
