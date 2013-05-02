using System;
using Microsoft.Xna.Framework;
using Spine;

namespace SuperFantasticSteampunk.BattleStates
{
    class ThinkRenderer : BattleStateRenderer
    {
        #region Instance Properties
        protected Think battleState
        {
            get { return base.battleState as Think; }
        }
        #endregion

        #region Constructors
        public ThinkRenderer(BattleState battleState)
            : base(battleState)
        {
        }
        #endregion

        #region Instance Methods
        public override void Update(GameTime gameTime)
        {
            // Update transitions and easing
        }

        public override void BeforeDraw(Renderer renderer)
        {
            // Draw elements under party members
        }

        public override void AfterDraw(Renderer renderer)
        {
            DrawThinkActionTypeMenu(renderer);
            if (battleState.CurrentThinkActionType != ThinkActionType.None)
                DrawOptionNamesSubMenu(renderer);
        }

        private void DrawThinkActionTypeMenu(Renderer renderer)
        {
            Vector2 position = new Vector2(100);
            foreach (ThinkActionType thinkActionType in Enum.GetValues(typeof(ThinkActionType)))
            {
                if (thinkActionType == ThinkActionType.None)
                    continue;
                renderer.DrawText(thinkActionType.ToString(), position, battleState.CurrentThinkActionType == thinkActionType ? Color.Blue : Color.White);
                position.Y += 20.0f;
            }
        }

        private void DrawOptionNamesSubMenu(Renderer renderer)
        {
            Vector2 position = new Vector2(200, 100);
            for (int i = 0; i < battleState.OptionNames.Count; ++i)
            {
                renderer.DrawText(battleState.OptionNames[i], position, i == battleState.CurrentOptionNameIndex ? Color.Blue : Color.White);
                position.Y += 20.0f;
            }
        }
        #endregion
    }
}
