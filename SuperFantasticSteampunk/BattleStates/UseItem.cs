using System;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.BattleStates
{
    class UseItem : BattleState
    {
        #region Instance Fields
        private ThinkAction thinkAction;
        private ScriptRunner scriptRunner;
        private Item item;
        #endregion

        #region Constructors
        public UseItem(Battle battle, ThinkAction thinkAction)
            : base(battle)
        {
            if (thinkAction == null)
                throw new Exception("ThinkAction cannot be null");
            this.thinkAction = thinkAction;
            scriptRunner = null;
            item = null;
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            item = ResourceManager.GetNewItem(thinkAction.OptionName);
            if (item != null)
                scriptRunner = new ScriptRunner(item.Data.Script, Battle, thinkAction.Actor, thinkAction.Target);
        }

        public override void Finish()
        {
            PopState();
        }

        public override void Update(GameTime gameTime)
        {
            if (scriptRunner == null)
                Finish();
            else
            {
                scriptRunner.Update(gameTime);
                if (scriptRunner.IsFinished())
                    Finish();
            }
        }
        #endregion
    }
}
