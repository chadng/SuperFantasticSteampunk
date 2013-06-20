using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.StatusEffects
{
    class PoisonBubble : Entity
    {
        #region Constants
        private const float alphaSpeed = 0.75f;
        #endregion

        #region Instance Fields
        private TextureData textureData;
        private float alpha;
        #endregion

        #region Constructors
        public PoisonBubble(Vector2 position, TextureData textureData)
            : base(position)
        {
            this.textureData = textureData;
            alpha = 1.0f;
            Velocity = new Vector2(0.0f, -200.0f);
            ZIndex = -1;
        }
        #endregion

        #region Instance Methods
        public override void Update(Delta delta)
        {
            base.Update(delta);
            alpha -= alphaSpeed * delta.Time;
            if (alpha <= 0.0f)
                Kill();
        }

        public override void Draw(Renderer renderer)
        {
            renderer.Draw(textureData, Position, new Color(1.0f, 1.0f, 1.0f, alpha), 0.0f, new Vector2(0.3f));
        }
        #endregion
    }

    class Poison : StatusEffect
    {
        #region Constants
        private const float tintAlphaSpeed = 1.2f;
        private const float bubbleParticleTime = 0.25f;
        #endregion

        #region Instance Fields
        private bool finished;
        private bool damageDone;
        private float tintAlpha;
        private bool incTintAlpha;
        private float bubbleParticleTimer;
        private TextureData bubbleParticleTextureData;
        #endregion

        #region Instance Properties
        public override StatusEffectType Type
        {
            get { return StatusEffectType.Poison; }
        }

        public override bool Active
        {
            get { return true; }
        }
        #endregion

        #region Constructors
        public Poison()
        {
            resetFieldsForUpdate();
            bubbleParticleTextureData = ResourceManager.GetTextureData("particles/poison_bubble");
        }
        #endregion

        #region Instance Methods
        public override void EndTurnStart(PartyMember partyMember)
        {
            base.EndTurnStart(partyMember);
            resetFieldsForUpdate();
        }

        public override void EndTurnUpdate(PartyMember partyMember, Delta delta)
        {
            base.EndTurnUpdate(partyMember, delta);

            updateTintAlpha(delta);
            updateBubbleParticles(partyMember, delta);

            if (!incTintAlpha && !damageDone)
            {
                int damage = partyMember.Health / 8;
                partyMember.DoDamage(damage > 0 ? damage : 1, true);
                damageDone = true;
                if (!partyMember.Alive)
                    finished = true;
            }

            partyMember.BattleEntity.Tint = finished ? Color.White : Color.Lerp(Color.White, Color.Purple, tintAlpha);
        }

        public override bool EndTurnIsFinished()
        {
            return finished;
        }

        private void resetFieldsForUpdate()
        {
            finished = false;
            damageDone = false;
            tintAlpha = 0.0f;
            incTintAlpha = true;
            bubbleParticleTimer = 0.0f;
        }

        private void updateTintAlpha(Delta delta)
        {
            if (incTintAlpha)
            {
                tintAlpha += tintAlphaSpeed * delta.Time;
                if (tintAlpha >= 1.0f)
                {
                    tintAlpha = 1.0f;
                    incTintAlpha = false;
                }
            }
            else
            {
                tintAlpha -= tintAlphaSpeed * delta.Time;
                if (tintAlpha <= 0.0f)
                {
                    tintAlpha = 0.0f;
                    finished = true;
                }
            }
        }

        private void updateBubbleParticles(PartyMember partyMember, Delta delta)
        {
            bubbleParticleTimer += delta.Time;
            if (bubbleParticleTimer >= bubbleParticleTime)
            {
                bubbleParticleTimer = 0.0f;
                Rectangle boundingBox = partyMember.BattleEntity.GetBoundingBox();
                float x = boundingBox.X + (boundingBox.Width / 2) + Game1.Random.Next(boundingBox.Width / 2) - (boundingBox.Width / 4);
                float y = boundingBox.Y + (boundingBox.Height / 2) + (Game1.Random.Next(boundingBox.Height / 4) - (boundingBox.Height / 8));
                Scene.AddEntity(new PoisonBubble(new Vector2(x, y), bubbleParticleTextureData));
            }
        }
        #endregion
    }
}
