using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace InsaneGame.files
{
    public class Enemy : Entity
    {
        private Animation enemyAnim;
        private Rectangle pathway;
        private float speed = 1.5f;
        private bool isFacingRight = true;

        public Enemy(Texture2D enemySpriteSheet, Rectangle pathway, float speed = 1.5f)
        {
            enemyAnim = new Animation(enemySpriteSheet);
            this.pathway = pathway;
            Position = new Vector2(pathway.X, pathway.Y);
            Hitbox = new Rectangle(pathway.X, pathway.Y, 20, 90);
            this.speed = speed; 
        }

        public override void Update()
        {
            if (!pathway.Intersects(Hitbox))
            {
                speed = -speed;
                isFacingRight = !isFacingRight;
            }
            Position.X += speed;

            Hitbox.X = (int)Position.X + 40;
            Hitbox.Y = (int)Position.Y; 
        }

        public bool HasHit(Rectangle playerRect)
        {
            return Hitbox.Intersects(playerRect);
        }


        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (isFacingRight)
                enemyAnim.Draw(spriteBatch, Position, gameTime, 80, SpriteEffects.FlipHorizontally);
            else
                enemyAnim.Draw(spriteBatch, Position, gameTime);
        }
    }
}
