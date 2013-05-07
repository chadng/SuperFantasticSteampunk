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
            string[] statements = scriptString.Split(';');
            Actions = new List<ScriptAction>(statements.Length);
            foreach (string statement in statements)
                Actions.Add(stringToScriptAction(statement));
            Actions.Sort((a, b) => a.Item1.CompareTo(b.Item1));
        }
        #endregion

        #region Instance Methods
        private ScriptAction stringToScriptAction(string str)
        {
            string[] parts = str.Split(' ');
            float time = float.Parse(parts[0]);
            string function = parts[1];

            List<object> arguments = new List<object>();
            for (int i = 2; i < parts.Length; ++i)
                arguments.Add(parseArgumentString(parts[i]));

            return new ScriptAction(time, function, arguments.ToArray());
        }

        private object parseArgumentString(string str)
        {
            int colonIndex = str.IndexOf(':'); // use index instead of Split so that colons can be part of a string
            return ResourceManager.ParseItemDataValue(str.Substring(0, colonIndex), str.Substring(colonIndex + 1));
        }
        #endregion
    }
}
