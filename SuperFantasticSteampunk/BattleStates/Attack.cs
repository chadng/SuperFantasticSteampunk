using System;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.BattleStates
{
    class Attack : BattleState
    {
        #region Instance Fields
        private ThinkAction thinkAction;
        #endregion

        #region Constructors
        public Attack(Battle battle, ThinkAction thinkAction)
            : base(battle)
        {
            if (thinkAction == null)
                throw new Exception("ThinkAction cannot be null");
            this.thinkAction = thinkAction;
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            //TODO: something
        }

        public override void Finish()
        {
            PopState();
        }

        public override void Update(GameTime gameTime)
        {
            PartyMember target = Battle.GetPartyBattleLayoutForPartyMember(thinkAction.Target).FirstInPartyMembersList(thinkAction.Target);
            int damage = target.CalculateDamageTaken(thinkAction.Actor);
            target.DoDamage(damage);
            Logger.Log(thinkAction.Actor.Data.Name + " did " + damage.ToString() + " damage to " + target.Data.Name);
            if (!target.Alive)
                target.Kill(Battle);

            Finish();
        }
        #endregion
    }
}
