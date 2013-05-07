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
        private List<ThinkAction> defendThinkActions;
        private List<ThinkAction> useItemThinkActions;
        #endregion

        #region Constructors
        public Act(Battle battle, List<ThinkAction> thinkActions)
            : base(battle)
        {
            if (thinkActions == null)
                throw new Exception("List<ThinkAction> cannot be null");

            attackThinkActions = new List<ThinkAction>(thinkActions.Where(thinkAction => thinkAction.Type == ThinkActionType.Attack));
            defendThinkActions = new List<ThinkAction>(thinkActions.Where(thinkAction => thinkAction.Type == ThinkActionType.Defend));
            useItemThinkActions = new List<ThinkAction>(thinkActions.Where(thinkAction => thinkAction.Type == ThinkActionType.UseItem));
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            Comparison<ThinkAction> speedComparer = new Comparison<ThinkAction>((a, b) => b.Actor.Speed.CompareTo(a.Actor.Speed));
            attackThinkActions.Sort(speedComparer);
            defendThinkActions.Sort(speedComparer);
            useItemThinkActions.Sort(speedComparer);
        }

        public override void Finish()
        {
            ChangeState(new EndTurn(Battle));
        }

        public override void Update(GameTime gameTime)
        {
            //TODO: Change these to individual states
            foreach (ThinkAction thinkAction in useItemThinkActions)
            {
                Logger.Log(thinkAction.Actor.Data.Name + " used '" + thinkAction.OptionName + "' item");
                Logger.Log("TODO: use item"); //TODO: use item
            }

            foreach (ThinkAction thinkAction in attackThinkActions)
            {
                thinkAction.Actor.EquipWeapon(thinkAction.OptionName);
                Logger.Log(thinkAction.Actor.Data.Name + " equipped '" + thinkAction.OptionName + "' weapon");

                PartyMember target = Battle.GetPartyBattleLayoutForPartyMember(thinkAction.Target).FirstInPartyMembersList(thinkAction.Target);

                if (thinkAction.Target.Alive)
                {
                    int damage = target.CalculateDamageTaken(thinkAction.Actor);
                    target.DoDamage(damage);
                    Logger.Log(thinkAction.Actor.Data.Name + " did " + damage.ToString() + " damage to " + target.Data.Name);
                }

                if (!target.Alive)
                {
                    Logger.Log(thinkAction.Actor.Data.Name + " target " + target.Data.Name + " is not alive");
                    target.Kill(Battle);
                    //TODO: choose new target
                }
            }

            Finish();
        }
        #endregion
    }
}
