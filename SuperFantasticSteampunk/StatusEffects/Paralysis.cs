using Microsoft.Xna.Framework;
using SuperFantasticSteampunk.BattleStates;
using SuperFantasticSteampunk.StatusEffects.Utils;

namespace SuperFantasticSteampunk.StatusEffects
{
    class Paralysis : StatusEffect
    {
        #region Constants
        private const int chanceOfEffect = 25;
        private const int shudderCount = 25;
        private const float shudderTime = 0.01f;
        #endregion

        #region Instance Fields
        private bool finished;
        private bool thinkActionActivationDecided;
        private ShudderManager shudderManager;
        #endregion

        #region Instance Properties
        public override StatusEffectType Type
        {
            get { return StatusEffectType.Paralysis; }
        }

        public override bool Active
        {
            get { return true; }
        }

        public override TextureData TextureData { get; protected set; }
        #endregion

        #region Constructors
        public Paralysis()
        {
            TextureData = ResourceManager.GetTextureData("particles/paralysis");
            shudderManager = new ShudderManager(shudderCount, shudderTime);
            resetFieldsForUpdate(null);
        }
        #endregion

        #region Instance Methods
        public override void BeforeActStart(ThinkAction thinkAction)
        {
            base.BeforeActStart(thinkAction);
            start(thinkAction.Actor);
        }

        public override void BeforeActUpdate(ThinkAction thinkAction, Delta delta)
        {
            base.BeforeActUpdate(thinkAction, delta);

            if (!thinkActionActivationDecided)
            {
                if (Game1.Random.Next(100) <= chanceOfEffect)
                    thinkAction.Active = false;
                thinkActionActivationDecided = true;
            }
            else if (thinkAction.Active)
                finished = true;
            else
                updateShockEffect(thinkAction.Actor, delta);

            checkFinish(thinkAction.Actor);
        }

        public override bool BeforeActIsFinished()
        {
            return finished;
        }

        public override void EndTurnStart(PartyMember partyMember)
        {
            base.EndTurnStart(partyMember);
            start(partyMember);
        }

        public override void EndTurnUpdate(PartyMember partyMember, Delta delta)
        {
            base.EndTurnUpdate(partyMember, delta);
            updateShockEffect(partyMember, delta);
            checkFinish(partyMember);
        }

        public override bool EndTurnIsFinished()
        {
            return finished;
        }

        private void start(PartyMember partyMember)
        {
            resetFieldsForUpdate(partyMember);
        }

        private void updateShockEffect(PartyMember partyMember, Delta delta)
        {
            shudderManager.Update(partyMember, delta);
            if (shudderManager.Finished)
                finished = true;
            bool shocked = shudderManager.ShudderCounter % 2 == 0;
            partyMember.BattleEntity.Position = shudderManager.PartyMemberStartPosition + (new Vector2(5.0f, 0.0f) * (shocked ? -1 : 1));
            partyMember.BattleEntity.Tint = shocked ? Color.Yellow : Color.White;
            partyMember.BattleEntity.PauseAnimation = true;
        }

        private void checkFinish(PartyMember partyMember)
        {
            if (finished)
            {
                partyMember.BattleEntity.Tint = Color.White;
                partyMember.BattleEntity.PauseAnimation = false;
            }
        }

        private void resetFieldsForUpdate(PartyMember partyMember)
        {
            finished = false;
            thinkActionActivationDecided = false;
            shudderManager.Reset(partyMember);
        }
        #endregion
    }
}
