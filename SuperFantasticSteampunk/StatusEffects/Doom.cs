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
        private ParticleManager particleManager;
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
            particleManager = new ParticleManager(particleTime, TextureData);
        }
        #endregion

        #region Instance Methods
        public override void EndTurnStart(PartyMember partyMember)
        {
            base.EndTurnStart(partyMember);

            if (!Inflictor.Alive)
                Inflictor = null;
             else if (++turns > durationInTurns)
            {
                partyMember.DoDamage(partyMember.Health, true);
                Inflictor = null;
            }

            shudderManager.Reset(partyMember);
            particleManager.Reset();
        }

        public override void EndTurnUpdate(PartyMember partyMember, Delta delta)
        {
            base.EndTurnUpdate(partyMember, delta);
            Fear.UpdateShudder(partyMember, Color.DarkRed, shudderManager, delta);
            particleManager.Update(Inflictor, delta);
        }

        public override bool EndTurnIsFinished()
        {
            return shudderManager.Finished;
        }
        #endregion
    }
}
