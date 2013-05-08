﻿using System;
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
                        string strToAdd = removeOuterBraces(str.Substring(lastColonIndex + 1, i - (lastColonIndex + 1)));
                        result.Add(strToAdd);
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

            result.Add(removeOuterBraces(str.Substring(lastColonIndex + 1)));

            return result;
        }

        private string removeOuterBraces(string str)
        {
            if (str[0] == '{')
            {
                str = str.Substring(1);
                if (str[str.Length - 1] == '}')
                    str = str.Substring(0, str.Length - 1);
            }
            return str;
        }

        private object parseArgumentString(string str)
        {
            int colonIndex = str.IndexOf(':'); // use index instead of Split so that colons can be part of a string
            return ResourceManager.ParseItemDataValue(str.Substring(0, colonIndex), removeOuterBraces(str.Substring(colonIndex + 1)));
        }
        #endregion
    }
}
