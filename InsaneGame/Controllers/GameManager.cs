using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace InsaneGame.files
{
    public class GameManager
    {
        private Rectangle rect;

        public GameManager(Rectangle endRectangle)
        {
            this.rect = endRectangle;
        }

        public bool HasGameEnded(Rectangle playerHitbox)
        {
            return rect.Intersects(playerHitbox);
        }

        public bool PlayerIsDead(Rectangle playerHitbox)
        {
            return rect.Intersects(playerHitbox);
        }
    }
}
