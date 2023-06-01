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
        public Texture2D spritesheet;

        public enum currentAnimation
        {
            Idle,
            Run
        }

        public Vector2 position;

        public abstract void Update();

        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
