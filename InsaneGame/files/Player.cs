using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace InsaneGame.files
{
    public class Player : Entity
    {
        public Vector2 velocity;
        public float playerSpeed = 5f;
        public Animation[] playerAmination;
        public currentAnimation playerAnimationController;

        public Player(Texture2D idleSprite, Texture2D runSprite)
        {
            playerAmination = new Animation[2];
            velocity = Vector2.Zero; //new Vector2()
            playerAmination[0] = new Animation(idleSprite);
            playerAmination[1] = new Animation(runSprite);
        }

        public override void Update()
        {
            KeyboardState keyboard = Keyboard.GetState();

            playerAnimationController = currentAnimation.Idle;

            if (keyboard.IsKeyDown(Keys.A))
            {
                velocity.X -= playerSpeed;
                playerAnimationController = currentAnimation.Run;
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                velocity.X += playerSpeed;
                playerAnimationController = currentAnimation.Run;
            }

            position = velocity;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            switch (playerAnimationController)
            {
                case currentAnimation.Idle:
                    playerAmination[0].Draw(spriteBatch, position, gameTime, 500);
                    break;
                case currentAnimation.Run:
                    playerAmination[1].Draw(spriteBatch, position, gameTime, 100);
                    break;
            }    

            //spriteBatch.Draw(spritesheet, position, Color.White);
        }
    }
}
