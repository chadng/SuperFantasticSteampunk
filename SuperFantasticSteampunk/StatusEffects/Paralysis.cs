using Microsoft.Xna.Framework;
using SuperFantasticSteampunk.BattleStates;

namespace SuperFantasticSteampunk.StatusEffects
{
    class Paralysis : StatusEffect
    {
        #region Constants
        private const int chanceOfEffect = 25;
        private const int shudderCount = 25;
        private const float shudderTime = 0.01f;
        #endregion

        #region Instance Fields
        private bool finished;
        private bool thinkActionActivationDecided;
        private int shudderCounter;
        private float shudderTimer;
        private Vector2 actorStartPosition;
        #endregion

        #region Instance Properties
        public override StatusEffectType Type
        {
            get { return StatusEffectType.Paralysis; }
        }

        public override bool Active
        {
            get { return true; }
        }

        public override TextureData TextureData { get; protected set; }
        #endregion

        #region Constructors
        public Paralysis()
        {
            resetFieldsForUpdate();
            TextureData = ResourceManager.GetTextureData("particles/paralysis");
        }
        #endregion

        #region Instance Methods
        public override void BeforeActStart(ThinkAction thinkAction)
        {
            base.BeforeActStart(thinkAction);
            resetFieldsForUpdate();
            actorStartPosition = thinkAction.Actor.BattleEntity.Position;
        }

        public override void BeforeActUpdate(ThinkAction thinkAction, Delta delta)
        {
            base.BeforeActUpdate(thinkAction, delta);

            if (!thinkActionActivationDecided)
            {
                if (Game1.Random.Next(100) <= chanceOfEffect)
                    thinkAction.Active = false;
                thinkActionActivationDecided = true;
            }
            else if (thinkAction.Active)
                finished = true;
            else
            {
                updateShudder(delta);
                bool shocked = shudderCounter % 2 == 0;
                thinkAction.Actor.BattleEntity.Position = actorStartPosition + (new Vector2(5.0f, 0.0f) * (shocked ? -1 : 1));
                thinkAction.Actor.BattleEntity.Tint = shocked ? Color.Yellow : Color.White;
                thinkAction.Actor.BattleEntity.PauseAnimation = true;
            }

            if (finished)
            {
                thinkAction.Actor.BattleEntity.Position = actorStartPosition;
                thinkAction.Actor.BattleEntity.Tint = Color.White;
                thinkAction.Actor.BattleEntity.PauseAnimation = false;
            }
        }

        public override bool BeforeActIsFinished()
        {
            return finished;
        }

        private void resetFieldsForUpdate()
        {
            finished = false;
            thinkActionActivationDecided = false;
            shudderCounter = 0;
            shudderTimer = 0.0f;
        }

        private void updateShudder(Delta delta)
        {
            shudderTimer += delta.Time;
            if (shudderTimer >= shudderTime)
            {
                shudderTimer = 0.0f;
                if (++shudderCounter > shudderCount)
                    finished = true;
            }
        }
        #endregion
    }
}
