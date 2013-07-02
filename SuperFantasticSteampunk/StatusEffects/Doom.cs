using Microsoft.Xna.Framework;
using SuperFantasticSteampunk.StatusEffects.Utils;

namespace SuperFantasticSteampunk.StatusEffects
{
    class Doom : StatusEffect, IInflictable
    {
        #region Constants
        private const int durationInTurns = 3;
        private const int shudderCount = 3;
        private const float shudderTime = 0.4f;
        private const float particleTime = 0.25f;
        #endregion

        #region Instance Fields
        private int turns;
        private ShudderManager shudderManager;
        private ParticleManager doomParticleManager;
        private ParticleManager textParticleManager;
        #endregion

        #region Instance Properties
        public PartyMember Inflictor { get; private set; }

        public override StatusEffectType Type
        {
            get { return StatusEffectType.Doom; }
        }

        public override bool Active
        {
            get { return Inflictor != null; }
        }

        public override TextureData TextureData { get; protected set; }
        #endregion

        #region Constructors
        public Doom(PartyMember inflictor)
        {
            Inflictor = inflictor;
            turns = 0;
            TextureData = ResourceManager.GetTextureData("particles/doom");
            shudderManager = new ShudderManager(shudderCount, shudderTime);
            doomParticleManager = new ParticleManager(particleTime, TextureData);
            textParticleManager = null;
        }
        #endregion

        #region Instance Methods
        public override void EndTurnStart(PartyMember partyMember)
        {
            base.EndTurnStart(partyMember);

            textParticleManager = new ParticleManager(particleTime, (durationInTurns - turns).ToString(), Color.LightBlue);

            if (!Inflictor.Alive)
                Inflictor = null;

            shudderManager.Reset(partyMember);
            doomParticleManager.Reset();
            textParticleManager.Reset();
        }

        public override void EndTurnUpdate(PartyMember partyMember, Delta delta)
        {
            base.EndTurnUpdate(partyMember, delta);
            Fear.UpdateShudder(partyMember, Color.DarkRed, shudderManager, delta);
            doomParticleManager.Update(Inflictor, delta);
            textParticleManager.Update(partyMember, delta);

            if (Inflictor.Alive && shudderManager.Finished)
            {
                if (++turns > durationInTurns)
                {
                    partyMember.DoDamage(partyMember.Health, true);
                    Inflictor = null;
                }
            }
        }

        public override bool EndTurnIsFinished()
        {
            return shudderManager.Finished || !Inflictor.Alive;
        }
        #endregion
    }
}
