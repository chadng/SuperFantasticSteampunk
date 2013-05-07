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
            currentThinkActionType = ThinkActionType.Attack; // change to use item first
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
            // if previousBattleState is action then increment currentThinkActionIndex
        }

        public override void Update(GameTime gameTime)
        {
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

            if (thinkAction == null)
                return;

            if (thinkAction.Target.Alive)
            {
                PartyMember target = Battle.GetPartyBattleLayoutForPartyMember(thinkAction.Target).FirstInPartyMembersList(thinkAction.Target);
                int damage = target.CalculateDamageTaken(thinkAction.Actor);
                target.DoDamage(damage);
                Logger.Log(thinkAction.Actor.Data.Name + " did " + damage.ToString() + " damage to " + target.Data.Name);
                ++currentThinkActionIndex;
                if (!target.Alive)
                    target.Kill(Battle);
            }
            else
            {
                if (Battle.PlayerParty.Count > 0 && Battle.EnemyParty.Count > 0)
                    PushState(new SelectTarget(Battle, thinkAction));
                else
                    Finish();
            }

            ////TODO: Change these to individual states
            //foreach (ThinkAction thinkAction in useItemThinkActions)
            //{
            //    Logger.Log(thinkAction.Actor.Data.Name + " used '" + thinkAction.OptionName + "' item");
            //    Logger.Log("TODO: use item"); //TODO: use item
            //}

            //foreach (ThinkAction thinkAction in attackThinkActions)
            //{
            //    thinkAction.Actor.EquipWeapon(thinkAction.OptionName);
            //    Logger.Log(thinkAction.Actor.Data.Name + " equipped '" + thinkAction.OptionName + "' weapon");

            //    PartyMember target = Battle.GetPartyBattleLayoutForPartyMember(thinkAction.Target).FirstInPartyMembersList(thinkAction.Target);

            //    if (thinkAction.Target.Alive)
            //    {
            //        int damage = target.CalculateDamageTaken(thinkAction.Actor);
            //        target.DoDamage(damage);
            //        Logger.Log(thinkAction.Actor.Data.Name + " did " + damage.ToString() + " damage to " + target.Data.Name);
            //    }

            //    if (!target.Alive)
            //    {
            //        Logger.Log(thinkAction.Actor.Data.Name + " target " + target.Data.Name + " is not alive");
            //        target.Kill(Battle);
            //        //TODO: choose new target
            //    }
            //}

            //Finish();
        }
        #endregion
    }
}
