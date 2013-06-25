using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk
{
    class Trap : Entity
    {
        #region Instance Fields
        private Attributes attributes;
        private PartyMember setter;
        private Battle battle;
        #endregion

        #region Instance Fields
        public WeaponData Data { get; private set; }
        #endregion

        #region Constructors
        public Trap(Sprite sprite, Vector2 position, Attributes attributes, WeaponData weaponData, PartyMember setter, Battle battle)
            : base(sprite, position)
        {
            if (attributes == null)
                throw new Exception("Attributes cannot be null");
            if (setter == null)
                throw new Exception("PartyMember setter cannot be null");
            if (battle == null)
                throw new Exception("Battle cannot be null");

            this.attributes = attributes;
            this.setter = setter;
            this.battle = battle;
            Data = weaponData;
        }
        #endregion

        #region Instance Methods
        public void Trigger(PartyMember partyMember, PartyBattleLayout partyBattleLayout)
        {
            int damage = partyMember.CalculateDamageTaken(this);
            if (damage > 0)
                partyMember.DoDamage(damage, true, false);
            if (partyMember.Alive)
                partyMember.ApplyStatusEffectsFromAttributes(setter, attributes);

            if (attributes.Enhancement == Enhancement.Explosive)
            {
                ParticleEffect.AddExplosion(GetCenter(), battle);
                List<PartyMember> partyMemberList = partyBattleLayout.GetListBehindTrap(this);
                if (partyMemberList != null && partyMemberList.Count > 0)
                {
                    damage = partyMemberList[0].CalculateDamageTaken(this);
                    partyMemberList[0].DoDamage(damage, false);
                }
            }

            partyBattleLayout.RemoveTrap(this);
            Kill();
        }
        #endregion
    }
}
