using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace InsaneGame.files
{
    public class Coin
    {
        public Rectangle rect;
        private Animation coinAnim;

        public Coin(Texture2D coinTexture, Vector2 pos)
        {
            coinAnim = new Animation(coinTexture, 32, 32);
            rect = new Rectangle((int)pos.X, (int)pos.Y, 32, 32);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            coinAnim.Draw(spriteBatch, new Vector2(rect.X, rect.Y),gameTime);
        }
    }
}
