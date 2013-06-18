using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;

namespace SuperFantasticSteampunk.BattleStates
{
    class ThinkRenderer : BattleStateRenderer
    {

        #region Instance Properties
        protected new Think battleState
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
        public override void Update(Delta delta)
        {
            // Update transitions and easing
        }

        public override void BeforeDraw(Renderer renderer)
        {
            // Draw elements under party members
        }

        public override void AfterDraw(Renderer renderer)
        {
            drawOuterMenu(renderer);
            if (battleState.CurrentThinkActionType != ThinkActionType.None)
                drawOptionNamesSubMenu(renderer);
            drawArrowOverCurrentPartyMember(renderer);
        }

        private void drawOuterMenu(Renderer renderer)
        {
            Vector2 position = new Vector2(100);
            foreach (string menuOption in Think.OuterMenuOptions)
            {
                renderer.DrawText(menuOption, position, battleState.CurrentOuterMenuOption == menuOption ? Color.Blue : Color.White, 0.0f, Vector2.Zero, Vector2.One);
                position.Y += 20.0f;
            }
        }

        private void drawOptionNamesSubMenu(Renderer renderer)
        {
            Vector2 position = new Vector2(200, 100);
            for (int i = 0; i < battleState.MenuOptions.Count; ++i)
            {
                ThinkMenuOption menuOption = battleState.MenuOptions[i];
                string text = menuOption.Name;
                if (i > 0)
                    text += " x " + (menuOption.Amount < 0 ? "*" : menuOption.Amount.ToString());
                Color color;
                if (menuOption.Disabled)
                    color = i == battleState.CurrentOptionNameIndex ? Color.DarkRed : Color.Gray;
                else
                    color = i == battleState.CurrentOptionNameIndex ? Color.Blue : Color.White;
                renderer.DrawText(text, position, color, 0.0f, Vector2.Zero, Vector2.One);
                position.Y += 20.0f;
            }
        }

        private void drawArrowOverCurrentPartyMember(Renderer renderer)
        {
            battleState.Battle.DrawArrowOverPartyMember(battleState.CurrentPartyMember, Color.White, renderer);
        }
        #endregion
    }
}
