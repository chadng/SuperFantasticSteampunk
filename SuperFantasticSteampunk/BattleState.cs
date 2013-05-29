using System;
using Microsoft.Xna.Framework;
using Spine;
using SuperFantasticSteampunk.BattleStates;

namespace SuperFantasticSteampunk
{
    abstract class BattleState
    {
        #region Instance Properties
        public Battle Battle { get; private set; }
        public BattleStateRenderer BattleStateRenderer { get; protected set; }

        public virtual bool KeepPartyMembersStatic
        {
            get { return false; }
        }
        #endregion

        #region Constructors
        protected BattleState(Battle battle)
        {
            if (battle == null)
                throw new Exception("Battle cannot be null");
            Battle = battle;
        }
        #endregion

        #region Instance Methods
        public abstract void Start();
        public abstract void Finish();

        public virtual void Pause()
        {
        }

        public virtual void Resume(BattleState previousBattleState)
        {
        }

        public abstract void Update(GameTime gameTime);

        public void ChangeState(BattleState state)
        {
            if (Battle.CurrentBattleState == this)
                Battle.ChangeState(state);
        }

        public void PushState(BattleState state)
        {
            if (Battle.CurrentBattleState == this)
                Battle.PushState(state);
        }

        public void PopState()
        {
            if (Battle.CurrentBattleState == this)
                Battle.PopState();
        }

        protected void removeDeadPartyMembers()
        {
            removeDeadPartyMembers(Battle.PlayerParty, Battle.EnemyParty);
            removeDeadPartyMembers(Battle.EnemyParty, Battle.PlayerParty);
        }

        protected Inventory getInventoryFromThinkActionType(ThinkActionType thinkActionType, CharacterClass characterClass)
        {
            switch (thinkActionType)
            {
            case ThinkActionType.Attack: return Battle.PlayerParty.WeaponInventories[characterClass];
            case ThinkActionType.Defend: return Battle.PlayerParty.ShieldInventory;
            case ThinkActionType.UseItem: return Battle.PlayerParty.ItemInventory;
            default: return null;
            }
        }

        private void removeDeadPartyMembers(Party party, Party otherParty)
        {
            for (int i = party.Count - 1; i >= 0; --i)
            {
                if (!party[i].Alive)
                {
                    if (otherParty == Battle.PlayerParty)
                    {
                        foreach (PartyMember partyMember in otherParty)
                        {
                            int experience = partyMember.CalculateExperienceGained(party[i]);
                            Scene.AddEntity(new FloatingText(experience.ToString() + " exp", Color.Gold, partyMember.BattleEntity.Position));
                            if (partyMember.AddExperience(experience))
                                Scene.AddEntity(new FloatingText("Level up!", Color.Blue, partyMember.BattleEntity.Position));
                        }
                    }

                    party[i].Kill(Battle);
                }
            }
        }
        #endregion
    }
}
