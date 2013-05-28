using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Spine;
using SuperFantasticSteampunk.OverworldStates;

namespace SuperFantasticSteampunk
{
    class Battle : Scene
    {
        #region Instance Fields
        private Stack<BattleState> states;
        private bool stateChanged;
        private TextureData whitePixelTextureData;
        private Camera camera;
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

        public Encounter OverworldEncounter { get; private set; }
        #endregion

        #region Constructors
        public Battle(Party playerParty, Party enemyParty, Encounter overworldEncounter)
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

            OverworldEncounter = overworldEncounter;

            camera = new Camera(Game1.ScreenSize);
            camera.Position = camera.Size / 2.0f;

            repositionPartyMembers();
            updateCamera();
            camera.Scale = camera.TargetScale;
            updateCamera();
            camera.Position = camera.TargetPosition;
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

            if (CurrentBattleState.KeepPartyMembersStatic)
                repositionPartyMembers();

            base.update(gameTime);

            if (CurrentBattleState.BattleStateRenderer != null)
                CurrentBattleState.BattleStateRenderer.Update(gameTime);

            updateCamera(() => camera.Update(gameTime));
        }

        protected override void draw(Renderer renderer)
        {
            Game1.BackgroundColor = Game1.GrassColor;
            renderer.Camera = camera;
            renderer.Tint = Clock.GetCurrentColor();

            renderer.Draw(ResourceManager.GetTextureData("battle_grass_floor"), Vector2.Zero, Color.White);
            if (CurrentBattleState.BattleStateRenderer != null)
                CurrentBattleState.BattleStateRenderer.BeforeDraw(renderer);

            base.draw(renderer);

            renderer.ResetTint();

            drawPersistentGui(renderer);

            if (CurrentBattleState.BattleStateRenderer != null)
                CurrentBattleState.BattleStateRenderer.AfterDraw(renderer);

            renderer.Camera = null;
        }

        protected override void finishCleanup()
        {
            Logger.Log("Finishing battle");
            PlayerParty.FinishBattle(this);
            EnemyParty.FinishBattle(this);
            base.finishCleanup();
        }

        private void updateCamera(Action midUpdateAction = null)
        {
            Vector2 firstPosition = (PlayerParty.Count > 0 ? PlayerParty : EnemyParty)[0].BattleEntity.Position;
            float lowestX = firstPosition.X - 200.0f, lowestY = firstPosition.Y - 400.0f;
            float highestX = firstPosition.X + 200.0f, highestY = firstPosition.Y + 200.0f;

            getLowestAndHighestPositionalValuesForParty(PlayerParty, ref lowestX, ref lowestY, ref highestX, ref highestY);
            getLowestAndHighestPositionalValuesForParty(EnemyParty, ref lowestX, ref lowestY, ref highestX, ref highestY);

            float scaleX = Game1.ScreenSize.X / (highestX - lowestX);
            float scaleY = Game1.ScreenSize.Y / (highestY - lowestY);

            if (scaleX < scaleY)
                camera.TargetScale = new Vector2(scaleX);
            else
                camera.TargetScale = new Vector2(scaleY);

            if (midUpdateAction != null)
                midUpdateAction();

            float scale = camera.Scale.X;
            float averageX = (lowestX + highestX) / 2.0f;
            float averageY = (lowestY + highestY) / 2.0f;
            camera.TargetPosition = new Vector2(averageX, averageY) * scale;
        }

        private void getLowestAndHighestPositionalValuesForParty(Party party, ref float lowestX, ref float lowestY, ref float highestX, ref float highestY)
        {
            foreach (PartyMember partyMember in party)
            {
                Vector2 position = partyMember.BattleEntity.Position;

                if (position.X - 200.0f < lowestX)
                    lowestX = position.X - 200.0f;
                else if (position.X + 200.0f > highestX)
                    highestX = position.X + 200.0f;

                if (position.Y - 400.0f < lowestY)
                    lowestY = position.Y - 400.0f;
                else if (position.Y + 200.0f > highestY)
                    highestY = position.Y + 200.0f;
            }
        }

        private void repositionPartyMembers()
        {
            Vector2 position = new Vector2(600.0f, 400.0f);
            PlayerPartyLayout.ForEachList(list =>
            {
                for (int i = 0; i < list.Count; ++i)
                    list[i].BattleEntity.Position = new Vector2(position.X - (200.0f * i), position.Y);
                position.X -= 150.0f;
                position.Y += 150.0f;
            });

            position = new Vector2(1200.0f, 400.0f);
            EnemyPartyLayout.ForEachList(list =>
            {
                for (int i = 0; i < list.Count; ++i)
                    list[i].BattleEntity.Position = new Vector2(position.X + (200.0f * i), position.Y);
                position.X += 150.0f;
                position.Y += 150.0f;
            });
        }

        private void drawPersistentGui(Renderer renderer)
        {
            foreach (PartyMember partyMember in PlayerParty)
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
