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
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            Comparison<ThinkAction> speedComparer = new Comparison<ThinkAction>((a, b) => b.Actor.Speed.CompareTo(a.Actor.Speed));
            attackThinkActions.Sort(speedComparer);
            useItemThinkActions.Sort(speedComparer);
            currentThinkActionType = ThinkActionType.UseItem;
        }

        public override void Finish()
        {
            currentThinkActionType = ThinkActionType.None;
            ChangeState(new EndTurn(Battle));
        }

        public override void Pause()
        {
            base.Pause();
        }

        public override void Resume(BattleState previousBattleState)
        {
            base.Resume(previousBattleState);
            if (previousBattleState is Attack || previousBattleState is UseItem)
                ++currentThinkActionIndex;
            removeDeadPartyMembers(Battle.PlayerParty);
            removeDeadPartyMembers(Battle.EnemyParty);
        }

        public override void Update(GameTime gameTime)
        {
            if (Battle.PlayerParty.Count == 0 || Battle.EnemyParty.Count == 0)
            {
                Finish();
                return;
            }

            ThinkAction thinkAction = null;

            if (currentThinkActionType == ThinkActionType.UseItem)
            {
                if (currentThinkActionIndex >= useItemThinkActions.Count)
                {
                    currentThinkActionIndex = 0;
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

            if (!thinkAction.Actor.Alive)
            {
                ++currentThinkActionIndex;
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

        private void removeDeadPartyMembers(Party party)
        {
            for (int i = party.Count - 1; i >= 0; --i)
            {
                if (!party[i].Alive)
                    party[i].Kill(Battle);
            }
        }
        #endregion
    }
}
