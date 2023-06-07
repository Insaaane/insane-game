using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace InsaneGame.files
{
    public class GameManager
    {
        private Rectangle endRectangle;

        public GameManager(Rectangle endRectangle)
        {
            this.endRectangle = endRectangle;
        }

        public bool HasGameEnded(Rectangle playerHitbox)
        {
            return endRectangle.Intersects(playerHitbox);
        }
    }
}
