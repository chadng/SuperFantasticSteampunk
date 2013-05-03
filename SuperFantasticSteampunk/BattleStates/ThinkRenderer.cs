using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;

namespace SuperFantasticSteampunk.BattleStates
{
    class ThinkRenderer : BattleStateRenderer
    {
        #region Instance Fields
        Texture2D arrowTexture;
        #endregion

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
            arrowTexture = ResourceManager.GetTexture("arrow_down");
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
            drawThinkActionTypeMenu(renderer);
            if (battleState.CurrentThinkActionType != ThinkActionType.None)
                drawOptionNamesSubMenu(renderer);
            drawArrowOverCurrentPartyMember(renderer);
        }

        private void drawThinkActionTypeMenu(Renderer renderer)
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
                renderer.DrawText(text, position, color);
                position.Y += 20.0f;
            }
        }

        private void drawArrowOverCurrentPartyMember(Renderer renderer)
        {
            Vector2 position = battleState.CurrentPartyMember.BattleEntity.Position;
            position.Y -= 400.0f;
            renderer.Draw(arrowTexture, position, Color.White);
        }
        #endregion
    }
}
