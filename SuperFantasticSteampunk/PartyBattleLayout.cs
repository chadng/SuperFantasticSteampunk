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
        public void MovePartyMemberUp(PartyMember partyMember)
        {
            if (!partyMember.HasStatusEffect(StatusEffectType.Paralysis))
                movePartyMemberAcrossLists(partyMember, true, -1);
        }

        public void MovePartyMemberDown(PartyMember partyMember)
        {
            if (!partyMember.HasStatusEffect(StatusEffectType.Paralysis))
                movePartyMemberAcrossLists(partyMember, false, 1);
        }

        public void MovePartyMemberBack(PartyMember partyMember) // last item/back is towards edge of screen
        {
            if (!partyMember.HasStatusEffect(StatusEffectType.Paralysis))
                movePartyMemberWithinList(partyMember, false, 1);
        }

        public void MovePartyMemberForward(PartyMember partyMember) // first item/forward is towards middle of screen
        {
            if (!partyMember.HasStatusEffect(StatusEffectType.Paralysis))
                movePartyMemberWithinList(partyMember, true, -1);
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

        private void movePartyMemberAcrossLists(PartyMember partyMember, bool edgeListIndexIsZero, int nextListRelativeIndex)
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
            nextList.Insert(partyMemberPosition, partyMember);
        }

        private void movePartyMemberWithinList(PartyMember partyMember, bool edgeIndexIsZero, int nextRelativeIndex)
        {
            if (!party.Contains(partyMember))
                return;

            List<PartyMember> list = getListWithPartyMember(partyMember);
            int partyMemberIndex = list.IndexOf(partyMember);
            if (partyMemberIndex != (edgeIndexIsZero ? 0 : list.Count - 1)) // if not at the end already
            {
                list.RemoveAt(partyMemberIndex);
                list.Insert(partyMemberIndex + nextRelativeIndex, partyMember);
            }
        }
        #endregion
    }
}
