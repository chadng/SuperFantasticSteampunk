using System;
using System.Collections.Generic;

namespace SuperFantasticSteampunk.BattleStates
{
    class MoveActor : BattleState
    {
        #region Instance Fields
        private Think thinkState;
        private InputButtonListener inputButtonListener;
        private List<PartyMember> originalActorList;
        private int originalActorListIndex;
        #endregion

        #region Instance Properties
        public PartyMember Actor { get; private set; }

        public override bool KeepPartyMembersStatic
        {
            get { return true; }
        }
        #endregion

        #region Constructors
        public MoveActor(Battle battle, PartyMember actor, Think thinkState)
            : base(battle)
        {
            if (actor == null)
                throw new Exception("PartyMember actor cannot be null");
            if (thinkState == null)
                throw new Exception("Think thinkState cannot be null");
            this.Actor = actor;
            this.thinkState = thinkState;

            inputButtonListener = new InputButtonListener(new Dictionary<InputButton, ButtonEventHandlers> {
                { InputButton.Up, new ButtonEventHandlers(down: moveActorUp) },
                { InputButton.Down, new ButtonEventHandlers(down: moveActorDown) },
                { InputButton.Left, new ButtonEventHandlers(down: moveActorBack) },
                { InputButton.Right, new ButtonEventHandlers(down: moveActorForward) },
                { InputButton.A, new ButtonEventHandlers(up: Finish) },
                { InputButton.B, new ButtonEventHandlers(up: cancel) }
            });

            BattleStateRenderer = new MoveActorRenderer(this, thinkState.BattleStateRenderer);
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            originalActorList = Battle.PlayerPartyLayout.PartyMembersList(Actor);
            originalActorListIndex = originalActorList.IndexOf(Actor);
        }

        public override void Finish()
        {
            base.Finish();
            PopState();
        }

        public override void Update(Delta delta)
        {
            inputButtonListener.Update(delta);
        }

        private void moveActorUp()
        {
            Battle.PlayerPartyLayout.MovePartyMemberUp(Actor, thinkState);
        }

        private void moveActorDown()
        {
            Battle.PlayerPartyLayout.MovePartyMemberDown(Actor, thinkState);
        }

        private void moveActorBack()
        {
            Battle.PlayerPartyLayout.MovePartyMemberBack(Actor, thinkState);
        }

        private void moveActorForward()
        {
            Battle.PlayerPartyLayout.MovePartyMemberForward(Actor, thinkState);
        }

        private void cancel()
        {
            Battle.PlayerPartyLayout.RemovePartyMember(Actor);
            originalActorList.Insert(originalActorListIndex, Actor);
            Finish();
        }
        #endregion
    }
}
