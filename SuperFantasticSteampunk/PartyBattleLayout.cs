using System;
using System.Collections.Generic;

namespace SuperFantasticSteampunk
{
    class PartyBattleLayout
    {
        #region Instance Fields
        private Party party;
        private List<List<PartyMember>> layout;
        #endregion

        #region Constructors
        public PartyBattleLayout(Party party)
        {
            if (party == null)
                throw new Exception("Party cannot be null");
            if (party.Count == 0)
                throw new Exception("Party cannot be empty");

            this.party = party;
            layout = new List<List<PartyMember>>(party.Count);
            foreach (PartyMember partyMember in party)
                layout.Add(newListWithPartyMember(partyMember));
        }
        #endregion

        #region Instance Methods
        public void MovePartyMemberUp(PartyMember partyMember, BattleStates.Think thinkState)
        {
            if (!partyMember.HasStatusEffect(StatusEffectType.Paralysis))
                movePartyMemberAcrossLists(partyMember, true, -1, thinkState);
        }

        public void MovePartyMemberDown(PartyMember partyMember, BattleStates.Think thinkState)
        {
            if (!partyMember.HasStatusEffect(StatusEffectType.Paralysis))
                movePartyMemberAcrossLists(partyMember, false, 1, thinkState);
        }

        public void MovePartyMemberBack(PartyMember partyMember, BattleStates.Think thinkState) // last item/back is towards edge of screen
        {
            if (!partyMember.HasStatusEffect(StatusEffectType.Paralysis))
                movePartyMemberWithinList(partyMember, false, 1, thinkState);
        }

        public void MovePartyMemberForward(PartyMember partyMember, BattleStates.Think thinkState) // first item/forward is towards middle of screen
        {
            if (!partyMember.HasStatusEffect(StatusEffectType.Paralysis))
                movePartyMemberWithinList(partyMember, true, -1, thinkState);
        }

        public void RemovePartyMember(PartyMember partyMember)
        {
            foreach (List<PartyMember> list in layout)
            {
                if (list.Remove(partyMember))
                    break;
            }
        }

        public List<PartyMember> PartyMembersList(PartyMember partyMember)
        {
            return getListWithPartyMember(partyMember);
        }

        public bool PartyMemberInFrontLine(PartyMember partyMember)
        {
            if (party.Contains(partyMember))
                return getListWithPartyMember(partyMember).IndexOf(partyMember) == 0;
            return false;
        }

        public void ForEachList(Action<List<PartyMember>> action)
        {
            layout.ForEach(action);
        }

        public List<PartyMember> RelativeList(List<PartyMember> list, int relativeIndex)
        {
            int index = layout.IndexOf(list) + relativeIndex;
            if (index < 0 || index >= layout.Count)
                return null;
            else
                return layout[index];
        }

        private List<PartyMember> newListWithPartyMember(PartyMember partyMember)
        {
            if (partyMember == null)
                throw new Exception("PartyMember cannot be null");
            List<PartyMember> result = new List<PartyMember>();
            result.Add(partyMember);
            return result;
        }

        private List<PartyMember> getListWithPartyMember(PartyMember partyMember)
        {
            foreach (List<PartyMember> list in layout)
            {
                if (list.Contains(partyMember))
                    return list;
            }
            throw new Exception("PartyMember not found in PartyBattleLayout");
        }

        private void movePartyMemberAcrossLists(PartyMember partyMember, bool edgeListIndexIsZero, int nextListRelativeIndex, BattleStates.Think thinkState)
        {
            if (!party.Contains(partyMember))
                return;

            List<PartyMember> list = getListWithPartyMember(partyMember);
            int listIndex = layout.IndexOf(list);
            if (listIndex == (edgeListIndexIsZero ? 0 : layout.Count - 1)) // if the edge list
                return;

            List<PartyMember> nextList = layout[listIndex + nextListRelativeIndex];
            int partyMemberPosition = Math.Min(list.IndexOf(partyMember), nextList.Count);
            list.Remove(partyMember);
            if (partyMemberPosition == 0 && nextList.Count > 0 && thinkState.PartyMemberHasCompletedThinkAction(nextList[partyMemberPosition]))
            {
                Weapon frontPartyMemberWeapon = nextList[partyMemberPosition].EquippedWeapon;
                if (frontPartyMemberWeapon != null && frontPartyMemberWeapon.Data.WeaponType == WeaponType.Melee)
                    ++partyMemberPosition;
            }
            nextList.Insert(partyMemberPosition, partyMember);
        }

        private void movePartyMemberWithinList(PartyMember partyMember, bool edgeIndexIsZero, int nextRelativeIndex, BattleStates.Think thinkState)
        {
            if (!party.Contains(partyMember))
                return;

            List<PartyMember> list = getListWithPartyMember(partyMember);
            int partyMemberIndex = list.IndexOf(partyMember);
            if (partyMemberIndex != (edgeIndexIsZero ? 0 : list.Count - 1)) // if not at the end already
            {
                list.RemoveAt(partyMemberIndex);
                int index = partyMemberIndex + nextRelativeIndex;
                if (index == 0 && list.Count > 0 && thinkState.PartyMemberHasCompletedThinkAction(list[index]))
                {
                    Weapon frontPartyMemberWeapon = list[index].EquippedWeapon;
                    if (frontPartyMemberWeapon != null && frontPartyMemberWeapon.Data.WeaponType == WeaponType.Melee)
                        ++index;
                }
                list.Insert(index, partyMember);
            }
        }
        #endregion
    }
}
