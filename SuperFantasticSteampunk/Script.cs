using System;
using System.Collections.Generic;

namespace SuperFantasticSteampunk
{
    using ScriptAction = Tuple<float, string, object[]>;

    class Script
    {
        #region Instance Fields
        public List<ScriptAction> Actions { get; private set; }
        #endregion

        #region Constructors
        public Script(string scriptString)
        {
            if (scriptString[0] == '{')
                scriptString = scriptString.Substring(1);
            if (scriptString[scriptString.Length - 1] == '}')
                scriptString = scriptString.Substring(0, scriptString.Length - 1);

            List<string> statements = splitString(scriptString, ';');
            Actions = new List<ScriptAction>(statements.Count);
            foreach (string statement in statements)
                Actions.Add(stringToScriptAction(statement));
            Actions.Sort((a, b) => a.Item1.CompareTo(b.Item1));
        }
        #endregion

        #region Instance Methods
        private ScriptAction stringToScriptAction(string str)
        {
            List<string> parts = splitString(str, ' ');
            float time = float.Parse(parts[0]);
            string function = parts[1];

            List<object> arguments = new List<object>();
            for (int i = 2; i < parts.Count; ++i)
                arguments.Add(parseArgumentString(parts[i]));

            return new ScriptAction(time, function, arguments.ToArray());
        }

        private List<string> splitString(string str, char separator)
        {
            List<string> result = new List<string>();

            int lastColonIndex = -1;
            int braceDepth = 0;
            for (int i = 0; i < str.Length; ++i)
            {
                if (braceDepth == 0)
                {
                    if (str[i] == separator)
                    {
                        result.Add(str.Substring(lastColonIndex + 1, i - (lastColonIndex + 1)));
                        lastColonIndex = i;
                        continue;
                    }
                }

                if (str[i] == '{')
                    ++braceDepth;
                else if (str[i] == '}')
                    --braceDepth;
            }

            if (braceDepth < 0)
                throw new Exception("Too many closing braces in Script");
            if (braceDepth > 0)
                throw new Exception("Too many opening braces in Script");

            result.Add(str.Substring(lastColonIndex + 1));

            return result;
        }

        private object parseArgumentString(string str)
        {
            int colonIndex = str.IndexOf(':'); // use index instead of Split so that colons can be part of a string
            return ResourceManager.ParseItemDataValue(str.Substring(0, colonIndex), str.Substring(colonIndex + 1));
        }
        #endregion
    }
}
