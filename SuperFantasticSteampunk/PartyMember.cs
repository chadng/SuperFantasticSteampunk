using System;
using Microsoft.Xna.Framework;
using Spine;

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

        public Weapon EquippedWeapon { get; private set; }
        public Shield EquippedShield { get; private set; }

        public bool Alive
        {
            get { return Health > 0; }
        }

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
            Animation animation = BattleEntity.Skeleton.Data.FindAnimation("battle_stance");
            if (animation != null)
                BattleEntity.AnimationState.SetAnimation(animation, true);
            BattleEntity.Skeleton.RootBone.Tap(b => { b.ScaleX = b.ScaleY = 0.75f; });
            BattleEntity.AnimationState.Time = Game1.Random.Next(100) / 100.0f;
        }

        public void FinishBattle()
        {
            BattleEntity = null;
            EquippedWeapon = null;
            EquippedShield = null;
        }

        public void Kill(Battle battle)
        {
            if (BattleEntity != null)
                BattleEntity.Kill();

            battle.GetPartyBattleLayoutForPartyMember(this).Try(pbl => pbl.RemovePartyMember(this));
            battle.GetPartyForPartyMember(this).Try(pbl => pbl.RemovePartyMember(this));
        }

        public void EquipWeapon(string name)
        {
            EquippedShield = null;
            if (EquippedWeapon == null)
                EquippedWeapon = ResourceManager.GetNewWeapon(name);
        }

        public void EquipShield(string name)
        {
            EquippedWeapon = null;
            if (EquippedShield == null)
                EquippedShield = ResourceManager.GetNewShield(name);
        }

        public void DoDamage(int amount)
        {
            Health -= amount;
            if (Health < 0)
                Health = 0;
        }

        public int CalculateDamageTaken(PartyMember enemy)
        {
            int damageToDo = enemy.calculateFinalAttackStat() * (enemy.criticalHit() ? 2 : 1);
            int damageToBlock = calcuateFinalDefenceStat();
            return Math.Min(damageToDo - damageToBlock, 1);
        }

        public void AddExperience(PartyMember enemy)
        {
            addExperience(calculateExperienceGained(enemy.Level, enemy.Data.ExperienceMultiplier));
        }

        private void addExperience(int amount)
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

        private int calculateExperienceGained(int enemyLevel, int enemyExperienceMultiplier)
        {
            return (int)Math.Round((enemyLevel / (double)Level) * enemyExperienceMultiplier * experienceConstant);
        }

        private int calculateFinalAttackStat()
        {
            //TODO: take into account stat stacking
            int result = 0;
            if (EquippedWeapon == null)
                result = Attack;
            else
            {
                result += EquippedWeapon.Data.Power;
                if (EquippedWeapon.Data.WeaponType == WeaponType.Melee)
                    result += Attack;
                else
                    result += SpecialAttack;
            }
            return result;
        }

        private int calcuateFinalDefenceStat()
        {
            //TODO: take into account stat stacking
            int result = Defence;
            if (EquippedShield != null)
                result += EquippedShield.Data.Defence;
            return result;
        }

        private bool criticalHit()
        {
            return Game1.Random.Next(MaxStat) < Charm;
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
