using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;
using SuperFantasticSteampunk.BattleStates;

namespace SuperFantasticSteampunk
{
    class PartyMember
    {
        #region Instance Fields
        private List<StatModifier> statModifiers;
        private string battleEntityIdleAnimationNameOverride;
        private bool hasIdleWeaponAnimation;
        private bool hasIdleShieldAnimation;
        private bool hasHurtWeaponAnimation;
        private bool hasHurtShieldAnimation;
        #endregion

        #region Instance Properties
        public PartyMemberData Data { get; private set; }

        public string Name { get; private set; }

        public int MaxHealth { get; private set; }
        public int Health { get; private set; }
        public int Speed { get; private set; }

        public List<StatusEffect> StatusEffects { get; private set; }

        public Entity BattleEntity { get; private set; }
        public Entity OverworldEntity { get; private set; }

        public Weapon EquippedWeapon { get; private set; }
        public Shield EquippedShield { get; private set; }

        public bool HurtThisTurn { get; private set; }

        public bool Alive
        {
            get { return Health > 0; }
        }

        public CharacterClass CharacterClass
        {
            get { return Data.CharacterClass; }
        }

        public Vector2 BattleEntityIdlePosition { get; set; }
        public string BattleEntityIdleAnimationName
        {
            get { return battleEntityIdleAnimationNameOverride ?? (hasIdleWeaponAnimation && EquippedWeapon != null ? "idle_weapon" : (hasIdleShieldAnimation && EquippedShield != null ? "idle_shield" : "idle")); }
        }
        public string BattleEntityHurtAnimationName
        {
            get { return hasHurtWeaponAnimation && EquippedWeapon != null ? "hurt_weapon" : (hasHurtShieldAnimation && EquippedShield != null ? "hurt_shield" : "hurt"); }
        }
        #endregion

        #region Constructors
        public PartyMember(PartyMemberData partyMemberData)
        {
            if (partyMemberData == null)
                throw new Exception("PartyMemberData cannot be null");
            Data = partyMemberData;
            statModifiers = new List<StatModifier>();
            StatusEffects = new List<StatusEffect>();
            BattleEntity = null;
            BattleEntityIdlePosition = Vector2.Zero;
            battleEntityIdleAnimationNameOverride = null;
            generateName();
            resetStats();
        }
        #endregion

        #region Instance Methods
        public void StartOverworld(Vector2 entityPosition)
        {
            if (Data.OverworldSpriteName != null && Data.OverworldSpriteName.Length > 0)
                OverworldEntity = new Entity(ResourceManager.GetNewSprite(Data.OverworldSpriteName), entityPosition);
            else
            {
                OverworldEntity = new Entity(ResourceManager.GetNewSkeleton(Data.OverworldSkeletonName), entityPosition);
                Rectangle customBoundingBox = OverworldEntity.GetBoundingBoxAt(Vector2.Zero);
                customBoundingBox.Height /= 4;
                customBoundingBox.Y += customBoundingBox.Height * 3;
                OverworldEntity.CustomBoundingBox = customBoundingBox;
                OverworldEntity.Scale = new Vector2(0.3f);
            }
        }

        public void FinishOverworld()
        {
            OverworldEntity = null;
        }
        
        public void StartBattle()
        {
            statModifiers.Clear();
            StatusEffects.Clear();
            calculateStatsFromModifiers();

            BattleEntity = new Entity(ResourceManager.GetNewSkeleton(Data.BattleSkeletonName), new Vector2());
            BattleEntity.Skeleton.SetSkin(Data.BattleSkeletonSkinName);
            BattleEntity.Scale = new Vector2(0.6f);
            BattleEntity.Altitude = Data.BattleAltitude;
            if (Data.BattleShadowFollowBoneName != null && Data.BattleShadowFollowBoneName.Length > 0)
                BattleEntity.ShadowFollowBone = BattleEntity.Skeleton.FindBone(Data.BattleShadowFollowBoneName);

            updateBattleEntitySkeleton();

            HurtThisTurn = false;

            hasIdleWeaponAnimation = BattleEntity.Skeleton.Data.FindAnimation("idle_weapon") != null;
            hasIdleShieldAnimation = BattleEntity.Skeleton.Data.FindAnimation("idle_shield") != null;
            hasHurtWeaponAnimation = BattleEntity.Skeleton.Data.FindAnimation("hurt_weapon") != null;
            hasHurtShieldAnimation = BattleEntity.Skeleton.Data.FindAnimation("hurt_shield") != null;
            BattleEntity.AnimationState.SetAnimation(BattleEntityIdleAnimationName, true);
            BattleEntity.AnimationState.Time = (float)Game1.Random.NextDouble();
        }

        public void FinishBattle()
        {
            statModifiers.Clear();
            StatusEffects.Clear();
            BattleEntity = null;
            HurtThisTurn = false;
        }

        public void EndTurn()
        {
            for (int i = StatusEffects.Count - 1; i >= 0; --i)
            {
                if (!StatusEffects[i].Active)
                    StatusEffects.RemoveAt(i);
            }
            for (int i = statModifiers.Count - 1; i >= 0; --i)
            {
                statModifiers[i].DecrementTurnsLeft();
                if (!statModifiers[i].Active)
                    statModifiers.RemoveAt(i);
            }
            calculateStatsFromModifiers();
            HurtThisTurn = false;
        }

        public void Kill(Battle battle)
        {
            if (BattleEntity != null)
                BattleEntity.Kill();

            battle.GetPartyBattleLayoutForPartyMember(this).Try(pbl => pbl.RemovePartyMember(this));
            battle.GetPartyForPartyMember(this).Try(pbl => pbl.RemovePartyMember(this));
        }

        public void AddStatModifier(StatModifier statModifier)
        {
            statModifiers.Add(statModifier);
            calculateStatsFromModifiers();
        }

        public void AddStatusEffect(StatusEffect statusEffect)
        {
            if (!HasStatusEffect(statusEffect.Type))
                StatusEffects.Add(statusEffect);
        }

        public void ApplyStatusEffectsFromAttributes(PartyMember inflictor, Attributes attributes, Battle battle)
        {
            bool sameParty = battle.GetPartyForPartyMember(inflictor) == battle.GetPartyForPartyMember(this);

            StatusEffect statusEffect;
            switch (attributes.Status)
            {
            case Status.Poisonous: statusEffect = new StatusEffects.Poison(); break;
            case Status.Shocking: statusEffect = new StatusEffects.Paralysis(); break;
            case Status.Scary: statusEffect = sameParty ? null : new StatusEffects.Fear(inflictor); break;
            default: statusEffect = null; break;
            }
            if (statusEffect != null)
                AddStatusEffect(statusEffect);

            if (attributes.Affiliation == Affiliation.Doom && !sameParty)
                AddStatusEffect(new StatusEffects.Doom(inflictor));
        }

        public void ForEachStatusEffect(Action<StatusEffect> action)
        {
            StatusEffects.ForEach(action);
        }

        public bool HasStatusEffect(StatusEffectType statusEffectType)
        {
            foreach (StatusEffect statusEffect in StatusEffects)
            {
                if (statusEffect.Type == statusEffectType)
                    return true;
            }
            return false;
        }

        public bool FearsPartyMember(PartyMember other)
        {
            foreach (StatusEffect statusEffect in StatusEffects)
            {
                if (statusEffect.Type == StatusEffectType.Fear)
                {
                    if ((statusEffect as StatusEffects.Fear).Inflictor == other)
                        return true;
                }
            }
            return false;
        }

        public void EquipWeapon(string name)
        {
            finishEquipment();
            EquippedShield = null;
            if (name == null)
                EquippedWeapon = null;
            else
                EquippedWeapon = ResourceManager.GetNewWeapon(name);
            updateBattleEntitySkeleton();
            setBattleEntityIdleAnimation();
        }

        public void EquipDefaultWeapon(Party party)
        {
            foreach (KeyValuePair<string, int> inventoryItem in party.WeaponInventories[CharacterClass])
            {
                if (inventoryItem.Value < 0)
                {
                    EquipWeapon(inventoryItem.Key);
                    if (EquippedWeapon != null)
                        break;
                }
            }
            setBattleEntityIdleAnimation();
        }

        public void EquipShield(string name)
        {
            finishEquipment();
            EquippedWeapon = null;
            if (name == null)
                EquippedShield = null;
            else
                EquippedShield = ResourceManager.GetNewShield(name);
            updateBattleEntitySkeleton();
            setBattleEntityIdleAnimation();
        }

        public void DoDamage(int amount, bool ignoreShield, bool playAnimation = true)
        {
            if (amount > 0 && !ignoreShield)
                HurtThisTurn = true;

            bool aliveBefore = Alive;
            bool heal = amount < 0;
            if (heal)
            {
                int finalHealth = Health - amount;
                if (finalHealth > MaxHealth)
                    amount = -(MaxHealth - Health);
            }
            else if (amount > Health)
                amount = Health;
            Health -= amount;

            Battle battle = Scene.Current as Battle;
            if (Health <= 0)
            {
                Health = 0;
                if (aliveBefore)
                {
                    BattleEntity.Visible = false;
                    if (battle != null)
                        ParticleEffect.AddSmokePuff(BattleEntity.GetCenter(), battle);
                }
            }
            else if (Health > MaxHealth)
                Health = MaxHealth;

            if (battle != null)
            {
                if (amount > 0 && playAnimation)
                {
                    BattleEntity.AnimationState.SetAnimation(BattleEntityHurtAnimationName, false);
                    BattleEntity.AnimationState.AddAnimation(BattleEntityIdleAnimationName, true);
                }
                Scene.AddEntity(new FloatingText(Math.Abs(amount).ToString(), heal ? Color.Blue : Color.Red, BattleEntity.GetCenter(), 5.0f, true));
            }
        }

        public int CalculateDamageTaken(PartyMember enemy)
        {
            return calculateDamageTaken(enemy.calculateFinalAttackStat());
        }

        public int CalculateDamageTaken(Trap trap)
        {
            return calculateDamageTaken(trap.Data.Power);
        }

        public void SetBattleEntityIdleAnimationNameOverride(string animationName)
        {
            if (animationName == null || animationName.Length == 0)
                battleEntityIdleAnimationNameOverride = null;
            else if (BattleEntity != null && BattleEntity.Skeleton.Data.FindAnimation(animationName) == null)
                battleEntityIdleAnimationNameOverride = null;
            else
                battleEntityIdleAnimationNameOverride = animationName;
        }

        public void Update(Delta delta)
        {
            if (EquippedWeapon != null)
                EquippedWeapon.Update(delta);
            else if (EquippedShield != null)
                EquippedShield.Update(delta);
        }

        private int calculateFinalAttackStat()
        {
            if (EquippedWeapon == null)
                return 0;

            int result = 0;
            result += EquippedWeapon.Data.Power;
            result = modifyStatFromAttributes(result, EquippedWeapon.Attributes);

            foreach (StatModifier statModifier in statModifiers)
                result += EquippedWeapon.Data.WeaponType == WeaponType.Ranged ? statModifier.RangedAttack : statModifier.MeleeAttack;
            return result;
        }

        private int calculateDamageTaken(int damageToDo)
        {
            int damageToBlock = calcuateFinalDefenceStat();
            int damage = damageToDo - damageToBlock;
            return damage < 0 ? 0 : damage;
        }

        private int calcuateFinalDefenceStat()
        {
            int result = 0;
            if (EquippedShield != null)
            {
                result += EquippedShield.Data.Defence;
                result = modifyStatFromAttributes(result, EquippedShield.Attributes);
            }
            foreach (StatModifier statModifier in statModifiers)
                result += statModifier.Defence;
            return result;
        }

        private int modifyStatFromAttributes(int stat, Attributes attributes)
        {
            int modifier = (int)Math.Ceiling(stat * 0.2f);

            if (attributes.Handling == Handling.Light)
                stat -= modifier;
            else if (attributes.Handling == Handling.Heavy)
                stat += modifier;

            if ((attributes.Affiliation == Affiliation.Light && Clock.IsDay) || (attributes.Affiliation == Affiliation.Darkness && Clock.IsNight))
                stat += modifier;

            return stat;
        }

        private void resetStats()
        {
            MaxHealth = Data.MaxHealth;
            Speed = Data.Speed;
            Health = MaxHealth;
        }

        private void calculateStatsFromModifiers()
        {
            MaxHealth = Data.MaxHealth;
            Speed = Data.Speed;

            foreach (StatModifier statModifier in statModifiers)
            {
                MaxHealth += (int)(Data.MaxHealth * statModifier.MaxHealth);
                Speed += (int)(Data.Speed * statModifier.Speed);
            }

            if (Health > MaxHealth)
                Health = MaxHealth;
        }

        private void updateBattleEntitySkeleton()
        {
            if (BattleEntity == null)
                return;

            if (EquippedWeapon != null && EquippedWeapon.TextureData != null)
            {
                BattleEntity.SetSkeletonAttachment("weapon", EquippedWeapon.Data.Name, EquippedWeapon.TextureData);
                EquippedWeapon.SkeletonRegionAttachment = BattleEntity.Skeleton.GetAttachment("weapon", EquippedWeapon.Data.Name) as RegionAttachment;
                EquippedWeapon.SkeletonBone = BattleEntity.Skeleton.FindBone("weapon");
                EquippedWeapon.SkeletonSlot = BattleEntity.Skeleton.FindSlot("weapon");
            }
            else
                BattleEntity.SetSkeletonAttachment("weapon", "none", forceNoTextureData: true);

            if (EquippedShield != null && EquippedShield.TextureData != null)
            {
                BattleEntity.SetSkeletonAttachment("shield", EquippedShield.Data.Name, EquippedShield.TextureData);
                EquippedShield.SkeletonRegionAttachment = BattleEntity.Skeleton.GetAttachment("shield", EquippedShield.Data.Name) as RegionAttachment;
                EquippedShield.SkeletonBone = BattleEntity.Skeleton.FindBone("shield");
                EquippedShield.SkeletonSlot = BattleEntity.Skeleton.FindSlot("shield");
            }
            else
                BattleEntity.SetSkeletonAttachment("shield", "none", forceNoTextureData: true);
        }

        private void setBattleEntityIdleAnimation()
        {
            if (BattleEntity != null)
            {
                string animationName = BattleEntityIdleAnimationName;
                if (animationName != BattleEntity.AnimationState.Animation.Name)
                    BattleEntity.AnimationState.SetAnimation(animationName, true);
            }
        }

        private void generateName()
        {
            if (Data.CharacterClass == CharacterClass.Enemy)
                Name = Data.Name;
            else
                Name = ResourceManager.PartyMemberTitles.Sample() + " " + ResourceManager.PartyMemberForenames.Sample() + " " + ResourceManager.PartyMemberSurnames.Sample();
        }

        private void finishEquipment()
        {
            if (EquippedShield != null)
                EquippedShield.Finish();
            if (EquippedWeapon != null)
                EquippedWeapon.Finish();
        }
        #endregion
    }
}
