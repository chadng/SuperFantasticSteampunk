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
        private float scriptStartTime;
        private float scriptStartTimer;
        #endregion

        #region Constructors
        public Attack(Battle battle, ThinkAction thinkAction)
            : base(battle)
        {
            if (thinkAction == null)
                throw new Exception("ThinkAction cannot be null");
            this.thinkAction = thinkAction;
            scriptStartTime = 0.0f;
            scriptStartTimer = 0.0f;
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            Weapon weapon = thinkAction.Actor.EquippedWeapon;
            Script script;
            if (weapon == null || weapon.Data.Script == null)
                script = new Script("0.0 doDamage string:actor string:target>list>front bool:false");
            else
                script = weapon.Data.Script;
            scriptRunner = new ScriptRunner(script, Battle, thinkAction.Actor, thinkAction.Target);

            AnimationState animationState = thinkAction.Actor.BattleEntity.AnimationState;
            scriptStartTime = animationState.Animation.Duration - (animationState.Time % animationState.Animation.Duration);
        }

        public override void Finish()
        {
            if (!thinkAction.InfiniteInInventory)
                Battle.IncrementItemsUsed(Battle.GetPartyForPartyMember(thinkAction.Actor));
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
        #endregion
    }
}
