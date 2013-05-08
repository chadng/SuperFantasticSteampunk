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
            float elapsedGameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            time += elapsedGameTime;
            while (scriptActionIndex < script.Actions.Count)
            {
                ScriptAction action = script.Actions[scriptActionIndex];
                if (time < action.Item1)
                    break;
                else
                {
                    executeAction(action);
                    ++scriptActionIndex;
                }
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
            case "queueAnimation": _queueAnimation(args); break;
            case "playAnimation": _playAnimation(args); break;
            case "doDamage": _doDamage(args); break;
            case "setVelocity": _setVelocity(args); break;
            case "setVelocityX": _setVelocityX(args); break;
            case "setVelocityY": _setVelocityY(args); break;
            case "setRotation": _setRotation(args); break;
            case "setAngularVelocity": _setAngularVelocity(args); break;
            case "log": _log(args); break;
            case "nop": break;
            default: break;
            }
        }

        private void _queueAnimation(object[] args)
        { // queueAnimation(string partyMemberId, string animationName, Script onStartCallback)
            string partyMemberId = (string)args[0];
            string animationName = (string)args[1];
            Script onStartCallback = (Script)args[2];

            PartyMember partyMember = getPartyMemberFromStringId(partyMemberId);
            AnimationState animationState = partyMember.BattleEntity.AnimationState;

            float animationStateTime = animationState.Time;
            float timeToAnimationEnd = animationState.Animation.Duration - (animationStateTime % animationState.Animation.Duration);

            animationState.AddAnimation(animationName, false, animationStateTime + timeToAnimationEnd);
            animationState.AddAnimation("idle", true);

            if (onStartCallback != null)
                addNestedScriptRunner(onStartCallback, timeToAnimationEnd);
        }

        private void _playAnimation(object[] args)
        { // playAnimation(string partyMemberId, string animationName)
            string partyMemberId = (string)args[0];
            string animationName = (string)args[1];

            PartyMember partyMember = getPartyMemberFromStringId(partyMemberId);
            AnimationState animationState = partyMember.BattleEntity.AnimationState;

            animationState.SetAnimation(animationName, false);
            animationState.AddAnimation("idle", true);
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

            if (actualTarget.EquippedShield != null)
                addNestedScriptRunner(actualTarget.EquippedShield.Data.Script, 0.0f);

            if (!actualTarget.Alive)
                actualTarget.Kill(battle);
        }

        private void _setVelocity(object[] args)
        { // setVelocity(string partyMemberId, float x, float y)
            string partyMemberId = (string)args[0];
            float x = (float)args[1];
            float y = (float)args[2];

            getPartyMemberFromStringId(partyMemberId).BattleEntity.Velocity = new Vector2(x, y);
        }

        private void _setVelocityX(object[] args)
        { // setVelocityX(string partyMemberId, float x)
            string partyMemberId = (string)args[0];
            float x = (float)args[1];

            Entity battleEntity = getPartyMemberFromStringId(partyMemberId).BattleEntity;
            battleEntity.Velocity = new Vector2(x, battleEntity.Velocity.Y);
        }

        private void _setVelocityY(object[] args)
        { // setVelocityY(string partyMemberId, float y)
            string partyMemberId = (string)args[0];
            float y = (float)args[1];

            Entity battleEntity = getPartyMemberFromStringId(partyMemberId).BattleEntity;
            battleEntity.Velocity = new Vector2(battleEntity.Velocity.X, y);
        }

        private void _setRotation(object[] args)
        { // setRotation(string partyMemberId, float amount)
            string partyMemberId = (string)args[0];
            float amount = (float)args[1];

            getPartyMemberFromStringId(partyMemberId).BattleEntity.Rotation = amount;
        }

        private void _setAngularVelocity(object[] args)
        { // setAngularVelocity(string partyMemberId, float amount)
            string partyMemberId = (string)args[0];
            float amount = (float)args[1];

            getPartyMemberFromStringId(partyMemberId).BattleEntity.AngularVelocity = amount;
        }

        private void _log(object[] args)
        { // log(string message)
            string message = (string)args[0];

            Logger.Log("Script log at " + time.ToString() + ": " + message);
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
