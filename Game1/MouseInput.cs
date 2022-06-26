using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    static class MouseInput
    {

        public static Vector3 MousePosToWorldCoordinates(GraphicsDevice GraphicsDevice, Matrix projection, Matrix view)
        {


            MouseState ms = Mouse.GetState();

            Vector3 nearScreenPoint = new Vector3(ms.X, ms.Y, 0.1f);
            Vector3 farScreenPoint = new Vector3(ms.X, ms.Y, 0.2f);
            Vector3 nearWorldPoint = GraphicsDevice.Viewport.Unproject(nearScreenPoint, projection, view, Matrix.Identity);
            Vector3 farWorldPoint = GraphicsDevice.Viewport.Unproject(farScreenPoint, projection, view, Matrix.Identity);

            Vector3 direction = farWorldPoint - nearWorldPoint;
            direction.Normalize();

            Ray ray = new Ray(nearWorldPoint, direction);

            Plane plane = new Plane(Vector3.Up, 0);

            var intersection = ray.Intersects(plane); //apply transformation matrix from camera/world to plane????
            if (intersection.HasValue)
            {
                Vector3 pickedPosition = nearWorldPoint + direction * intersection.Value;

                pickedPosition.Y = 0;

                Console.WriteLine(pickedPosition);

                return pickedPosition;
            }

            return new Vector3(0, -100, 0); //used as null return as Vector3 non nullable - no valid return will have y!=0
                   
        }

    }
}
