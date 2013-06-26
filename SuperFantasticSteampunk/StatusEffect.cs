using SuperFantasticSteampunk.BattleStates;

namespace SuperFantasticSteampunk
{
    enum StatusEffectType { Poison, Paralysis, Fear, Doom }
    enum StatusEffectEvent { BeforeAct, EndTurn }

    abstract class StatusEffect
    {
        #region Instance Properties
        public abstract StatusEffectType Type { get; }
        public abstract bool Active { get; }
        public abstract TextureData TextureData { get; protected set; }
        #endregion

        #region Instance Methods
        public virtual void BeforeActStart(ThinkAction thinkAction)
        {
        }

        public virtual void BeforeActUpdate(ThinkAction thinkAction, Delta delta)
        {
        }

        public virtual void EndTurnStart(PartyMember partyMember)
        {
        }

        public virtual void EndTurnUpdate(PartyMember partyMember, Delta delta)
        {
        }

        public virtual bool BeforeActIsFinished()
        {
            return true;
        }

        public virtual bool EndTurnIsFinished()
        {
            return true;
        }
        #endregion
    }
}
