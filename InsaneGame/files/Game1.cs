using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using TiledSharp;

namespace InsaneGame.files
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

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

            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            #region Tilemap
            map = new TmxMap("Content\\Level1.tmx");
            tileset = Content.Load<Texture2D>("Cave Tileset\\" + map.Tilesets[0].Name.ToString());
            int tileWidth = map.Tilesets[0].TileWidth;
            int tileHeight = map.Tilesets[0].TileHeight;
            int tilesetTileWidth = tileset.Width / tileWidth;

            tilemapManager = new TilemapManager(map, tileset, tilesetTileWidth, tileWidth, tileHeight);
            #endregion

            collisionRects = new List<Rectangle>();

            foreach (var obj in map.ObjectGroups["Collisions"].Objects)
            {
                if (obj.Name == "")
                {
                    collisionRects.Add(new Rectangle((int)obj.X, (int)obj.Y, (int)obj.Width, (int)obj.Height));
                }
            }

            #region Player
            player = new Player(
                Content.Load<Texture2D>("Sprite Pack 4\\1 - Agent_Mike_Idle (32 x 32)"),
                Content.Load<Texture2D>("Sprite Pack 4\\1 - Agent_Mike_Running (32 x 32)"));
            #endregion
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            var initPos = player.Position;
            player.Update();
            #region Player Collisions
            //y axis 
            foreach (var rect in collisionRects)
            {
                //player.IsFalling = true;
                if (rect.Intersects(player.PlayerFallRect))
                {
                    //player.IsFalling = false;
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
                    player.Position.X = initPos.X;
                    player.Velocity.X = initPos.X;
                    break;
                }
            }  
            #endregion

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            tilemapManager.Draw(_spriteBatch);
            player.Draw(_spriteBatch, gameTime);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}