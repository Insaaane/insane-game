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
        private float speed = 3f;
        private bool isFacingRight = true;

        public Enemy(Texture2D enemySpriteSheet, Rectangle pathway, float speed = 3f)
        {
            enemyAnim = new Animation(enemySpriteSheet);
            this.pathway = pathway;
            Position = new Vector2(pathway.X, pathway.Y);
            Hitbox = new Rectangle(pathway.X, pathway.Y, 16, 16);
            this.speed = speed; 
        }

        public override void Update()
        {
            if (!pathway.Contains(Hitbox))
            {
                speed = -speed;
                isFacingRight = !isFacingRight;
            }
            Position.X += speed;

            Hitbox.X = (int)Position.X;
            Hitbox.Y = (int)Position.Y; 
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (isFacingRight)
                enemyAnim.Draw(spriteBatch, Position, gameTime, 80, SpriteEffects.FlipHorizontally);
            else
                enemyAnim.Draw(spriteBatch, Position, gameTime, 80);
        }
    }
}
