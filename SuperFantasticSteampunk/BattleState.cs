﻿using System;
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

        public virtual void Finish()
        {
            if (BattleStateRenderer != null)
                BattleStateRenderer.Finish();
        }

        public virtual void Pause()
        {
            if (BattleStateRenderer != null)
                BattleStateRenderer.Pause();
        }

        public virtual void Resume(BattleState previousBattleState)
        {
            if (BattleStateRenderer != null)
                BattleStateRenderer.Resume();
        }

        public abstract void Update(Delta delta);

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

        protected Inventory getInventoryFromThinkActionType(ThinkActionType thinkActionType, PartyMember partyMember)
        {
            Party party = Battle.GetPartyForPartyMember(partyMember);
            if (party == null)
                return null;
            switch (thinkActionType)
            {
            case ThinkActionType.Attack: return party.WeaponInventories[partyMember.CharacterClass];
            case ThinkActionType.Defend: return party.ShieldInventory;
            case ThinkActionType.UseItem: return party.ItemInventory;
            default: return null;
            }
        }

        private void removeDeadPartyMembers(Party party, Party otherParty)
        {
            for (int i = party.Count - 1; i >= 0; --i)
            {
                if (!party[i].Alive)
                    party[i].Kill(Battle);
            }
        }
        #endregion
    }
}
