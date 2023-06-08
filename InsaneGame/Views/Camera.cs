using Microsoft.Xna.Framework;

namespace InsaneGame.files
{
    public class Camera
    {
        public Matrix Transform;

        private Vector2 position;
        private Vector2 targetPosition;
        private float lerpFactor = 0.15f;

        public int MapWidth = 3520;
        public int MapHeight = 2000;

        public Matrix Follow(Rectangle target) 
        {
            targetPosition.X = -target.X - target.Width / 2 + Main.screenWidth / 2;
            targetPosition.Y = -target.Y - target.Height / 2 + Main.screenHeight / 2;

            targetPosition.X = MathHelper.Clamp(targetPosition.X, -MapWidth + Main.screenWidth, 0);
            targetPosition.Y = MathHelper.Clamp(targetPosition.Y, -MapHeight + Main.screenHeight, 0);

            position = Vector2.Lerp(position, targetPosition, lerpFactor);

            Transform = Matrix.CreateTranslation(new Vector3(position, 0));

            return Transform;
        }
    }
}
