using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.BattleStates
{
    class Act : BattleState
    {
        #region Instance Fields
        private List<ThinkAction> attackThinkActions;
        private List<ThinkAction> useItemThinkActions;
        private ThinkActionType currentThinkActionType;
        private int currentThinkActionIndex;
        private bool currentThinkActionFinished;
        private bool statusEffectsCompleteForAction;
        #endregion

        #region Constructors
        public Act(Battle battle, List<ThinkAction> thinkActions)
            : base(battle)
        {
            if (thinkActions == null)
                throw new Exception("List<ThinkAction> cannot be null");

            attackThinkActions = new List<ThinkAction>(thinkActions.Where(thinkAction => thinkAction.Type == ThinkActionType.Attack));
            useItemThinkActions = new List<ThinkAction>(thinkActions.Where(thinkAction => thinkAction.Type == ThinkActionType.UseItem));
            currentThinkActionIndex = 0;
            currentThinkActionType = ThinkActionType.None;
            statusEffectsCompleteForAction = false;
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            Comparison<ThinkAction> speedComparer = new Comparison<ThinkAction>((a, b) => b.Actor.Speed.CompareTo(a.Actor.Speed));
            attackThinkActions.Sort(speedComparer);
            useItemThinkActions.Sort(speedComparer);
            currentThinkActionType = ThinkActionType.UseItem;
            currentThinkActionFinished = false;
            statusEffectsCompleteForAction = false;
        }

        public override void Finish()
        {
            base.Finish();
            currentThinkActionType = ThinkActionType.None;
            ChangeState(new EndTurn(Battle));
        }

        public override void Resume(BattleState previousBattleState)
        {
            base.Resume(previousBattleState);
            if (previousBattleState is Attack || previousBattleState is UseItem)
                currentThinkActionFinished = true;
            else if (previousBattleState is HandleStatusEffects)
                statusEffectsCompleteForAction = true;
            removeDeadPartyMembers();
        }

        public override void Update(Delta delta)
        {
            if (Battle.PlayerParty.Count == 0 || Battle.EnemyParty.Count == 0)
            {
                Finish();
                return;
            }
            else if (currentThinkActionFinished)
            {
                if (allPartyMembersIdle())
                    getNextThinkAction();
                return;
            }

            ThinkAction thinkAction = null;

            if (currentThinkActionType == ThinkActionType.UseItem)
            {
                if (currentThinkActionIndex >= useItemThinkActions.Count)
                {
                    currentThinkActionIndex = 0;
                    statusEffectsCompleteForAction = false;
                    currentThinkActionType = ThinkActionType.Attack;
                    return;
                }

                thinkAction = useItemThinkActions[currentThinkActionIndex];
            }
            else if (currentThinkActionType == ThinkActionType.Attack)
            {
                if (currentThinkActionIndex >= attackThinkActions.Count)
                {
                    Finish();
                    return;
                }

                thinkAction = attackThinkActions[currentThinkActionIndex];
            }

            pushStateForThinkAction(thinkAction);
        }

        private void pushStateForThinkAction(ThinkAction thinkAction)
        {
            if (thinkAction == null)
                return;

            if (!statusEffectsCompleteForAction && thinkAction.Actor.Alive)
            {
                PushState(new HandleStatusEffects(Battle, StatusEffectEvent.BeforeAct, thinkAction: thinkAction));
                return;
            }

            if (!thinkAction.Actor.Alive || !thinkAction.Active)
            {
                if (!ThinkMenuOption.IsDefaultOptionName(thinkAction.OptionName))
                {
                    Inventory inventory = getInventoryFromThinkActionType(thinkAction.Type, thinkAction.Actor.CharacterClass);
                    if (inventory != null)
                        inventory.AddItem(thinkAction.OptionName);
                }
                getNextThinkAction();
                return;
            }

            if (thinkAction.Target.Alive)
            {
                if (thinkAction.Type == ThinkActionType.Attack)
                    PushState(new Attack(Battle, thinkAction));
                else
                    PushState(new UseItem(Battle, thinkAction));
            }
            else
            {
                if (Battle.PlayerParty.Count > 0 && Battle.EnemyParty.Count > 0)
                    PushState(new SelectTarget(Battle, thinkAction));
                else
                    Finish();
            }
        }

        private void getNextThinkAction()
        {
            ++currentThinkActionIndex;
            currentThinkActionFinished = false;
            statusEffectsCompleteForAction = false;
        }

        private bool allPartyMembersIdle()
        {
            return allPartyMembersIdle(Battle.PlayerParty) && allPartyMembersIdle(Battle.EnemyParty);
        }

        private bool allPartyMembersIdle(Party party)
        {
            foreach (PartyMember partyMember in party)
            {
                if (partyMember.Alive && partyMember.BattleEntity != null)
                {
                    if (partyMember.BattleEntity.AnimationState.Animation.Name != partyMember.GetBattleEntityIdleAnimationName())
                        return false;
                }
            }
            return true;
        }
        #endregion
    }
}
