using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InsaneGame.files
{
    public class Animation
    {
        Texture2D spritesheet;
        int frames;
        int rows = 0;
        int c = 0;
        float timeSinceLastFrame = 0;

        public Animation(Texture2D spritesheet, float width = 96, float height = 96) 
        { 
            this.spritesheet = spritesheet;
            frames = (int)(spritesheet.Width / width);
            Console.WriteLine(frames);
        }  

        public void Draw(SpriteBatch spriteBatch, Vector2 position, GameTime gameTime, float millisecondsPerFrames = 150, SpriteEffects effect = SpriteEffects.None, int a = 96)
        {
            if (c < frames)
            {
                var rect = new Rectangle(a * c, rows, a, a);
                spriteBatch.Draw(spritesheet, position, rect, Color.White, 0f, new Vector2(), 1f, effect, 1);
                timeSinceLastFrame += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (timeSinceLastFrame > millisecondsPerFrames)
                {
                    timeSinceLastFrame -= millisecondsPerFrames;
                    c++;
                    if (c == frames)
                    {
                        c = 0;
                    }
                }
            }
        }
    }
}
