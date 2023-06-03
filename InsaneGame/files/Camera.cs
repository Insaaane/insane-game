using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsaneGame.files
{
    public class Camera
    {
        public Matrix Transform;

        public Matrix Follow(Rectangle target) 
        {
            var translation = new Vector3(-target.X - target.Width / 2, -target.Y - target.Height / 2, 0);
            var offset = new Vector3(Game1.screenWidth / 2, Game1.screenHeight / 2, 0);

            Transform = Matrix.CreateTranslation(translation) * Matrix.CreateTranslation(offset);
            return Transform;
        }
    }
}
