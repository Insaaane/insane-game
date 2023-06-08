using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsaneGame.files
{
    public abstract class Entity
    {
        public Texture2D Spritesheet;
        public Vector2 Position;
        public Rectangle Hitbox;

        public enum CurrentAnimation
        {
            Idle,
            Run,
            Jump
        }

        public abstract void Update();

        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
