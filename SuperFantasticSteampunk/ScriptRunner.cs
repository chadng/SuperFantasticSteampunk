using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Spine;

namespace SuperFantasticSteampunk
{
    using ScriptAction = Tuple<float, string, object[], int>;
    
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
        private bool blocked;
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
            blocked = false;
        }
        #endregion

        #region Instance Methods
        public void Update(Delta delta)
        {
            time += delta.Time;
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
                    nestedScriptRunner.ScriptRunner.Update(delta);
                else
                    nestedScriptRunner.Time -= delta.Time;
            }
        }

        public bool IsFinished()
        {
            foreach (NestedScriptRunner nestedScriptRunner in nestedScriptRunners)
            {
                if (!nestedScriptRunner.ScriptRunner.IsFinished())
                    return false;
            }
            return !blocked && scriptActionIndex >= script.Actions.Count;
        }

        private void executeAction(ScriptAction action)
        {
            switch (mode)
            {
            case ScriptRunnerMode.CatchFunction:
                try { executeActionInternal(action); }
                catch { }
                break;
            case ScriptRunnerMode.CatchScript:
                try { executeAction(action); }
                catch { scriptActionIndex = script.Actions.Count; }
                break;
            default: executeActionInternal(action); break;
            }
        }

        private void executeActionInternal(ScriptAction action)
        {
            object[] args = action.Item3;
            switch (action.Item2)
            {
            case "queueAnimation": _queueAnimation(args); break;
            case "playAnimation": _playAnimation(args); break;
            case "doDamage": _doDamage(args); break;
            case "addHealth": _addHealth(args); break;
            case "addMaxHealthStatModifier": _addMaxHealthStatModifier(args); break;
            case "addMeleeAttackStatModifier": _addMeleeAttackStatModifier(args); break;
            case "addRangedAttackStatModifier": _addRangedAttackStatModifier(args); break;
            case "addDefenceStatModifier": _addDefenceStatModifier(args); break;
            case "addSpeedStatModifier": _addSpeedStatModifier(args); break;
            case "addStatusEffect": _addStatusEffect(args); break;
            case "setVelocity": _setVelocity(args); break;
            case "setVelocityX": _setVelocityX(args); break;
            case "setVelocityY": _setVelocityY(args); break;
            case "setAcceleration": _setAcceleration(args); break;
            case "setAccelerationX": _setAccelerationX(args); break;
            case "setAccelerationY": _setAccelerationY(args); break;
            case "setRotation": _setRotation(args); break;
            case "setAngularVelocity": _setAngularVelocity(args); break;
            case "moveTo": _moveTo(args); break;
            case "moveToIdlePosition": _moveToIdlePosition(args); break;
            case "setIdleAnimation": _setIdleAnimation(args); break;
            case "createTrap": _createTrap(args); break;
            case "random": _random(args); break;
            case "log": _log(args); break;
            case "safe": _safe(args); break;
            case "nop": break;
            default: break;
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
            animationState.AddAnimation(partyMember.BattleEntityIdleAnimationName, true);

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
            animationState.AddAnimation(partyMember.BattleEntityIdleAnimationName, true);

            if (onFinishCallback != null)
                addNestedScriptRunner(onFinishCallback, animation.Duration);
        }

        private void _doDamage(object[] args)
        { // doDamage(string actorPartyMemberSelector, string targetPartyMemberSelector)
            string actorPartyMemberSelector = (string)args[0];
            string targetPartyMemberSelector = (string)args[1];

            PartyMember actor = getPartyMemberFromSelector(actorPartyMemberSelector);
            PartyMember selectedTarget = getPartyMemberFromSelector(targetPartyMemberSelector);

            List<PartyMember> targets;
            if (actor.EquippedWeapon == null)
            {
                targets = new List<PartyMember>(1);
                targets.Add(selectedTarget);
            }
            else
            {
                targets = getTargetsFromEnhancement(selectedTarget, actor.EquippedWeapon.Attributes.Enhancement);
                if (actor.EquippedWeapon.Attributes.Enhancement == Enhancement.Explosive)
                    ParticleEffect.AddExplosion(selectedTarget.BattleEntity.GetCenter(), battle);
            }

            int damage = -1;
            foreach (PartyMember target in targets)
            {
                bool breakNow = false;
                if (actor.EquippedWeapon.Attributes.Enhancement == Enhancement.Relentless)
                {
                    if (damage < 0)
                        damage = target.CalculateDamageTaken(actor);
                    int damageDealt = Math.Min(damage, target.Health);
                    target.DoDamage(damage, false);
                    damage -= damageDealt;
                    if (damage <= 0)
                        breakNow = true;
                }
                else
                {
                    damage = target.CalculateDamageTaken(actor);
                    target.DoDamage(damage, false);
                }

                if (actor.EquippedWeapon != null && target.Alive)
                    target.ApplyStatusEffectsFromAttributes(actor, actor.EquippedWeapon.Attributes, battle);

                if (target.EquippedShield != null)
                {
                    if (actor.EquippedWeapon.Data.WeaponType == WeaponType.Melee)
                    {
                        if (target.EquippedShield.Attributes.Enhancement == Enhancement.Spiky)
                            actor.DoDamage(Math.Max(damage / 10, 1), true, false);
                        actor.ApplyStatusEffectsFromAttributes(target, target.EquippedShield.Attributes, battle);
                    }

                    if (target.EquippedShield.Data.Script != null)
                        addNestedScriptRunner(target.EquippedShield.Data.Script, 0.0f);
                }

                if (breakNow)
                    break;
            }

            if (actor.EquippedWeapon != null && actor.EquippedWeapon.Attributes.Handling == Handling.Uncomfortable)
                actor.DoDamage(Math.Max(actor.CalculateDamageTaken(actor) / 10, 1), true);
        }

        private void _addHealth(object[] args)
        { // addHealth(string partyMemberSelector, int amount)
            string partyMemberSelector = (string)args[0];
            int amount = (int)args[1];

            PartyMember partyMember = getPartyMemberFromSelector(partyMemberSelector);
            partyMember.DoDamage(-amount, true);
        }

        private void _addMaxHealthStatModifier(object[] args)
        { // addMaxHealthStatModifier(string partyMemberSelector, int turns, int amount)
            string partyMemberSelector = (string)args[0];
            int turns = (int)args[1];
            int amount = (int)args[2];

            PartyMember partyMember = getPartyMemberFromSelector(partyMemberSelector);
            partyMember.AddStatModifier(new StatModifier(turns, maxHealth: amount));
        }

        private void _addMeleeAttackStatModifier(object[] args)
        { // addMeleeAttackStatModifier(string partyMemberSelector, int turns, int amount)
            string partyMemberSelector = (string)args[0];
            int turns = (int)args[1];
            int amount = (int)args[2];

            PartyMember partyMember = getPartyMemberFromSelector(partyMemberSelector);
            partyMember.AddStatModifier(new StatModifier(turns, meleeAttack: amount));
        }

        private void _addRangedAttackStatModifier(object[] args)
        { // addRangedAttackStatModifier(string partyMemberSelector, int turns, int amount)
            string partyMemberSelector = (string)args[0];
            int turns = (int)args[1];
            int amount = (int)args[2];

            PartyMember partyMember = getPartyMemberFromSelector(partyMemberSelector);
            partyMember.AddStatModifier(new StatModifier(turns, rangedAttack: amount));
        }

        private void _addDefenceStatModifier(object[] args)
        { // addDefenceStatModifier(string partyMemberSelector, int turns, int amount)
            string partyMemberSelector = (string)args[0];
            int turns = (int)args[1];
            int amount = (int)args[2];

            PartyMember partyMember = getPartyMemberFromSelector(partyMemberSelector);
            partyMember.AddStatModifier(new StatModifier(turns, defence: amount));
        }

        private void _addSpeedStatModifier(object[] args)
        { // addSpeedStatModifier(string partyMemberSelector, int turns, int amount)
            string partyMemberSelector = (string)args[0];
            int turns = (int)args[1];
            int amount = (int)args[2];

            PartyMember partyMember = getPartyMemberFromSelector(partyMemberSelector);
            partyMember.AddStatModifier(new StatModifier(turns, speed: amount));
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
            case StatusEffectType.Doom: statusEffect = new StatusEffects.Doom(actor); break;
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

        private void _setAcceleration(object[] args)
        { // setAcceleration(string partyMemberSelector, float x, float y)
            string partyMemberSelector = (string)args[0];
            float x = (float)args[1];
            float y = (float)args[2];

            getPartyMemberFromSelector(partyMemberSelector).BattleEntity.Acceleration = new Vector2(x, y);
        }

        private void _setAccelerationX(object[] args)
        { // setAccelerationX(string partyMemberSelector, float x)
            string partyMemberSelector = (string)args[0];
            float x = (float)args[1];

            Entity battleEntity = getPartyMemberFromSelector(partyMemberSelector).BattleEntity;
            battleEntity.Acceleration = new Vector2(x, battleEntity.Acceleration.Y);
        }

        private void _setAccelerationY(object[] args)
        { // setAccelerationY(string partyMemberSelector, float y)
            string partyMemberSelector = (string)args[0];
            float y = (float)args[1];

            Entity battleEntity = getPartyMemberFromSelector(partyMemberSelector).BattleEntity;
            battleEntity.Acceleration = new Vector2(battleEntity.Acceleration.X, y);
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

        private void _moveTo(object[] args)
        { // moveTo(string actorPartyMemberSelector, string targetPartyMemberSelector, float speed, bool accelerate, Script callback)
            string actorPartyMemberSelector = (string)args[0];
            string targetPartyMemberSelector = (string)args[1];
            float speed = (float)args[2];
            bool accelerate = (bool)args[3];
            Script callback = (Script)args[4];

            PartyMember actor = getPartyMemberFromSelector(actorPartyMemberSelector);
            PartyMember target = getPartyMemberFromSelector(targetPartyMemberSelector);
            Entity actorEntity = actor.BattleEntity;
            Entity targetEntity = target.BattleEntity;
            Vector2 velocityVector = targetEntity.Position - actorEntity.Position;
            velocityVector.Normalize();
            velocityVector *= speed;
            if (accelerate)
                actorEntity.Acceleration = velocityVector;
            else
                actorEntity.Velocity = velocityVector;

            this.blocked = true;
            ScriptRunner self = this;
            actorEntity.UpdateExtensions.Add(new UpdateExtension((updateExtension, delta) => {
                if (actorEntity.CollidesWith(targetEntity))
                {
                    updateExtension.Active = false;
                    self.blocked = false;
                    TriggerTrap(actor, target);
                    if (actor.Alive)
                        addNestedScriptRunner(callback, 0.0f);
                }
            }));
        }

        private void _moveToIdlePosition(object[] args)
        { // moveToIdlePosition(string partyMemberSelector, bool decelerate, Script callback)
            string partyMemberSelector = (string)args[0];
            float speed = (float)args[1];
            bool decelerate = (bool)args[2];
            Script callback = (Script)args[3];

            PartyMember partyMember = getPartyMemberFromSelector(partyMemberSelector);
            Entity entity = partyMember.BattleEntity;
            Vector2 directionVector = partyMember.BattleEntityIdlePosition - entity.Position;
            bool movingRight = directionVector.X > 0.0f;
            float distance = directionVector.Length();
            directionVector.Normalize();
            entity.Velocity = directionVector * speed;
            float time = distance / speed;
            Vector2 originalVelocity = entity.Velocity;

            this.blocked = true;
            bool unblocked = false;
            const float velocityToleranceForUnblocking = 50.0f;
            const float velocityToleranceForStopping = 1.0f;
            ScriptRunner self = this;
            entity.UpdateExtensions.Add(new UpdateExtension((updateExtension, delta) => {
                if (decelerate)
                {
                    float currentDistance = (partyMember.BattleEntityIdlePosition - entity.Position).Length();
                    entity.Velocity = originalVelocity * (currentDistance / distance);
                }
                else
                    time -= delta.Time;

                if (!unblocked && (time <= 0.0f || (movingRight && entity.Velocity.X < velocityToleranceForUnblocking) || (!movingRight && entity.Velocity.X > -velocityToleranceForUnblocking)))
                {
                    self.blocked = false;
                    unblocked = true;
                    addNestedScriptRunner(callback, 0.0f);
                }

                if (unblocked && (time <= 0.0f || (movingRight && entity.Velocity.X < velocityToleranceForStopping) || (!movingRight && entity.Velocity.X > -velocityToleranceForStopping)))
                {
                    entity.Position = partyMember.BattleEntityIdlePosition;
                    updateExtension.Active = false;
                }
            }));
        }

        private void _setIdleAnimation(object[] args)
        { // setIdleAnimation(string partyMemberSelector, string animationName)
            string partyMemberSelector = (string)args[0];
            string animationName = (string)args[1];

            PartyMember partyMember = getPartyMemberFromSelector(partyMemberSelector);
            partyMember.SetBattleEntityIdleAnimationNameOverride(animationName);
        }

        private void _createTrap(object[] args)
        { // createTrap(string actorPartyMemberSelector, string targetPartyMemberSelector, string spriteName)
            string actorPartyMemberSelector = (string)args[0];
            string targetPartyMemberSelector = (string)args[1];
            string spriteName = (string)args[2];

            PartyMember actorPartyMember = getPartyMemberFromSelector(actorPartyMemberSelector);
            PartyMember frontPartyMember = getPartyMemberFromSelector(targetPartyMemberSelector + ">list>front");
            float nudgeMultiplier = battle.GetPartyForPartyMember(actorPartyMember) == battle.PlayerParty ? 1.0f : -1.0f;
            Vector2 position = frontPartyMember.BattleEntity.Position + new Vector2(frontPartyMember.BattleEntity.GetBoundingBox().Width * nudgeMultiplier, 0.0f);

            Trap trap = new Trap(ResourceManager.GetNewSprite(spriteName), position, actorPartyMember.EquippedWeapon.Attributes, actorPartyMember.EquippedWeapon.Data, actorPartyMember, battle);
            battle.GetPartyBattleLayoutForPartyMember(frontPartyMember).PlaceTrapInFrontOfPartyMember(frontPartyMember, trap);
            Scene.AddEntity(trap);
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
                case "list": return battle.GetPartyBattleLayoutForPartyMember(partyMember).GetListWithPartyMember(partyMember);
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

        private List<PartyMember> getTargetsFromEnhancement(PartyMember target, Enhancement enhancement)
        {
            PartyBattleLayout layout = battle.GetPartyBattleLayoutForPartyMember(target);
            List<PartyMember> result;
            switch (enhancement)
            {
            case Enhancement.Piercing:
            case Enhancement.Relentless:
                result = new List<PartyMember>(layout.GetListWithPartyMember(target)); // new list isn't created in PartyBattleLayout#PartyMembersList
                break;
            case Enhancement.Explosive:
                result = layout.PartyMembersArea(target);
                break;
            case Enhancement.Inaccurate:
                const double chanceOfMiss = 0.4;
                result = new List<PartyMember>(1);
                if (Game1.Random.NextDouble() <= chanceOfMiss)
                {
                    List<PartyMember> targetList = layout.GetListWithPartyMember(target);
                    List<PartyMember> otherList = layout.RelativeList(targetList, Game1.Random.Next(1) == 0 ? 1 : -1);
                    if (otherList != null && otherList.Count > 0)
                        result.Add(otherList[0]);
                }
                else
                    result.Add(target);
                break;
            default:
                result = new List<PartyMember>(1);
                result.Add(target);
                break;
            }
            return result;
        }

        private void TriggerTrap(PartyMember actor, PartyMember target)
        {
            PartyBattleLayout battleLayout = battle.GetPartyBattleLayoutForPartyMember(target);
            Trap trap = battleLayout.GetTrapInFrontOfPartyMember(target);
            if (trap != null)
                trap.Trigger(actor, battleLayout);
        }
        #endregion
    }
}
