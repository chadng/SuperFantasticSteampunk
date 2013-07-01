using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.StatusEffects.Utils
{
    class ShudderManager
    {
        #region Instance Fields
        public readonly int ShudderCount;
        public readonly float ShudderTime;
        #endregion

        #region Instance Methods
        public int ShudderCounter { get; private set; }
        public float ShudderTimer { get; private set; }
        public Vector2 PartyMemberStartPosition { get; private set; }
        public bool Finished { get; private set; }
        #endregion

        #region Constructors
        public ShudderManager(int shudderCount, float shudderTime)
        {
            ShudderCount = shudderCount;
            ShudderTime = shudderTime;
            Reset(null);
        }
        #endregion

        #region Instance Methods
        public void Reset(PartyMember partyMember)
        {
            ShudderCounter = 0;
            ShudderTimer = 0.0f;
            Finished = false;
            if (partyMember != null)
                PartyMemberStartPosition = partyMember.BattleEntity.Position;
        }

        public void Update(PartyMember partyMember, Delta delta)
        {
            if (Finished)
                return;
            ShudderTimer += delta.Time;
            if (ShudderTimer >= ShudderTime)
            {
                ShudderTimer = 0.0f;
                if (++ShudderCounter > ShudderCount)
                {
                    partyMember.BattleEntity.Position = PartyMemberStartPosition;
                    Finished = true;
                }
            }
        }
        #endregion
    }
}
