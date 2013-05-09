using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.BattleStates
{
    class EndTurn : BattleState
    {
        #region Instance Fields
        Party currentStatusEffectParty;
        int currentStatusEffectPartyMemberIndex;
        #endregion

        #region Constructors
        public EndTurn(Battle battle)
            : base(battle)
        {
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            currentStatusEffectParty = Battle.PlayerParty;
            currentStatusEffectPartyMemberIndex = 0;
        }

        public override void Finish()
        {
            List<string> exclude = new List<string> { "Scale" };
            foreach (PartyMember partyMember in Battle.PlayerParty)
            {
                partyMember.EndTurn();
                partyMember.BattleEntity.ResetManipulation(exclude);
            }
            foreach (PartyMember partyMember in Battle.EnemyParty)
            {
                partyMember.EndTurn();
                partyMember.BattleEntity.ResetManipulation(exclude);
            }

            removeDeadPartyMembers();

            if (Battle.PlayerParty.Count == 0)
                ChangeState(new Lose(Battle));
            else if (Battle.EnemyParty.Count == 0)
                ChangeState(new Win(Battle));
            else
                ChangeState(new Think(Battle));
        }

        public override void Resume(BattleState previousBattleState)
        {
            base.Resume(previousBattleState);
            if (previousBattleState is HandleStatusEffects)
                ++currentStatusEffectPartyMemberIndex;
        }

        public override void Update(GameTime gameTime)
        {
            if (currentStatusEffectPartyMemberIndex < currentStatusEffectParty.Count)
            {
                if (currentStatusEffectParty[currentStatusEffectPartyMemberIndex].Alive)
                    PushState(new HandleStatusEffects(Battle, StatusEffectEvent.EndTurn, partyMember: currentStatusEffectParty[currentStatusEffectPartyMemberIndex]));
                else
                    ++currentStatusEffectPartyMemberIndex;
            }
            else if (currentStatusEffectParty == Battle.PlayerParty)
            {
                currentStatusEffectParty = Battle.EnemyParty;
                currentStatusEffectPartyMemberIndex = 0;
            }
            else
                Finish();
        }
        #endregion
    }
}
