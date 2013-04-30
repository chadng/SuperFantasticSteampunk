using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.BattleStates
{
    class Think : BattleState
    {
        #region Instance Fields
        private InputButtonListener inputButtonListener;
        #endregion

        #region Constructors
        public Think(Battle battle)
            : base(battle)
        {
            inputButtonListener = new InputButtonListener(new Dictionary<InputButton, ButtonEventHandlers> {
                { InputButton.Up, new ButtonEventHandlers(press: buttonUpPressed) },
                { InputButton.Down, new ButtonEventHandlers(press: buttonDownPressed) }
            });
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            //TODO: Set up data structures for storing position data and move selections
        }

        public override void Finish()
        {
            ChangeState(new Act(battle));
        }

        public override void Update(GameTime gameTime)
        {
            //TODO: Allow player to select from a menu to reposition and choose moves for party
            inputButtonListener.Update(gameTime);
        }

        private void buttonUpPressed()
        {
            System.Console.WriteLine("Up");
        }

        private void buttonDownPressed()
        {
            System.Console.WriteLine("Down");
        }

        private void buttonAPressed()
        {
            System.Console.WriteLine("A");
        }

        private void buttonBPressed()
        {
            System.Console.WriteLine("B");
        }

        private void buttonXPressed()
        {
            System.Console.WriteLine("X");
        }

        private void buttonYPressed()
        {
            System.Console.WriteLine("Y");
        }
        #endregion
    }
}
