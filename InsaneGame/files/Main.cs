using Apos.Gui;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using TiledSharp;

namespace InsaneGame.files
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static float screenWidth;
        public static float screenHeight;

        private bool exitRequested; 
        private float exitDelayTimer; 
        private const float exitDelayDuration = 0.8f;

        #region UI
        IMGUI _ui;
        #endregion

        #region Managers
        private GameManager endGameManager;
        private GameManager deathGameManager;
        private TilemapManager tilemapManager;
        #endregion

        #region Tilemap
        private TmxMap map;
        private Texture2D tileset;
        private List<Rectangle> collisionRects;
        private Rectangle startRect;
        private Rectangle endRect;
        private Rectangle deathRect;
        #endregion

        #region Player
        private Player player;
        private List<FireBall> fireballs;
        private Texture2D fireballTextureR;
        private Texture2D fireballTextureL;
        private int time_between_fieballs;
        private int time_between_hurt = 20;
        private int points;
        #endregion

        #region Emeny
        private Enemy mainEnemy;
        private List<Enemy> enemyList;
        private List<Rectangle> enemyPathway;
        #endregion

        #region Camera
        private Camera camera;
        private Matrix transformMatrix;
        #endregion

        #region Coins
        private List<Coin> coins;
        #endregion

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            _graphics.IsFullScreen = true;
            screenHeight = _graphics.PreferredBackBufferHeight;
            screenWidth = _graphics.PreferredBackBufferWidth;
            _graphics.SynchronizeWithVerticalRetrace = true;
            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            #region UI
            FontSystem fontSystem = FontSystemFactory.Create(GraphicsDevice, 2048, 2048);
            fontSystem.AddFont(TitleContainer.OpenStream($"{Content.RootDirectory}/PixelFont.ttf"));
            GuiHelper.Setup(this, fontSystem);
            _ui = new IMGUI();
            #endregion

            #region Tilemap
            map = new TmxMap("Content\\Level.tmx");
            tileset = Content.Load<Texture2D>("Texture\\" + map.Tilesets[0].Name.ToString());
            var tileWidth = map.Tilesets[0].TileWidth;
            var tileHeight = map.Tilesets[0].TileHeight;
            var tilesetTileWidth = tileset.Width / tileWidth;
            tilemapManager = new TilemapManager(map, tileset, tilesetTileWidth, tileWidth, tileHeight);
            #endregion

            #region Collision
            collisionRects = new List<Rectangle>();
            foreach (var obj in map.ObjectGroups["Collisions"].Objects)
            {
                if (obj.Name == "")
                {
                    collisionRects.Add(new Rectangle((int)obj.X, (int)obj.Y, (int)obj.Width, (int)obj.Height));
                }
                else if (obj.Name == "start")
                {
                    startRect = new Rectangle((int)obj.X, (int)obj.Y, (int)obj.Width, (int)obj.Height);
                }
                else if (obj.Name == "end")
                {
                    endRect = new Rectangle((int)obj.X, (int)obj.Y, (int)obj.Width, (int)obj.Height);
                }
                else if (obj.Name == "death")
                {
                    deathRect = new Rectangle((int)obj.X, (int)obj.Y, (int)obj.Width, (int)obj.Height);
                }
            }
            #endregion

            endGameManager = new GameManager(endRect);
            deathGameManager = new GameManager(endRect);

            #region Player
            player = new Player(
                new Vector2(startRect.X, startRect.Y),
                Content.Load<Texture2D>("PlayerSprites\\idle"),
                Content.Load<Texture2D>("PlayerSprites\\run"),
                Content.Load<Texture2D>("PlayerSprites\\jump"));
            #endregion

            #region Fireball
            fireballs = new List<FireBall>();
            fireballTextureR = Content.Load<Texture2D>("FireBall\\FB001");
            fireballTextureL = Content.Load<Texture2D>("FireBall\\FB002");
            #endregion

            #region Camera 
            camera = new Camera();
            #endregion

            #region Enemy
            enemyPathway = new List<Rectangle>();
            foreach (var obj in map.ObjectGroups["EnemyPathways"].Objects)
            {
                enemyPathway.Add(new Rectangle((int)obj.X, (int)obj.Y, (int)obj.Width, (int)obj.Height));
            }
            enemyList = new List<Enemy>();
            for (var i = 0; i < enemyPathway.Count; i++)
            {
                mainEnemy = new Enemy(
                Content.Load<Texture2D>("EnemySprites\\enemy_run"),
                enemyPathway[i]);
                enemyList.Add(mainEnemy);
            }
            #endregion

            #region Coin
            coins = new List<Coin>();
            var coinTexture = Content.Load<Texture2D>("Coins\\Coin");
            foreach (var o in map.ObjectGroups["Collectibles"].Objects)
            {
                var coin = new Coin(coinTexture, new Vector2((float)o.X, (float)o.Y));
                coins.Add(coin);
            }
            #endregion
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            #region Enemies
            foreach (var enemy in enemyList)
            {
                enemy.Update();
                if (enemy.HasHit(player.Hitbox))
                {
                    player.HitCounter++;
                    if (player.HitCounter > time_between_hurt)
                    {
                        player.Health--;
                        player.HitCounter = 0;
                    }
                }
            }
            #endregion

            #region Camera update
            transformMatrix = camera.Follow(player.Hitbox);
            #endregion

            if (deathRect.Intersects(player.Hitbox))
            {
                player.Health = 0;
            }
            #region Player

            #region Fireball
            if (player.IsShooting)
            {
                if (time_between_fieballs > 5 && fireballs.ToArray().Length < 20)
                {
                    var temp_hitbox_R = new Rectangle((int)player.Position.X + 40, 
                        (int)player.Position.Y + 30, fireballTextureR.Width, fireballTextureR.Height);
                    var temp_hitbox_L = new Rectangle((int)player.Position.X, 
                        (int)player.Position.Y + 30, fireballTextureL.Width, fireballTextureL.Height);

                    if (player.Effects == SpriteEffects.None)
                    {
                        fireballs.Add(new FireBall(fireballTextureR, 8, temp_hitbox_R));
                    }
                    if (player.Effects == SpriteEffects.FlipHorizontally)
                    {
                        fireballs.Add(new FireBall(fireballTextureL, -8, temp_hitbox_L));
                    }
                    time_between_fieballs = 0;
                }
                else 
                {
                    time_between_fieballs++;
                }
            }
            foreach (var fireball in fireballs.ToArray())
            {
                fireball.Update();

                foreach (var rect in collisionRects)
                {
                    if (rect.Intersects(fireball.hitbox))
                    {
                        fireballs.Remove(fireball);
                        break; 
                    }
                }
                foreach (var enemy in enemyList.ToArray())
                {
                    if (fireball.hitbox.Intersects(enemy.Hitbox))
                    {
                        fireballs.Remove(fireball);
                        enemyList.Remove(enemy);
                        points++;
                        break;
                    }
                }
            }
            #endregion

            #region Player Collisions
            var initPos = player.Position;
            player.Update();
            foreach (var rect in collisionRects)
            {
                if (rect.Intersects(player.PlayerFallRect))
                {
                    player.IsJumping = false;
                    player.Position.Y = initPos.Y;
                    player.Velocity.Y = initPos.Y;
                    player.CountOfJumps = 0;
                    break;
                }
            }
            foreach (var rect in collisionRects)
            {
                if (rect.Intersects(player.Hitbox))
                {
                    player.Position = initPos;
                    player.Velocity = initPos;
                    break;
                }
            }
            #endregion

            #endregion

            #region Coin
            foreach (var coin in coins.ToArray())
            {
                if (coin.rect.Intersects(player.Hitbox))
                {
                    points++;
                    coins.Remove(coin);
                    break;
                }
            }
            #endregion

            #region UI
            GuiHelper.UpdateSetup(gameTime);
            _ui.UpdateAll(gameTime);

            Panel.Push().XY = new Vector2(1560, 10);
            Label.Put($"Points: {points} / 10", 50, Color.White, 0, false);
            Panel.Pop();

            Panel.Push().XY = new Vector2(10, 10);
            Label.Put($"Health: {player.Health}", 50, Color.Red, 0, false);
            Panel.Pop();

            Panel.Push().XY = new Vector2(850, 10);
            if (points >= 10)
            {
                Label.Put("Найдите выход", 40, Color.White, 0, false);
            }
            Panel.Pop();

            Panel.Push().XY = new Vector2(740, 10);
            if (endGameManager.HasGameEnded(player.Hitbox) && points <= 10)
            {
                Label.Put("Наберите больше очков", 40, Color.White, 0, false);
            }
            Panel.Pop();


            Panel.Push().XY = new Vector2(750, 430);

            if (endGameManager.HasGameEnded(player.Hitbox) && points >= 10)
            {
                Label.Put("You Won!", 90, Color.Green, 0, false);
                exitRequested = true;
            }
            if (player.Health <= 0)
            {
                exitRequested = true;
                Label.Put("Game Over", 90, Color.Red, 0, false);
            }
            Panel.Pop();

            GuiHelper.UpdateCleanup();
            #endregion

            if (exitRequested)
            {
                exitDelayTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (exitDelayTimer >= exitDelayDuration)
                {
                    Exit();
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SkyBlue);
            _spriteBatch.Begin(transformMatrix: transformMatrix);
            tilemapManager.Draw(_spriteBatch);

            #region Coins
            foreach (var coin in coins)
            {
                coin.Draw(_spriteBatch, gameTime);
            }
            #endregion

            #region Enemies
            foreach (var enemy in enemyList)
            {
                enemy.Draw(_spriteBatch, gameTime);
            }
            #endregion

            #region FireBall
            foreach (var fireball in fireballs.ToArray())
            {
                fireball.Draw(_spriteBatch);
            }
            #endregion

            #region Player
            player.Draw(_spriteBatch, gameTime);
            #endregion

            _spriteBatch.End();
            _ui.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}