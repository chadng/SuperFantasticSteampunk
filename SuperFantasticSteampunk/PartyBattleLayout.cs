using System;
using System.Collections.Generic;

namespace SuperFantasticSteampunk
{
    class PartyBattleLayout
    {
        #region Instance Fields
        private Party party;
        private List<List<PartyMember>> layout;
        private Trap[] traps;
        #endregion

        #region Instance Properties
        public int ListCount
        {
            get { return layout.Count; }
        }
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
            traps = new Trap[layout.Count];
            resetTraps();
        }
        #endregion

        #region Instance Methods
        public void FinishBattle()
        {
            resetTraps();
        }

        public void ArrangeFromString(string str)
        {
            foreach (List<PartyMember> list in layout)
                list.Clear();

            str = str.Replace(" ", "");
            string[] lines = str.Split('\n');
            List<PartyMember> partyMembersLeftToAdd = new List<PartyMember>(party);
            for (int i = 0; i < layout.Count && i < lines.Length; ++i)
            {
                string[] indexes = lines[i].Split(',');
                foreach (string indexStr in indexes)
                {
                    int index = int.Parse(indexStr);
                    if (index < 0 || index >= party.Count)
                        continue;
                    if (partyMembersLeftToAdd.Contains(party[index]))
                    {
                        layout[i].Add(party[index]);
                        partyMembersLeftToAdd.Remove(party[index]);
                    }
                }
            }

            layout[0].AddRange(partyMembersLeftToAdd);
        }

        public void MovePartyMemberUp(PartyMember partyMember, BattleStates.Think thinkState)
        {
            if (!partyMember.HasStatusEffectOfType(StatusEffectType.Paralysis))
                movePartyMemberAcrossLists(partyMember, true, -1, thinkState);
        }

        public void MovePartyMemberDown(PartyMember partyMember, BattleStates.Think thinkState)
        {
            if (!partyMember.HasStatusEffectOfType(StatusEffectType.Paralysis))
                movePartyMemberAcrossLists(partyMember, false, 1, thinkState);
        }

        public void MovePartyMemberBack(PartyMember partyMember, BattleStates.Think thinkState) // last item/back is towards edge of screen
        {
            if (!partyMember.HasStatusEffectOfType(StatusEffectType.Paralysis))
                movePartyMemberWithinList(partyMember, false, 1, thinkState);
        }

        public void MovePartyMemberForward(PartyMember partyMember, BattleStates.Think thinkState) // first item/forward is towards middle of screen
        {
            if (!partyMember.HasStatusEffectOfType(StatusEffectType.Paralysis))
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

        public void PlaceTrapInFrontOfPartyMember(PartyMember partyMember, Trap trap)
        {
            int index = layout.IndexOf(GetListWithPartyMember(partyMember));
            if (index < 0 || index >= traps.Length)
                return;
            if (traps[index] != null)
                traps[index].Kill();
            traps[index] = trap;
        }

        public void RemoveTrap(Trap trap)
        {
            int index = traps.IndexOf(trap);
            if (index >= 0 && index < traps.Length)
                traps[index] = null;
        }

        public List<PartyMember> PartyMembersArea(PartyMember partyMember)
        {
            List<PartyMember> result = new List<PartyMember>(5);
            result.Add(partyMember);

            List<PartyMember> partyMemberList = GetListWithPartyMember(partyMember);
            int partyMemberIndex = partyMemberList.IndexOf(partyMember);
            if (partyMemberIndex > 0)
                result.Add(partyMemberList[partyMemberIndex - 1]);
            if (partyMemberIndex + 1 < partyMemberList.Count)
                result.Add(partyMemberList[partyMemberIndex + 1]);

            List<PartyMember> adjacentPartyMemberList = RelativeList(partyMemberList, 1);
            if (adjacentPartyMemberList != null && partyMemberIndex < adjacentPartyMemberList.Count)
                result.Add(adjacentPartyMemberList[partyMemberIndex]);
            adjacentPartyMemberList = RelativeList(partyMemberList, -1);
            if (adjacentPartyMemberList != null && partyMemberIndex < adjacentPartyMemberList.Count)
                result.Add(adjacentPartyMemberList[partyMemberIndex]);

            return result;
        }

        public bool PartyMemberInFrontLine(PartyMember partyMember)
        {
            if (party.Contains(partyMember))
                return GetListWithPartyMember(partyMember).IndexOf(partyMember) == 0;
            return false;
        }

        public List<PartyMember> GetListWithPartyMember(PartyMember partyMember)
        {
            foreach (List<PartyMember> list in layout)
            {
                if (list.Contains(partyMember))
                    return list;
            }
            throw new Exception("PartyMember not found in PartyBattleLayout");
        }

        public List<PartyMember> GetListBehindTrap(Trap trap)
        {
            int index = traps.IndexOf(trap);
            if (index < 0 || index >= layout.Count)
                throw new Exception("Trap not found in PartyBattleLayout");
            return layout[index];
        }

        public Trap GetTrapInFrontOfPartyMember(PartyMember partyMember)
        {
            List<PartyMember> list = GetListWithPartyMember(partyMember);
            int index = layout.IndexOf(list);
            if (index < 0 || index >= traps.Length)
                return null;
            return traps[index];
        }

        public List<PartyMember> GetListAt(int index)
        {
            if (index < 0 || index >= layout.Count)
                return null;
            return layout[index];
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

        public int IndexOfList(List<PartyMember> list)
        {
            return layout.IndexOf(list);
        }

        private List<PartyMember> newListWithPartyMember(PartyMember partyMember)
        {
            if (partyMember == null)
                throw new Exception("PartyMember cannot be null");
            List<PartyMember> result = new List<PartyMember>();
            result.Add(partyMember);
            return result;
        }

        private void movePartyMemberAcrossLists(PartyMember partyMember, bool edgeListIndexIsZero, int nextListRelativeIndex, BattleStates.Think thinkState)
        {
            if (!party.Contains(partyMember))
                return;

            List<PartyMember> list = GetListWithPartyMember(partyMember);
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

            List<PartyMember> list = GetListWithPartyMember(partyMember);
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

        private void resetTraps()
        {
            for (int i = 0; i < traps.Length; ++i)
            {
                if (traps[i] != null)
                    traps[i].Kill();
                traps[i] = null;
            }
        }
        #endregion
    }
}
