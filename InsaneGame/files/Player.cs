using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace InsaneGame.files
{
    public class Player : Entity
    {
        public Vector2 Velocity;
        public Rectangle PlayerFallRect;

        public float PlayerSpeed = 5f;
        public float Gravity = 3f;
        public bool IsFalling = true;

        public Animation[] PlayerAmination;
        public CurrentAnimation PlayerAnimationController;

        public Player(Texture2D idleSprite, Texture2D runSprite)
        {
            PlayerAmination = new Animation[2];

            Position = new Vector2();
            Velocity = new Vector2(); //new Vector2()

            PlayerAmination[0] = new Animation(idleSprite);
            PlayerAmination[1] = new Animation(runSprite);
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 32, 32);
        }

        public override void Update()
        {
            KeyboardState keyboard = Keyboard.GetState();

            PlayerAnimationController = CurrentAnimation.Idle;

            if (IsFalling)
                Velocity.Y += Gravity;

            if (keyboard.IsKeyDown(Keys.A))
            {
                Velocity.X -= PlayerSpeed;
                PlayerAnimationController = CurrentAnimation.Run;
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                Velocity.X += PlayerSpeed;
                PlayerAnimationController = CurrentAnimation.Run;
            }

            Position = Velocity;
            Hitbox.X = (int)Position.X;
            Hitbox.Y = (int)Position.Y;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            switch (PlayerAnimationController)
            {
                case CurrentAnimation.Idle:
                    PlayerAmination[0].Draw(spriteBatch, Position, gameTime, 500);
                    break;
                case CurrentAnimation.Run:
                    PlayerAmination[1].Draw(spriteBatch, Position, gameTime, 100);
                    break;
            }    

            //spriteBatch.Draw(spritesheet, position, Color.White);
        }
    }
}
