using System;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk
{
    class PartyMember
    {
        #region Constants
        public int MaxStat = 250;
        public int MaxLevel = 99;
        public int ExperienceNeededToLevelUp = 100;
        private double experienceConstant = 10.0;
        #endregion

        #region Instance Properties
        public PartyMemberData Data { get; private set; }
        public int MaxHealth { get; private set; }
        public int Health { get; private set; }
        public int Attack { get; private set; }
        public int SpecialAttack { get; private set; }
        public int Defence { get; private set; }
        public int Speed { get; private set; }
        public int Charm { get; private set; }
        public int Level { get; private set; }
        public int Experience { get; private set; }
        public Entity BattleEntity { get; private set; }

        public CharacterClass CharacterClass
        {
            get { return Data.CharacterClass; }
        }
        #endregion

        #region Constructors
        public PartyMember(PartyMemberData partyMemberData)
        {
            if (partyMemberData == null)
                throw new Exception("PartyMemberData cannot be null");
            Data = partyMemberData;
            BattleEntity = null;
            Level = 5;
            resetStatsFromLevel();
        }
        #endregion

        #region Instance Methods
        public void StartBattle()
        {
            BattleEntity = new Entity(ResourceManager.GetNewSkeleton(Data.SkeletonName), new Vector2());
            BattleEntity.Skeleton.SetSkin(Data.SkeletonSkinName);
            BattleEntity.Skeleton.SetSlotsToBindPose();
        }

        public void FinishBattle()
        {
            BattleEntity = null;
        }

        public void DoDamage(int amount)
        {
            Health -= amount;
            if (Health < 0)
                Health = 0;
        }

        public void AddExperience(int amount)
        {
            Experience += amount;
            while (Experience >= ExperienceNeededToLevelUp)
            {
                ++Level;
                Experience -= ExperienceNeededToLevelUp;
            }
            if (Level > MaxLevel)
            {
                Level = MaxLevel;
                Experience = ExperienceNeededToLevelUp;
            }
            resetStatsFromLevel();
        }

        public void AddExperience(PartyMember enemy)
        {
            AddExperience(calculateExperienceGained(enemy.Level, enemy.Data.ExperienceMultiplier));
        }

        private int calculateExperienceGained(int enemyLevel, int enemyExperienceMultiplier)
        {
            return (int)Math.Round((enemyLevel / (double)Level) * enemyExperienceMultiplier * experienceConstant);
        }

        private void resetStatsFromLevel()
        {
            MaxHealth = (int)Math.Round(Level * (Data.MaxHealth / (double)MaxLevel));
            Attack = (int)Math.Round(Level * (Data.Attack / (double)MaxLevel));
            SpecialAttack = (int)Math.Round(Level * (Data.SpecialAttack / (double)MaxLevel));
            Defence = (int)Math.Round(Level * (Data.Defence / (double)MaxLevel));
            Speed = (int)Math.Round(Level * (Data.Speed / (double)MaxLevel));
            Charm = (int)Math.Round(Level * (Data.Charm / (double)MaxLevel));
            Health = MaxHealth;
        }
        #endregion
    }
}
