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
        public float Gravity = 5f;
        public float JumpSpeed = -14f;
        public float startY;

        public bool IsFalling = true;
        public bool IsJumping;

        public Animation[] PlayerAmination;
        public CurrentAnimation PlayerAnimationController;

        public Player(Texture2D idleSprite, Texture2D runSprite)
        {
            PlayerAmination = new Animation[2];

            Position = new Vector2();
            Velocity = new Vector2();

            PlayerAmination[0] = new Animation(idleSprite);
            PlayerAmination[1] = new Animation(runSprite);
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 25, 25);
            PlayerFallRect = new Rectangle((int)Position.X, (int)Position.Y, 25, 25);
        }

        public override void Update()
        {
            KeyboardState keyboard = Keyboard.GetState();

            PlayerAnimationController = CurrentAnimation.Idle;

            Position = Velocity;
            
            Move(keyboard);
            //if (IsFalling)
            Velocity.Y += Gravity;

            startY = Position.Y;
            Jump(keyboard);

            Hitbox.X = (int)Position.X;
            Hitbox.Y = (int)Position.Y;
            PlayerFallRect.X = (int)Position.X;
            PlayerFallRect.Y = (int)Velocity.Y;
        }


        private void Move(KeyboardState keyboard)
        {
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
        }

        private void Jump(KeyboardState keyboard)
        {
            if (IsJumping)
            {
                Position.Y += JumpSpeed;
                JumpSpeed += 1;
                if (Position.Y >= startY) 
                {
                    Position.Y = startY;
                    IsJumping = false;
                }
                else
                {
                    if (keyboard.IsKeyDown(Keys.Space) || keyboard.IsKeyDown(Keys.W))
                    {
                        IsJumping = true;
                        JumpSpeed = -14f;
                        Position.Y += JumpSpeed;
                    }
                }
            }
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
        }
    }
}
