using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using TiledSharp;

namespace InsaneGame.files
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static float screenWidth;
        public static float screenHeight;

        #region Managers
        private GameManager _gameManager;
        private TilemapManager tilemapManager;
        #endregion

        #region Tilemap
        private TmxMap map;
        private Texture2D tileset;
        private List<Rectangle> collisionRects;
        private Rectangle startRect;
        private Rectangle endRect;
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

        public Game1()
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
            }
            #endregion

            _gameManager = new GameManager(endRect);

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

            #region Managers
            if (_gameManager.HasGameEnded(player.Hitbox))
            {
                Console.WriteLine("Game Ended");
            }
            if (player.Health <= 0)
            {
                Console.WriteLine("GameOVER");
            }
            Console.WriteLine($"Health = {player.Health}");
            #endregion

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
                        fireballs.Add(new FireBall(fireballTextureR, 7, temp_hitbox_R));
                    }
                    if (player.Effects == SpriteEffects.FlipHorizontally)
                    {
                        fireballs.Add(new FireBall(fireballTextureL, -7, temp_hitbox_L));
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
            //y axis 
            foreach (var rect in collisionRects)
            {
                if (rect.Intersects(player.PlayerFallRect))
                {
                    player.IsJumping = false;
                    player.Position.Y = initPos.Y;
                    player.Velocity.Y = initPos.Y;
                    //player.Gravity = 0;
                    player.CountOfJumps = 0;
                    break;
                }
            }

            //x axis
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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SkyBlue);

            _spriteBatch.Begin(transformMatrix: transformMatrix);

            tilemapManager.Draw(_spriteBatch);

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

            base.Draw(gameTime);
        }
    }
}