using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Spine;

namespace SuperFantasticSteampunk
{
    using ScriptAction = Tuple<float, string, object[]>;

    class NestedScriptRunner
    {
        #region Instance Properties
        public float Time { get; set; }
        public ScriptRunner ScriptRunner { get; private set; }
        #endregion

        #region Constructors
        public NestedScriptRunner(float time, ScriptRunner scriptRunner)
        {
            Time = time;
            ScriptRunner = scriptRunner;
        }
        #endregion
    }

    class ScriptRunner
    {
        #region Instance Fields
        private Script script;
        private int scriptActionIndex;
        private Battle battle;
        private PartyMember actor;
        private PartyMember target;
        private float time;
        private List<NestedScriptRunner> nestedScriptRunners;
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
            nestedScriptRunners = new List<NestedScriptRunner>();
        }
        #endregion

        #region Instance Methods
        public void Update(GameTime gameTime)
        {
            if (scriptActionIndex >= script.Actions.Count)
                return;
            
            float elapsedGameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            time += elapsedGameTime;
            ScriptAction action = script.Actions[scriptActionIndex];
            if (time >= action.Item1)
            {
                executeAction(action);
                ++scriptActionIndex;
            }

            foreach (NestedScriptRunner nestedScriptRunner in nestedScriptRunners)
            {
                if (nestedScriptRunner.Time <= 0.0f)
                    nestedScriptRunner.ScriptRunner.Update(gameTime);
                else
                    nestedScriptRunner.Time -= elapsedGameTime;
            }
        }

        public bool IsFinished()
        {
            foreach (NestedScriptRunner nestedScriptRunner in nestedScriptRunners)
            {
                if (!nestedScriptRunner.ScriptRunner.IsFinished())
                    return false;
            }
            return scriptActionIndex >= script.Actions.Count;
        }

        private void executeAction(ScriptAction action)
        {
            object[] args = action.Item3;
            switch (action.Item2)
            {
            case "playAnimation": _playAnimation(args); break;
            case "doDamage": _doDamage(args); break;
            case "nop": break;
            default: break;
            }
        }

        private void _playAnimation(object[] args)
        { // playAnimation(string partyMemberId, string animationName, bool playNow, Script onStartCallback)
            string partyMemberId = (string)args[0];
            string animationName = (string)args[1];
            bool playNow = (bool)args[2];
            Script onStartCallback = (Script)args[3];

            PartyMember partyMember = getPartyMemberFromStringId(partyMemberId);
            AnimationState animationState = partyMember.BattleEntity.AnimationState;
            Animation originalAnimation = animationState.Animation;

            float animationStateTime = animationState.Time;
            float timeToAnimationEnd = 0.0f;
            if (!playNow)
                timeToAnimationEnd = animationStateTime + animationState.Animation.Duration - (animationStateTime % animationState.Animation.Duration);

            animationState.AddAnimation(animationName, false, timeToAnimationEnd);
            animationState.AddAnimation(originalAnimation, true);

            if (onStartCallback != null)
                addNestedScriptRunner(onStartCallback, playNow ? 0.0f : timeToAnimationEnd - animationStateTime);
        }

        private void _doDamage(object[] args)
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

        private void addNestedScriptRunner(Script script, float delay)
        {
            nestedScriptRunners.Add(new NestedScriptRunner(delay, new ScriptRunner(script, battle, actor, target)));
        }
        #endregion
    }
}
