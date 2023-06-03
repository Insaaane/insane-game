using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsaneGame.files
{
    public class FireBall
    {
        private Texture2D fireballTexture;
        private float speed;
        public Rectangle hitbox;

        public FireBall(Texture2D fireballTexture, float speed, Rectangle hitbox)
        {
            this.fireballTexture = fireballTexture;
            this.speed = speed;
            this.hitbox = hitbox;
        }

        public void Update()
        {
            hitbox.X += (int)speed;
        }

        public bool HasHit(Rectangle rect)
        {
            return hitbox.Intersects(rect);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(fireballTexture, hitbox, Color.White);
        }
    }
}
