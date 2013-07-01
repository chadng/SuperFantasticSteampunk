using System;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.BattleStates
{
    class HandleStatusEffects : BattleState
    {
        #region Instance Fields
        private ThinkAction thinkAction;
        private PartyMember partyMember;
        private StatusEffectEvent statusEffectEvent;
        private int currentStatusEffectIndex;
        #endregion

        #region Constructors
        public HandleStatusEffects(Battle battle, StatusEffectEvent statusEffectEvent, ThinkAction thinkAction = null, PartyMember partyMember = null)
            : base(battle)
        {
            if (thinkAction == null && partyMember == null)
                throw new Exception("Either ThinkAction or PartyMember must not be null");
            this.thinkAction = thinkAction;
            this.partyMember = partyMember ?? thinkAction.Actor;
            this.statusEffectEvent = statusEffectEvent;
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            currentStatusEffectIndex = -1;
            getNextStatusEffect();
        }

        public override void Finish()
        {
            base.Finish();
            PopState();
        }

        public override void Update(Delta delta)
        {
            if (currentStatusEffectIndex >= partyMember.StatusEffects.Count)
            {
                Finish();
                return;
            }

            bool statusEffectFinished = false;
            StatusEffect statusEffect = partyMember.StatusEffects[currentStatusEffectIndex];
            switch (statusEffectEvent)
            {
            case StatusEffectEvent.BeforeAct:
                if (statusEffect.BeforeActIsFinished())
                    statusEffectFinished = true;
                else
                    statusEffect.BeforeActUpdate(thinkAction, delta);
                break;
            case StatusEffectEvent.EndTurn:
                if (statusEffect.EndTurnIsFinished())
                    statusEffectFinished = true;
                else
                    statusEffect.EndTurnUpdate(partyMember, delta);
                break;
            default: break;
            }

            if (statusEffectFinished && partyMember.BattleEntity.AnimationState.Animation.Name == partyMember.GetBattleEntityIdleAnimationName())
                getNextStatusEffect();
        }

        private void getNextStatusEffect()
        {
            if (++currentStatusEffectIndex >= partyMember.StatusEffects.Count)
                return;

            StatusEffect statusEffect = partyMember.StatusEffects[currentStatusEffectIndex];
            switch (statusEffectEvent)
            {
            case StatusEffectEvent.BeforeAct: statusEffect.BeforeActStart(thinkAction); break;
            case StatusEffectEvent.EndTurn: statusEffect.EndTurnStart(partyMember); break;
            default: break;
            }
        }
        #endregion
    }
}
