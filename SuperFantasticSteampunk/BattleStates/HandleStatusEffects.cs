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
            switch (statusEffectEvent)
            {
            case StatusEffectEvent.BeforeAct:
                partyMember.ForEachStatusEffect(statusEffect => statusEffect.BeforeActStart(thinkAction));
                break;
            case StatusEffectEvent.EndTurn:
                partyMember.ForEachStatusEffect(statusEffect => statusEffect.EndTurnStart(partyMember));
                break;
            }
        }

        public override void Finish()
        {
            base.Finish();
            PopState();
        }

        public override void Update(Delta delta)
        {
            bool allStatusEffectsFinished = true;

            switch (statusEffectEvent)
            {
            case StatusEffectEvent.BeforeAct:
                partyMember.ForEachStatusEffect(statusEffect =>
                {
                    if (!statusEffect.BeforeActIsFinished())
                    {
                        statusEffect.BeforeActUpdate(thinkAction, delta);
                        allStatusEffectsFinished = false;
                    }
                });
                break;
            case StatusEffectEvent.EndTurn:
                partyMember.ForEachStatusEffect(statusEffect =>
                {
                    if (!statusEffect.EndTurnIsFinished())
                    {
                        statusEffect.EndTurnUpdate(partyMember, delta);
                        allStatusEffectsFinished = false;
                    }
                });
                break;
            }

            if (allStatusEffectsFinished)
                Finish();
        }
        #endregion
    }
}
