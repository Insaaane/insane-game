using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace InsaneGame.files
{
    public class Player : Entity
    {
        public Vector2 Velocity;
        public Rectangle PlayerFallRect;
        public SpriteEffects Effects;

        public float PlayerSpeed = 4f;
        public float Gravity = 7f;
        public float JumpSpeed = -23f;
        public float StartY;

        public int Health = 10;
        public int HitCounter = 0;


        public int CountOfJumps = 0;

        public bool IsJumping;
        public bool IsShooting;

        public Animation[] PlayerAmination;
        public CurrentAnimation PlayerAnimationController;

        public Player(Vector2 position, Texture2D idleSprite, Texture2D runSprite, Texture2D jumpSprite)
        {
            PlayerAmination = new Animation[3];

            Position = position;
            Velocity = new Vector2();
            Effects = SpriteEffects.None;

            PlayerAmination[0] = new Animation(idleSprite);
            PlayerAmination[1] = new Animation(runSprite);
            PlayerAmination[2] = new Animation(jumpSprite);

            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 96, 87);
            PlayerFallRect = new Rectangle((int)Position.X, (int)Position.Y, 96, 87);
        }

        public override void Update()
        {
            var keyboard = Keyboard.GetState();
            var mouse = Mouse.GetState();

            PlayerAnimationController = CurrentAnimation.Idle;

            Position = Velocity;

            IsShooting = Equals(mouse.LeftButton, ButtonState.Pressed);

            StartY = Position.Y;

            Move(keyboard);
            Jump(keyboard);

            Velocity.Y += Gravity;

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
                Effects = SpriteEffects.FlipHorizontally;
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                Velocity.X += PlayerSpeed;
                PlayerAnimationController = CurrentAnimation.Run;
                Effects = SpriteEffects.None;
            }
            if (keyboard.IsKeyDown(Keys.S))
            {
                Gravity = 7f;
            }
        }

        private void Jump(KeyboardState keyboard)
        {
            if (IsJumping)
            {
                Velocity.Y += JumpSpeed;
                JumpSpeed += 0.8f;
                //Move(keyboard);
                PlayerAnimationController = CurrentAnimation.Jump;

                if (Velocity.Y >= StartY)
                {
                    Velocity.Y = StartY;
                    IsJumping = false;
                }
            }
            else
            {
                if ((keyboard.IsKeyDown(Keys.Space) || keyboard.IsKeyDown(Keys.W)) && CountOfJumps < 2)
                {
                    IsJumping = true;
                    CountOfJumps += 1;
                    JumpSpeed = -23f;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            switch (PlayerAnimationController)
            {
                case CurrentAnimation.Idle:
                    PlayerAmination[0].Draw(spriteBatch, Position, gameTime, 150, Effects);
                    break;
                case CurrentAnimation.Run:
                    PlayerAmination[1].Draw(spriteBatch, Position, gameTime, 80, Effects);
                    break;
                case CurrentAnimation.Jump:
                    PlayerAmination[2].Draw(spriteBatch, Position, gameTime, 100, Effects);
                    break;
            }
        }
    }
}