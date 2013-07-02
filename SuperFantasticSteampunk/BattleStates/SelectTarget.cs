using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.BattleStates
{
    class SelectTarget : BattleState
    {
        #region Instance Fields
        private ThinkAction thinkAction;
        private bool isEnemy;
        private PartyBattleLayout partyBattleLayout;
        private InputButtonListener inputButtonListener;
        #endregion

        #region Instance Properties
        public PartyMember Actor
        {
            get { return thinkAction.Actor; }
        }

        public PartyMember PotentialTarget { get; private set; }

        public override bool KeepPartyMembersStatic
        {
            get { return true; }
        }
        #endregion

        #region Constructors
        public SelectTarget(Battle battle, ThinkAction thinkAction)
            : base(battle)
        {
            if (thinkAction == null)
                throw new Exception("ThinkAction cannot be null");
            this.thinkAction = thinkAction;
            isEnemy = Battle.GetPartyForPartyMember(thinkAction.Actor) == Battle.EnemyParty;
            inputButtonListener = isEnemy ? null : new InputButtonListener(new Dictionary<InputButton, ButtonEventHandlers> {
                { InputButton.Up, new ButtonEventHandlers(down: chooseTargetUp) },
                { InputButton.Down, new ButtonEventHandlers(down: chooseTargetDown) },
                { InputButton.Left, new ButtonEventHandlers(down: chooseTargetLeft) },
                { InputButton.Right, new ButtonEventHandlers(down: chooseTargetRight) },
                { InputButton.A, new ButtonEventHandlers(up: selectTarget) },
                { InputButton.B, new ButtonEventHandlers(up: cancelTargetSelection) }
            });
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            if (isEnemy)
                thinkAction.Target = Think.ChooseTargetForEnemyPartyMember(Battle);
            else
            {
                partyBattleLayout = null;
                if (thinkAction.Type == ThinkActionType.Attack)
                {
                    WeaponData weaponData = ResourceManager.GetWeaponData(thinkAction.OptionName);
                    if (weaponData != null && weaponData.WeaponUseAgainst == WeaponUseAgainst.Enemy)
                    {
                        partyBattleLayout = Battle.EnemyPartyLayout;
                        PotentialTarget = Battle.EnemyParty[0];
                    }
                }

                if (partyBattleLayout == null)
                {
                    partyBattleLayout = Battle.PlayerPartyLayout;
                    PotentialTarget = Battle.PlayerParty[0];
                }

                BattleStateRenderer = new SelectTargetRenderer(this);
            }
        }

        public override void Finish()
        {
            base.Finish();
            PopState();
        }

        public override void Update(Delta delta)
        {
            if (isEnemy || PotentialTarget == null || thinkAction.Type == ThinkActionType.None || thinkAction.Type == ThinkActionType.Defend)
            {
                Finish();
                return;
            }

            inputButtonListener.Update(delta);
        }

        private void chooseTargetUp()
        {
            chooseTargetAcrossLists(-1);
        }

        private void chooseTargetDown()
        {
            chooseTargetAcrossLists(1);
        }

        private void chooseTargetLeft()
        {
            int relativeIndex = partyBattleLayout == Battle.PlayerPartyLayout ? 1 : -1;
            chooseTargetWithinList(relativeIndex);
        }

        private void chooseTargetRight()
        {
            int relativeIndex = partyBattleLayout == Battle.PlayerPartyLayout ? -1 : 1;
            chooseTargetWithinList(relativeIndex);
        }

        private void chooseTargetAcrossLists(int relativeIndex, int currentIndex = -1, List<PartyMember> originalList = null)
        {
            List<PartyMember> partyMemberList;
            if (currentIndex < 0 || currentIndex >= partyBattleLayout.ListCount)
                partyMemberList = partyBattleLayout.GetListWithPartyMember(PotentialTarget);
            else
                partyMemberList = partyBattleLayout.GetListAt(currentIndex);

            int nextListIndex = partyBattleLayout.IndexOfList(partyMemberList) + relativeIndex;
            if (nextListIndex < 0 || nextListIndex >= partyBattleLayout.ListCount)
                return;

            List<PartyMember> nextPartyMemberList = partyBattleLayout.RelativeList(partyMemberList, relativeIndex);
            if (nextPartyMemberList == null || nextPartyMemberList.Count == 0) // A list in the middle might be empty so check next list
            {
                chooseTargetAcrossLists(relativeIndex, nextListIndex);
                return;
            }

            int index = partyBattleLayout.GetListWithPartyMember(PotentialTarget).IndexOf(PotentialTarget);
            if (index >= nextPartyMemberList.Count)
                index = nextPartyMemberList.Count - 1;

            PotentialTarget = nextPartyMemberList[index];
        }

        private void chooseTargetWithinList(int relativeIndex)
        {
            List<PartyMember> partyMemberList = partyBattleLayout.GetListWithPartyMember(PotentialTarget);
            int nextIndex = partyMemberList.IndexOf(PotentialTarget) + relativeIndex;
            if (nextIndex < 0 || nextIndex >= partyMemberList.Count)
                return;
            PotentialTarget = partyMemberList[nextIndex];
        }

        private void selectTarget()
        {
            thinkAction.Target = PotentialTarget;
            Finish();
        }

        private void cancelTargetSelection()
        {
            thinkAction.Target = null;
            Finish();
        }
        #endregion
    }
}
