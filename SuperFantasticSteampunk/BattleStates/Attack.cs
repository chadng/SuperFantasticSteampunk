using System;
using Microsoft.Xna.Framework;
using Spine;

namespace SuperFantasticSteampunk.BattleStates
{
    class Attack : BattleState
    {
        #region Instance Fields
        private ThinkAction thinkAction;
        private Entity battleEntity;
        private Animation originalAnimation;
        private Animation attackAnimation;
        #endregion

        #region Constructors
        public Attack(Battle battle, ThinkAction thinkAction)
            : base(battle)
        {
            if (thinkAction == null)
                throw new Exception("ThinkAction cannot be null");
            this.thinkAction = thinkAction;
            battleEntity = thinkAction.Actor.BattleEntity;
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            originalAnimation = battleEntity.AnimationState.Animation;
            Weapon weapon = thinkAction.Actor.EquippedWeapon;
            if (weapon == null)
                return;

            string animationName = weapon.Data.AttackAnimationName;
            if (animationName != null)
            {
                attackAnimation = battleEntity.Skeleton.Data.FindAnimation(animationName);
                if (attackAnimation != null)
                {
                    float timeToAnimationEnd = battleEntity.AnimationState.Animation.Duration - (battleEntity.AnimationState.Time % battleEntity.AnimationState.Animation.Duration);
                    battleEntity.AnimationState.AddAnimation(attackAnimation, false, battleEntity.AnimationState.Time + timeToAnimationEnd);
                    battleEntity.AnimationState.AddAnimation(originalAnimation, true);
                }
            }
        }

        public override void Finish()
        {
            PopState();
        }

        public override void Update(GameTime gameTime)
        {
            if (attackAnimation != null) // if attack animation has not started
            {
                if (battleEntity.AnimationState.Animation.Name == attackAnimation.Name)
                    attackAnimation = null; // indicate that the attack animation has started
                return;
            }
            else if (battleEntity.AnimationState.Animation.Name != originalAnimation.Name) // if the attack animation is playing/original animation not resumed
                return;

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
