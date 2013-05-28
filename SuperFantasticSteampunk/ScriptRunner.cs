﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Spine;

namespace SuperFantasticSteampunk
{
    using ScriptAction = Tuple<float, string, object[]>;
    
    enum ScriptRunnerMode { Normal, CatchFunction, CatchScript }

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
        private ScriptRunnerMode mode;
        #endregion

        #region Constructors
        public ScriptRunner(Script script, Battle battle, PartyMember actor, PartyMember target, ScriptRunnerMode mode = ScriptRunnerMode.Normal)
        {
            this.script = script;
            scriptActionIndex = 0;
            this.battle = battle;
            this.actor = actor;
            this.target = target;
            time = 0.0f;
            nestedScriptRunners = new List<NestedScriptRunner>();
            this.mode = mode;
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
            try
            {
                object[] args = action.Item3;
                switch (action.Item2)
                {
                case "queueAnimation": _queueAnimation(args); break;
                case "playAnimation": _playAnimation(args); break;
                case "doDamage": _doDamage(args); break;
                case "addHealth": _addHealth(args); break;
                case "addMaxHealthStatModifier": _addMaxHealthStatModifier(args); break;
                case "addAttackStatModifier": _addAttackStatModifier(args); break;
                case "addSpecialAttackStatModifier": _addSpecialAttackStatModifier(args); break;
                case "addDefenceStatModifier": _addDefenceStatModifier(args); break;
                case "addSpeedStatModifier": _addSpeedStatModifier(args); break;
                case "addCharmStatModifier": _addCharmStatModifier(args); break;
                case "addStatusEffect": _addStatusEffect(args); break;
                case "setVelocity": _setVelocity(args); break;
                case "setVelocityX": _setVelocityX(args); break;
                case "setVelocityY": _setVelocityY(args); break;
                case "setRotation": _setRotation(args); break;
                case "setAngularVelocity": _setAngularVelocity(args); break;
                case "random": _random(args); break;
                case "log": _log(args); break;
                case "safe": _safe(args); break;
                case "nop": break;
                default: break;
                }
            }
            catch (Exception e)
            {
                switch (mode)
                {
                case ScriptRunnerMode.CatchFunction: break;
                case ScriptRunnerMode.CatchScript: scriptActionIndex = script.Actions.Count; break;
                default: throw e;
                }
            }
        }

        private void _queueAnimation(object[] args)
        { // queueAnimation(string partyMemberSelector, string animationName, [Script onStartCallback], [Script onFinishCallback])
            string partyMemberSelector = (string)args[0];
            string animationName = (string)args[1];
            Script onStartCallback = args.Length <= 2 ? null : (Script)args[2];
            Script onFinishCallback = args.Length <= 3 ? null : (Script)args[3]; 

            PartyMember partyMember = getPartyMemberFromSelector(partyMemberSelector);
            AnimationState animationState = partyMember.BattleEntity.AnimationState;
            Animation animation = animationState.Data.SkeletonData.FindAnimation(animationName);
            if (animation == null)
                throw new Exception("Animation '" + animationName + "' could not be found");

            float animationStateTime = animationState.Time;
            float timeToCurrentAnimationEnd = animationState.Animation.Duration - (animationStateTime % animationState.Animation.Duration);

            animationState.AddAnimation(animation, false, animationStateTime + timeToCurrentAnimationEnd);
            animationState.AddAnimation("idle", true);

            if (onStartCallback != null)
                addNestedScriptRunner(onStartCallback, timeToCurrentAnimationEnd);
            if (onFinishCallback != null)
                addNestedScriptRunner(onFinishCallback, timeToCurrentAnimationEnd + animation.Duration);
        }

        private void _playAnimation(object[] args)
        { // playAnimation(string partyMemberSelector, string animationName, [Script onFinishCallback])
            string partyMemberSelector = (string)args[0];
            string animationName = (string)args[1];
            Script onFinishCallback = args.Length <= 2 ? null : (Script)args[2];

            PartyMember partyMember = getPartyMemberFromSelector(partyMemberSelector);
            AnimationState animationState = partyMember.BattleEntity.AnimationState;
            Animation animation = animationState.Data.SkeletonData.FindAnimation(animationName);
            if (animation == null)
                throw new Exception("Animation '" + animationName + "' could not be found");

            animationState.SetAnimation(animation, false);
            animationState.AddAnimation("idle", true);

            if (onFinishCallback != null)
                addNestedScriptRunner(onFinishCallback, animation.Duration);
        }

        private void _doDamage(object[] args)
        { // doDamage(string actorPartyMemberSelector, string targetPartyMemberSelector)
            string actorPartyMemberSelector = (string)args[0];
            string targetPartyMemberSelector = (string)args[1];

            PartyMember actor = getPartyMemberFromSelector(actorPartyMemberSelector);
            PartyMember target = getPartyMemberFromSelector(targetPartyMemberSelector);

            int damage = target.CalculateDamageTaken(actor);
            target.DoDamage(damage);
            Scene.AddEntity(new FloatingText(damage.ToString(), Color.Red, target.BattleEntity.Position));

            if (target.EquippedShield != null)
                addNestedScriptRunner(target.EquippedShield.Data.Script, 0.0f);
        }

        private void _addHealth(object[] args)
        { // addHealth(string partyMemberSelector, int amount)
            string partyMemberSelector = (string)args[0];
            int amount = (int)args[1];

            PartyMember partyMember = getPartyMemberFromSelector(partyMemberSelector);
            partyMember.DoDamage(-amount);
        }

        private void _addMaxHealthStatModifier(object[] args)
        { // addMaxHealthStatModifier(string partyMemberSelector, int turns, float percentage)
            string partyMemberSelector = (string)args[0];
            int turns = (int)args[1];
            float percentage = (float)args[2];

            PartyMember partyMember = getPartyMemberFromSelector(partyMemberSelector);
            partyMember.AddStatModifier(new StatModifier(turns, maxHealth: percentage));
        }

        private void _addAttackStatModifier(object[] args)
        { // addAttackStatModifier(string partyMemberSelector, int turns, float percentage)
            string partyMemberSelector = (string)args[0];
            int turns = (int)args[1];
            float percentage = (float)args[2];

            PartyMember partyMember = getPartyMemberFromSelector(partyMemberSelector);
            partyMember.AddStatModifier(new StatModifier(turns, attack: percentage));
        }

        private void _addSpecialAttackStatModifier(object[] args)
        { // addSpecialAttackStatModifier(string partyMemberSelector, int turns, float percentage)
            string partyMemberSelector = (string)args[0];
            int turns = (int)args[1];
            float percentage = (float)args[2];

            PartyMember partyMember = getPartyMemberFromSelector(partyMemberSelector);
            partyMember.AddStatModifier(new StatModifier(turns, specialAttack: percentage));
        }

        private void _addDefenceStatModifier(object[] args)
        { // addDefenceStatModifier(string partyMemberSelector, int turns, float percentage)
            string partyMemberSelector = (string)args[0];
            int turns = (int)args[1];
            float percentage = (float)args[2];

            PartyMember partyMember = getPartyMemberFromSelector(partyMemberSelector);
            partyMember.AddStatModifier(new StatModifier(turns, defence: percentage));
        }

        private void _addSpeedStatModifier(object[] args)
        { // addSpeedStatModifier(string partyMemberSelector, int turns, float percentage)
            string partyMemberSelector = (string)args[0];
            int turns = (int)args[1];
            float percentage = (float)args[2];

            PartyMember partyMember = getPartyMemberFromSelector(partyMemberSelector);
            partyMember.AddStatModifier(new StatModifier(turns, speed: percentage));
        }

        private void _addCharmStatModifier(object[] args)
        { // addCharmStatModifier(string partyMemberSelector, int turns, float percentage)
            string partyMemberSelector = (string)args[0];
            int turns = (int)args[1];
            float percentage = (float)args[2];

            PartyMember partyMember = getPartyMemberFromSelector(partyMemberSelector);
            partyMember.AddStatModifier(new StatModifier(turns, charm: percentage));
        }

        private void _addStatusEffect(object[] args)
        { // addStatusEffect(string partyMemberSelector, StatusEffectType statusEffectType)
            string partyMemberSelector = (string)args[0];
            StatusEffectType statusEffectType = (StatusEffectType)args[1];

            StatusEffect statusEffect;
            switch (statusEffectType)
            {
            case StatusEffectType.Poison: statusEffect = new StatusEffects.Poison(); break;
            case StatusEffectType.Paralysis: statusEffect = new StatusEffects.Paralysis(); break;
            case StatusEffectType.Fear: statusEffect = new StatusEffects.Fear(actor); break;
            default: statusEffect = null; break;
            }

            if (statusEffect != null)
            {
                PartyMember partyMember = getPartyMemberFromSelector(partyMemberSelector);
                partyMember.AddStatusEffect(statusEffect);
            }
        }

        private void _setVelocity(object[] args)
        { // setVelocity(string partyMemberSelector, float x, float y)
            string partyMemberSelector = (string)args[0];
            float x = (float)args[1];
            float y = (float)args[2];

            getPartyMemberFromSelector(partyMemberSelector).BattleEntity.Velocity = new Vector2(x, y);
        }

        private void _setVelocityX(object[] args)
        { // setVelocityX(string partyMemberSelector, float x)
            string partyMemberSelector = (string)args[0];
            float x = (float)args[1];

            Entity battleEntity = getPartyMemberFromSelector(partyMemberSelector).BattleEntity;
            battleEntity.Velocity = new Vector2(x, battleEntity.Velocity.Y);
        }

        private void _setVelocityY(object[] args)
        { // setVelocityY(string partyMemberSelector, float y)
            string partyMemberSelector = (string)args[0];
            float y = (float)args[1];

            Entity battleEntity = getPartyMemberFromSelector(partyMemberSelector).BattleEntity;
            battleEntity.Velocity = new Vector2(battleEntity.Velocity.X, y);
        }

        private void _setRotation(object[] args)
        { // setRotation(string partyMemberSelector, float amount)
            string partyMemberSelector = (string)args[0];
            float amount = (float)args[1];

            getPartyMemberFromSelector(partyMemberSelector).BattleEntity.Rotation = amount;
        }

        private void _setAngularVelocity(object[] args)
        { // setAngularVelocity(string partyMemberSelector, float amount)
            string partyMemberSelector = (string)args[0];
            float amount = (float)args[1];

            getPartyMemberFromSelector(partyMemberSelector).BattleEntity.AngularVelocity = amount;
        }

        private void _random(object[] args)
        { // random(float chance, Script successScript, [Script failScript])
            float chance = (float)args[0];
            Script successScript = (Script)args[1];
            Script failScript = args.Length <= 2 ? null : (Script)args[2];

            if (Game1.Random.NextDouble() <= chance)
                addNestedScriptRunner(successScript, 0.0f);
            else if (failScript != null)
                addNestedScriptRunner(failScript, 0.0f);
        }

        private void _log(object[] args)
        { // log(string message)
            string message = (string)args[0];

            Logger.Log("Script log at " + time.ToString() + ": " + message);
        }

        private void _safe(object[] args)
        { // safe(Script script, [bool catchFunctions = false])
            Script script = (Script)args[0];
            bool catchFunctions = args.Length <= 1 ? false : (bool)args[1];

            addNestedScriptRunner(script, 0.0f, catchFunctions ? ScriptRunnerMode.CatchFunction : ScriptRunnerMode.CatchScript);
        }

        private PartyMember getPartyMemberFromSelector(string selector)
        {
            string[] selectorParts = selector.Split('>');
            object result;
            switch (selectorParts[0])
            {
            case "actor": result = actor; break;
            case "target": result = target; break;
            default: result = null; break;
            }

            for (int i = 1; i < selectorParts.Length; ++i)
                result = getNextPartyMemberOrListFromSelectorPart(result, selectorParts[i]);

            return result as PartyMember;
        }

        private object getNextPartyMemberOrListFromSelectorPart(object partyMemberOrList, string selectorPart)
        {
            if (partyMemberOrList == null)
                return null;

            PartyMember partyMember = partyMemberOrList as PartyMember;
            if (partyMember != null)
            {
                switch (selectorPart)
                {
                case "list": return battle.GetPartyBattleLayoutForPartyMember(partyMember).PartyMembersList(partyMember);
                default: return null;
                }
            }
            else
            {
                List<PartyMember> list = partyMemberOrList as List<PartyMember>;
                switch (selectorPart)
                {
                case "front": return list[0];
                case "back": return list[list.Count - 1];
                case "up": return battle.GetPartyBattleLayoutForPartyMember(list[0]).RelativeList(list, -1);
                case "down": return battle.GetPartyBattleLayoutForPartyMember(list[0]).RelativeList(list, 1);
                default: return null;
                }
            }
        }

        private void addNestedScriptRunner(Script script, float delay, ScriptRunnerMode newMode = ScriptRunnerMode.Normal)
        {
            if (newMode == ScriptRunnerMode.Normal && mode != ScriptRunnerMode.Normal)
                newMode = ScriptRunnerMode.CatchScript;
            nestedScriptRunners.Add(new NestedScriptRunner(delay, new ScriptRunner(script, battle, actor, target, newMode)));
        }
        #endregion
    }
}
