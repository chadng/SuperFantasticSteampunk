using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Spine;

namespace SuperFantasticSteampunk
{
    using ScriptAction = Tuple<float, string, object[]>;

    class ScriptRunner
    {
        #region Instance Fields
        private Script script;
        private int scriptActionIndex;
        private Battle battle;
        private PartyMember actor;
        private PartyMember target;
        private float time;
        #endregion

        #region Constructors
        public ScriptRunner(Script script, Battle battle, PartyMember actor, PartyMember target)
        {
            this.script = script;
            scriptActionIndex = 0;
            this.battle = battle;
            this.actor = actor;
            this.target = target;
            time = 0.0f;
        }
        #endregion

        #region Instance Methods
        public void Update(GameTime gameTime)
        {
            if (scriptActionIndex >= script.Actions.Count)
                return;
            
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            ScriptAction action = script.Actions[scriptActionIndex];
            if (time >= action.Item1)
            {
                executeAction(action);
                ++scriptActionIndex;
            }
        }

        public bool IsFinished()
        {
            return scriptActionIndex >= script.Actions.Count;
        }

        private void executeAction(ScriptAction action)
        {
            object[] args = action.Item3;
            switch (action.Item2)
            {
            case "playAnimation": playAnimation(args); break;
            case "doDamage": doDamage(args); break;
            case "nop": break;
            default: break;
            }
        }

        private void playAnimation(object[] args)
        { // playAnimation(string partyMemberId, string animationName, bool playNow)
            string partyMemberId = (string)args[0];
            string animationName = (string)args[1];
            bool playNow = (bool)args[2];

            PartyMember partyMember = getPartyMemberFromStringId(partyMemberId);
            AnimationState animationState = partyMember.BattleEntity.AnimationState;
            Animation originalAnimation = animationState.Animation;

            float timeToAnimationEnd = 0.0f;
            if (!playNow)
                timeToAnimationEnd = animationState.Time + animationState.Animation.Duration - (animationState.Time % animationState.Animation.Duration);

            animationState.AddAnimation(animationName, false, timeToAnimationEnd);
            animationState.AddAnimation(originalAnimation, true);
        }

        private void doDamage(object[] args)
        { // doDamage(string actorPartyMemberId, string targetPartyMemberId)
            string actorPartyMemberId = (string)args[0];
            string targetPartyMemberId = (string)args[1];

            PartyMember actor = getPartyMemberFromStringId(actorPartyMemberId);
            PartyMember target = getPartyMemberFromStringId(targetPartyMemberId);

            PartyMember actualTarget = battle.GetPartyBattleLayoutForPartyMember(target).FirstInPartyMembersList(target);
            int damage = actualTarget.CalculateDamageTaken(actor);
            actualTarget.DoDamage(damage);
            if (!actualTarget.Alive)
                actualTarget.Kill(battle);
        }

        private PartyMember getPartyMemberFromStringId(string id)
        {
            switch (id)
            {
            case "actor": return actor;
            case "target": return target;
            default: return null;
            }
        }
        #endregion
    }
}
