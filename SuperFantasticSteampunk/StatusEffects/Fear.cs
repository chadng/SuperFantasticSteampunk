using Microsoft.Xna.Framework;
using SuperFantasticSteampunk.StatusEffects.Utils;

namespace SuperFantasticSteampunk.StatusEffects
{
    class Fear : StatusEffect, IInflictable
    {
        #region Constants
        private const int durationInTurns = 3;
        private const int shudderCount = 3;
        private const float shudderTime = 0.4f;
        private const float shudderDistance = 30.0f;
        private const float particleTime = 0.25f;
        #endregion

        #region Static Methods
        public static void UpdateShudder(PartyMember partyMember, Color tint, ShudderManager shudderManager, Delta delta)
        {
            shudderManager.Update(partyMember, delta);
            if (shudderManager.Finished)
            {
                partyMember.BattleEntity.Tint = Color.White;
                partyMember.BattleEntity.PauseAnimation = false;
            }
            else
            {
                float shudderPercentage = shudderManager.ShudderTimer / shudderManager.ShudderTime;
                if (shudderManager.ShudderCounter % 2 != 0)
                    shudderPercentage = 1.0f - shudderPercentage;
                partyMember.BattleEntity.Position = shudderManager.PartyMemberStartPosition + new Vector2(shudderPercentage * shudderDistance, 0.0f);
                partyMember.BattleEntity.Tint = tint;
                partyMember.BattleEntity.PauseAnimation = true;
            }
        }
        #endregion

        #region Instance Fields
        private int turns;
        private ShudderManager shudderManager;
        private ParticleManager particleManager;
        private bool beforeActFinished;
        private bool endTurnFinishedEarly;
        #endregion

        #region Instance Properties
        public PartyMember Inflictor { get; private set; }

        public override StatusEffectType Type
        {
            get { return StatusEffectType.Fear; }
        }

        public override bool Active
        {
            get { return turns <= durationInTurns; }
        }

        public override TextureData TextureData { get; protected set; }
        #endregion

        #region Constructors
        public Fear(PartyMember inflictor)
        {
            Inflictor = inflictor;
            turns = 0;
            TextureData = ResourceManager.GetTextureData("particles/fear");
            shudderManager = new ShudderManager(shudderCount, shudderTime);
            particleManager = new ParticleManager(particleTime, TextureData);
            beforeActFinished = false;
            endTurnFinishedEarly = false;
        }
        #endregion

        #region Instance Methods
        public override void BeforeActStart(BattleStates.ThinkAction thinkAction)
        {
            base.BeforeActStart(thinkAction);
            if (thinkAction.Target == Inflictor)
            {
                thinkAction.Active = false;
                beforeActFinished = false;
                shudderManager.Reset(thinkAction.Actor);
                particleManager.Reset();
            }
            else
                beforeActFinished = true;
        }

        public override void BeforeActUpdate(BattleStates.ThinkAction thinkAction, Delta delta)
        {
            if (beforeActFinished)
                return;
            base.BeforeActUpdate(thinkAction, delta);
            update(thinkAction.Actor, delta);
            if (shudderManager.Finished)
                beforeActFinished = true;
        }

        public override bool BeforeActIsFinished()
        {
            return beforeActFinished;
        }

        public override void EndTurnStart(PartyMember partyMember)
        {
            base.EndTurnStart(partyMember);
            if (Game1.Random.Next(durationInTurns) < turns)
                turns = durationInTurns + 1;
            else
                ++turns;

            if (Active)
            {
                endTurnFinishedEarly = false;
                shudderManager.Reset(partyMember);
                particleManager.Reset();
            }
            else
                endTurnFinishedEarly = true;
        }

        public override void EndTurnUpdate(PartyMember partyMember, Delta delta)
        {
            if (endTurnFinishedEarly)
                return;
            base.EndTurnUpdate(partyMember, delta);
            update(partyMember, delta);
        }

        public override bool EndTurnIsFinished()
        {
            return endTurnFinishedEarly || shudderManager.Finished;
        }

        private void update(PartyMember partyMember, Delta delta)
        {
            UpdateShudder(partyMember, Color.Gray, shudderManager, delta);
            particleManager.Update(Inflictor, delta);
        }
        #endregion
    }
}
