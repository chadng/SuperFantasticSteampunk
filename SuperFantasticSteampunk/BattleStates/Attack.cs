using System;
using Microsoft.Xna.Framework;
using Spine;

namespace SuperFantasticSteampunk.BattleStates
{
    class Attack : BattleState
    {
        #region Instance Fields
        private ThinkAction thinkAction;
        private ScriptRunner scriptRunner;
        private bool scriptStarted;
        private float scriptStartTime;
        private float scriptStartTimer;
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
            scriptStarted = false;
            scriptStartTime = 0.0f;
            scriptStartTimer = 0.0f;
            battleEntity = thinkAction.Actor.BattleEntity;
            playAttackAnimation = false;
            timeTilHurt = 0.0f;
            alreadyHurt = false;
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            Weapon weapon = thinkAction.Actor.EquippedWeapon;
            Script script;
            if (weapon == null || weapon.Data.Script == null)
                script = new Script("0.0 doDamage string:actor string:target");
            else
                script = weapon.Data.Script;
            scriptRunner = new ScriptRunner(script, Battle, thinkAction.Actor, thinkAction.Target);

            AnimationState animationState = battleEntity.AnimationState;
            scriptStartTime = animationState.Animation.Duration - (animationState.Time % animationState.Animation.Duration);
        }

        public override void Finish()
        {
            PopState();
        }

        public override void Update(GameTime gameTime)
        {
            scriptStartTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (scriptStartTimer >= scriptStartTime)
            {
                scriptRunner.Update(gameTime);
                if (scriptRunner.IsFinished())
                    Finish();
            }
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
