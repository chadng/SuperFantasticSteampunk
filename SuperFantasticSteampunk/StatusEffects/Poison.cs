using Microsoft.Xna.Framework;
using SuperFantasticSteampunk.StatusEffects.Utils;

namespace SuperFantasticSteampunk.StatusEffects
{
    class Poison : StatusEffect
    {
        #region Constants
        private const float tintAlphaSpeed = 1.2f;
        private const float bubbleParticleTime = 0.25f;
        #endregion

        #region Instance Fields
        private bool finished;
        private bool damageDone;
        private float tintAlpha;
        private bool incTintAlpha;
        private ParticleManager particleManager;
        #endregion

        #region Instance Properties
        public override StatusEffectType Type
        {
            get { return StatusEffectType.Poison; }
        }

        public override bool Active
        {
            get { return true; }
        }

        public override TextureData TextureData { get; protected set; }
        #endregion

        #region Constructors
        public Poison()
        {
            TextureData = ResourceManager.GetTextureData("particles/poison_bubble");
            particleManager = new ParticleManager(bubbleParticleTime, TextureData);
            resetFieldsForUpdate();
        }
        #endregion

        #region Instance Methods
        public override void EndTurnStart(PartyMember partyMember)
        {
            base.EndTurnStart(partyMember);
            resetFieldsForUpdate();
        }

        public override void EndTurnUpdate(PartyMember partyMember, Delta delta)
        {
            base.EndTurnUpdate(partyMember, delta);

            updateTintAlpha(delta);
            particleManager.Update(partyMember, delta);

            if (!incTintAlpha && !damageDone)
            {
                int damage = partyMember.Health / 8;
                partyMember.DoDamage(damage > 0 ? damage : 1, true);
                damageDone = true;
                if (!partyMember.Alive)
                    finished = true;
            }

            partyMember.BattleEntity.Tint = finished ? Color.White : Color.Lerp(Color.White, Color.Purple, tintAlpha);
        }

        public override bool EndTurnIsFinished()
        {
            return finished;
        }

        private void resetFieldsForUpdate()
        {
            finished = false;
            damageDone = false;
            tintAlpha = 0.0f;
            incTintAlpha = true;
            particleManager.Reset();
        }

        private void updateTintAlpha(Delta delta)
        {
            if (incTintAlpha)
            {
                tintAlpha += tintAlphaSpeed * delta.Time;
                if (tintAlpha >= 1.0f)
                {
                    tintAlpha = 1.0f;
                    incTintAlpha = false;
                }
            }
            else
            {
                tintAlpha -= tintAlphaSpeed * delta.Time;
                if (tintAlpha <= 0.0f)
                {
                    tintAlpha = 0.0f;
                    finished = true;
                }
            }
        }
        #endregion
    }
}
