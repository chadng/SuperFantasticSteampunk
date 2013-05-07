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
        private bool playAttackAnimation;
        private float timeTilHurt;
        private bool alreadyHurt;
        #endregion

        #region Constructors
        public Attack(Battle battle, ThinkAction thinkAction)
            : base(battle)
        {
            if (thinkAction == null)
                throw new Exception("ThinkAction cannot be null");
            this.thinkAction = thinkAction;
            battleEntity = thinkAction.Actor.BattleEntity;
            playAttackAnimation = false;
            timeTilHurt = 0.0f;
            alreadyHurt = false;
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
                    playAttackAnimation = true;
                    timeTilHurt = weapon.Data.AttackHurtTime;
                }
            }
        }

        public override void Finish()
        {
            PopState();
        }

        public override void Update(GameTime gameTime)
        {
            if (playAttackAnimation)
            {
                if (attackAnimation != null) // if attack animation has not started
                {
                    if (battleEntity.AnimationState.Animation.Name == attackAnimation.Name)
                        attackAnimation = null; // indicate that the attack animation has started
                    return;
                }
                else if (battleEntity.AnimationState.Animation.Name != originalAnimation.Name) // if the attack animation is playing/original animation not resumed
                {
                    timeTilHurt -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (timeTilHurt <= 0.0f)
                        hurt();
                    return;
                }

                if (timeTilHurt <= 0.0f)
                    Finish();
                return;
            }

            hurt();
            Finish();
        }

        private void hurt()
        {
            if (alreadyHurt)
                return;

            PartyMember target = Battle.GetPartyBattleLayoutForPartyMember(thinkAction.Target).FirstInPartyMembersList(thinkAction.Target);
            int damage = target.CalculateDamageTaken(thinkAction.Actor);
            target.DoDamage(damage);
            if (playAttackAnimation)
                playHurtAnimationOnPartyMember(target);
            Logger.Log(thinkAction.Actor.Data.Name + " did " + damage.ToString() + " damage to " + target.Data.Name);
            if (!target.Alive)
                target.Kill(Battle);
            alreadyHurt = true;
        }

        private void playHurtAnimationOnPartyMember(PartyMember partyMember)
        {
            AnimationState animationState = partyMember.BattleEntity.AnimationState;
            Animation originalAnimation = animationState.Animation;
            Animation hurtAnimation = partyMember.BattleEntity.Skeleton.Data.FindAnimation("hurt");
            if (hurtAnimation != null)
            {
                animationState.AddAnimation(hurtAnimation, false);
                animationState.AddAnimation(originalAnimation, true);
            }
        }
        #endregion
    }
}
