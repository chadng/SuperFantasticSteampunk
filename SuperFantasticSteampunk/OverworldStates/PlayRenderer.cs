using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.OverworldStates
{
    class PlayRenderer : OverworldStateRenderer
    {
        #region Constants
        private const float flashTime = 0.1f;
        #endregion

        #region Instance Fields
        private float flashTimer;
        private bool alphaFull;
        private bool playerInvincibleBefore;
        #endregion

        #region Instance Properties
        private Entity playerOverworldEntity
        {
            get { return overworldState.Overworld.PlayerParty.PrimaryPartyMember.OverworldEntity; }
        }
        #endregion

        #region Constructors
        public PlayRenderer(OverworldState overworldState)
            : base(overworldState)
        {
            resetFlash();
            playerInvincibleBefore = false;
        }
        #endregion

        #region Instance Methods
        public override void Resume()
        {
            resetFlash();
            base.Resume();
        }

        public override void Update(Delta delta)
        {
            if (overworldState.Overworld.PlayerIsInvincible)
            {
                updatePlayerInvincibilityFlashing(delta);
                playerInvincibleBefore = true;
            }
            else if (playerInvincibleBefore)
            {
                playerOverworldEntity.Tint = Color.White;
                playerInvincibleBefore = false;
            }
        }

        public override void BeforeDraw(Renderer renderer)
        {
        }

        public override void AfterDraw(Renderer renderer)
        {
        }

        private void resetFlash()
        {
            flashTimer = 0.0f;
            alphaFull = true;
        }

        private void updatePlayerInvincibilityFlashing(Delta delta)
        {
            flashTimer += delta.Time;
            if (flashTimer >= flashTime)
            {
                flashTimer = 0.0f;
                alphaFull = !alphaFull;
                playerOverworldEntity.Tint = Color.White * (alphaFull ? 1.0f : 0.6f);
            }
        }
        #endregion
    }
}
