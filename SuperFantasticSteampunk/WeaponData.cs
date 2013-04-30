using System.Collections.Generic;

namespace SuperFantasticSteampunk
{
    enum WeaponType { Melee, Ranged }

    class WeaponData
    {
        public string Name { get; private set; }
        public WeaponType WeaponType { get; private set; }
        public CharacterClass CharacterClass { get; private set; }
        public int Power { get; private set; }

        public WeaponData(Dictionary<string, object> properties)
        {
            foreach (KeyValuePair<string, object> pair in properties)
            {
                switch (pair.Key)
                {
                case "Name": Name = (string)pair.Value; break;
                case "WeaponType": WeaponType = (WeaponType)pair.Value; break;
                case "CharacterClass": CharacterClass = (CharacterClass)pair.Value; break;
                case "Power": Power = (int)pair.Value; break;
                }
            }
        }
    }
}
