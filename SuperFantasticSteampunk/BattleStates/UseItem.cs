using System;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.BattleStates
{
    class UseItem : BattleState
    {
        #region Instance Fields
        private ThinkAction thinkAction;
        #endregion

        #region Constructors
        public UseItem(Battle battle, ThinkAction thinkAction)
            : base(battle)
        {
            if (thinkAction == null)
                throw new Exception("ThinkAction cannot be null");
            this.thinkAction = thinkAction;
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            //TODO: something
        }

        public override void Finish()
        {
            PopState();
        }

        public override void Update(GameTime gameTime)
        {
            Logger.Log(thinkAction.Actor.Data.Name + " used '" + thinkAction.OptionName + "' item");
            Logger.Log("TODO: use item");
            Finish();
        }
        #endregion
    }
}
