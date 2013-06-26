using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk
{
    class Trap : ItemWithAttributes
    {
        #region Instance Fields
        private PartyMember setter;
        private Battle battle;
        private Entity entity;
        #endregion

        #region Instance Fields
        public WeaponData Data { get; private set; }
        #endregion

        #region Constructors
        public Trap(Sprite sprite, Vector2 position, Attributes attributes, WeaponData weaponData, PartyMember setter, Battle battle)
            : base(attributes)
        {
            if (attributes == null)
                throw new Exception("Attributes cannot be null");
            if (setter == null)
                throw new Exception("PartyMember setter cannot be null");
            if (battle == null)
                throw new Exception("Battle cannot be null");

            this.setter = setter;
            this.battle = battle;
            Data = weaponData;
            entity = new Entity(sprite, position);
            entity.UpdateExtensions.Add(new UpdateExtension((updateExtension, delta) => {
                Update(delta);
            }));
            Scene.AddEntity(entity);
        }
        #endregion

        #region Instance Methods
        public void Trigger(PartyMember partyMember, PartyBattleLayout partyBattleLayout)
        {
            int damage = partyMember.CalculateDamageTaken(this);
            if (damage > 0)
                partyMember.DoDamage(damage, true, false);
            if (partyMember.Alive)
                partyMember.ApplyStatusEffectsFromAttributes(setter, Attributes, battle);

            if (Attributes.Enhancement == Enhancement.Explosive)
            {
                ParticleEffect.AddExplosion(entity.GetCenter(), battle);
                List<PartyMember> partyMemberList = partyBattleLayout.GetListBehindTrap(this);
                if (partyMemberList != null && partyMemberList.Count > 0)
                {
                    PartyMember frontPartyMember = partyMemberList[0];
                    damage = frontPartyMember.CalculateDamageTaken(this);
                    frontPartyMember.DoDamage(damage, false);
                    if (frontPartyMember.Alive)
                        frontPartyMember.ApplyStatusEffectsFromAttributes(setter, Attributes, battle);
                }
            }
            else
                ParticleEffect.AddSmokePuff(entity.GetCenter(), battle);

            partyBattleLayout.RemoveTrap(this);
            Kill();
        }

        public void Kill()
        {
            entity.Kill();
        }

        protected override void emitStatusParticle(particleEmissionCallback callback)
        {
            Rectangle boundingBox = entity.GetBoundingBox();
            float x = boundingBox.X + Game1.Random.Next(boundingBox.Width);
            float y = boundingBox.Y + Game1.Random.Next(boundingBox.Height);
            callback(x, y);
        }

        protected override void updateAffiliationTint(Color color, float alpha)
        {
            entity.Tint = Color.Lerp(Color.White, color, alpha);
        }
        #endregion
    }
}
