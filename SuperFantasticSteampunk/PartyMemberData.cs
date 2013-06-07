using System.Collections.Generic;

namespace SuperFantasticSteampunk
{
    enum CharacterClass { Warrior, Marksman, Thief, Medic, Enemy }
    enum OverworldMovementType { Wander, Follow, Run }

    class PartyMemberData // Represents base stats for a PartyMember
    {
        #region Instance Properties
        public string Name { get; private set; }
        public CharacterClass CharacterClass { get; private set; }
        public int MaxHealth { get; private set; }
        public int Speed { get; private set; }
        public string OverworldSpriteName { get; private set; }
        public string OverworldSkeletonName { get; private set; }
        public string BattleSkeletonName { get; private set; }
        public string BattleSkeletonSkinName { get; private set; }
        public float BattleAltitude { get; private set; }
        public string BattleShadowFollowBoneName { get; private set; }
        public OverworldMovementType OverworldMovementType { get; private set; }
        public string DefaultWeapons { get; private set; }
        public string DefaultShields { get; private set; }
        public string DefaultItems { get; private set; }
        #endregion

        #region Instance Methods
        public Dictionary<string, int> DefaultWeaponsToDictionary()
        {
            return stringToDictionary(DefaultWeapons);
        }

        public Dictionary<string, int> DefaultShieldsToDictionary()
        {
            return stringToDictionary(DefaultShields);
        }

        public Dictionary<string, int> DefaultItemsToDictionary()
        {
            return stringToDictionary(DefaultItems);
        }

        private Dictionary<string, int> stringToDictionary(string str)
        {
            if (str == null || str.Length == 0)
                return new Dictionary<string, int>();

            string[] items = str.TrimEnd(';').Split(';');
            Dictionary<string, int> result = new Dictionary<string, int>(items.Length);
            foreach (string item in items)
            {
                string[] parts = item.Split(':');
                result.Add(parts[0], int.Parse(parts[1]));
            }
            return result;
        }
        #endregion
    }
}
