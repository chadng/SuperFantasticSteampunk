using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Spine;

namespace SuperFantasticSteampunk
{
    class Battle : Scene
    {
        #region Instance Fields
        private Stack<BattleState> states;
        private bool stateChanged;
        private TextureData whitePixelTextureData;
        #endregion

        #region Instance Properties
        public BattleState CurrentBattleState
        {
            get { return states.Peek(); }
        }

        public Party PlayerParty { get; private set; }
        public Party EnemyParty { get; private set; }

        public PartyBattleLayout PlayerPartyLayout { get; private set; }
        public PartyBattleLayout EnemyPartyLayout { get; private set; }
        #endregion

        #region Constructors
        public Battle(Party playerParty, Party enemyParty)
        {
            if (playerParty == null)
                throw new Exception("Party playerParty cannot be null");
            if (enemyParty == null)
                throw new Exception("Party enemyParty cannot be null");

            PlayerParty = playerParty.Tap(party => party.StartBattle(this));
            EnemyParty = enemyParty.Tap(party => party.StartBattle(this));

            PlayerPartyLayout = new PartyBattleLayout(PlayerParty);
            EnemyPartyLayout = new PartyBattleLayout(EnemyParty);

            states = new Stack<BattleState>();
            states.Push(new BattleStates.Intro(this));
            stateChanged = true;
        }
        #endregion

        #region Instance Methods
        public void ChangeState(BattleState battleState)
        {
            states.Pop();
            states.Push(battleState);
            stateChanged = true;
        }

        public void PushState(BattleState battleState)
        {
            CurrentBattleState.Pause();
            Logger.Log(CurrentBattleState.GetType().Name + " battle state paused");
            states.Push(battleState);
            stateChanged = true;
        }

        public void PopState()
        {
            BattleState previousBattleState = states.Pop();
            CurrentBattleState.Resume(previousBattleState);
            Logger.Log(CurrentBattleState.GetType().Name + " battle state resumed");
        }

        public void AddBattleEntity(Entity entity)
        {
            addEntity(entity);
        }

        public void RemoveBattleEntity(Entity entity)
        {
            removeEntity(entity);
        }

        public Party GetPartyForPartyMember(PartyMember partyMember)
        {
            if (PlayerParty.Contains(partyMember))
                return PlayerParty;
            if (EnemyParty.Contains(partyMember))
                return EnemyParty;
            return null;
        }

        public PartyBattleLayout GetPartyBattleLayoutForPartyMember(PartyMember partyMember)
        {
            if (PlayerParty.Contains(partyMember))
                return PlayerPartyLayout;
            if (EnemyParty.Contains(partyMember))
                return EnemyPartyLayout;
            return null;
        }

        protected override void update(GameTime gameTime)
        {
            if (stateChanged)
            {
                stateChanged = false;
                CurrentBattleState.Start();
                Logger.Log(CurrentBattleState.GetType().Name + " battle state started");
            }

            CurrentBattleState.Update(gameTime);
            base.update(gameTime);

            if (CurrentBattleState is BattleStates.Think)
                repositionPartyMembers();

            if (CurrentBattleState.BattleStateRenderer != null)
                CurrentBattleState.BattleStateRenderer.Update(gameTime);
        }

        protected override void draw(Renderer renderer)
        {
            if (CurrentBattleState.BattleStateRenderer != null)
                CurrentBattleState.BattleStateRenderer.BeforeDraw(renderer);
            drawPersistentGui(renderer);
            base.draw(renderer);
            if (CurrentBattleState.BattleStateRenderer != null)
                CurrentBattleState.BattleStateRenderer.AfterDraw(renderer);
        }

        protected override void finishCleanup()
        {
            Logger.Log("Finishing battle");
            PlayerParty.FinishBattle(this);
            EnemyParty.FinishBattle(this);
            base.finishCleanup();
        }

        private void repositionPartyMembers()
        {
            Vector2 position = new Vector2(400.0f);
            PlayerPartyLayout.ForEachList(list =>
            {
                position.X = 400.0f;
                for (int i = list.Count - 1; i >= 0; --i)
                {
                    list[i].BattleEntity.Position = position;
                    position.X += 150.0f;
                }
                position.Y += 150.0f;
            });

            position = new Vector2(1000.0f, 400.0f);
            EnemyPartyLayout.ForEachList(list =>
            {
                position.X = 1000.0f;
                for (int i = 0; i < list.Count; ++i)
                {
                    list[i].BattleEntity.Position = position;
                    position.X -= 150.0f;
                }
                position.Y += 150.0f;
            });
        }

        private void drawPersistentGui(Renderer renderer)
        {
            foreach (PartyMember partyMember in PlayerParty)
                drawHealthBar(partyMember, renderer);
            foreach (PartyMember partyMember in EnemyParty)
                drawHealthBar(partyMember, renderer);
        }

        private void drawHealthBar(PartyMember partyMember, Renderer renderer)
        {
            if (whitePixelTextureData == null)
                whitePixelTextureData = ResourceManager.GetTextureData("white_pixel");
            if (whitePixelTextureData == null)
                return;

            float percentageHealth = partyMember.Health / (float)partyMember.MaxHealth;
            Color color = Color.Lerp(Color.Red, Color.Green, percentageHealth);
            float width = 200.0f * percentageHealth;
            float height = 20.0f;

            renderer.Draw(whitePixelTextureData, partyMember.BattleEntity.Position + new Vector2(20.0f), color, 0.0f, new Vector2(width, height));
        }
        #endregion
    }
}
