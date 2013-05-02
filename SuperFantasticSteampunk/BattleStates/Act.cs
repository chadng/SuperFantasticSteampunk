using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.BattleStates
{
    class Act : BattleState
    {
        #region Instance Fields
        private List<ThinkAction> thinkActions;
        #endregion

        #region Constructors
        public Act(Battle battle, List<ThinkAction> thinkActions)
            : base(battle)
        {
            if (thinkActions == null)
                throw new Exception("List<ThinkAction> cannot be null");
            this.thinkActions = thinkActions;
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            //TODO: Order entities by speed, setup for moving
        }

        public override void Finish()
        {
            ChangeState(new EndTurn(Battle));
        }

        public override void Update(GameTime gameTime)
        {
            //TODO: Perform move for each entity. Defence stances happen first
            Finish();
        }
        #endregion
    }
}
