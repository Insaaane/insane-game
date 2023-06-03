using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.MediaFoundation;
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

        #region Tilemap
        private TmxMap map;
        private TilemapManager tilemapManager;
        private Texture2D tileset;
        private List<Rectangle> collisionRects;
        private Rectangle startRect;
        private Rectangle endRect;
        #endregion

        #region Player
        private Player player;
        private List<FireBall> fireballs;
        private Texture2D fireballTexture;
        #endregion

        #region Emeny
        private Enemy MainEnemy;
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

            #region Player
            player = new Player(
                new Vector2(startRect.X, startRect.Y),
                Content.Load<Texture2D>("PlayerSprites\\idle"),
                Content.Load<Texture2D>("PlayerSprites\\run"),
                Content.Load<Texture2D>("PlayerSprites\\jump"));
            #endregion

            #region Fireball
            fireballs = new List<FireBall>();
            fireballTexture = Content.Load<Texture2D>("FireBall\\fireBall");
            #endregion

            #region Enemy

            enemyPathway = new List<Rectangle>();
            foreach (var obj in map.ObjectGroups["EnemyPathways"].Objects)
            {
                enemyPathway.Add(new Rectangle((int)obj.X, (int)obj.Y, (int)obj.Width, (int)obj.Height));
            }
            enemyList = new List<Enemy>();


            for (var i  = 0; i < enemyPathway.Count; i++)
            {
                MainEnemy = new Enemy(
                Content.Load<Texture2D>("EnemySprites\\enemy_run"),
                enemyPathway[i]);
                enemyList.Add(MainEnemy);
            }
            #endregion

            #region Camera 
            camera = new Camera();
            #endregion
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            #region Enemies
            foreach(var enemy in enemyList)
            {
                enemy.Update();
            }
            #endregion

            #region Camera update
            transformMatrix = camera.Follow(player.Hitbox);
            #endregion

            #region Player

            #region FireballTexture
            
            if (player.IsShoting)
            {
                if (player.Effects == SpriteEffects.None)
                {
                    var temp_hitbox = new Rectangle((int)player.Position.X, (int)player.Position.Y, fireballTexture.Width, fireballTexture.Height);
                    fireballs.Add(new FireBall(fireballTexture, 4, temp_hitbox));
                }
            }

            foreach (var fireball in fireballs.ToArray())
            {

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
            GraphicsDevice.Clear(Color.LightBlue);

            //_spriteBatch.Begin(transformMatrix: transformMatrix);
            _spriteBatch.Begin();

            tilemapManager.Draw(_spriteBatch);
            #region Enemies
            foreach (var enemy in enemyList)
            {
                enemy.Draw(_spriteBatch, gameTime);
            }
            #endregion

            #region Player
            player.Draw(_spriteBatch, gameTime);

            #region Bullet

            foreach (var fireball in fireballs.ToArray())
            {
                fireball.Draw(_spriteBatch);
            }

            #endregion

            #endregion
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}