using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.Rooms;
using Game1.Town;
using Game1.Town.Districts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Characters;

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





        private static Ray GetRay(GraphicsDevice GraphicsDevice, Matrix projection, Matrix view)
        {
            MouseState ms = Mouse.GetState();

            Vector3 nearScreenPoint = new Vector3(ms.X, ms.Y, 0.1f);
            Vector3 farScreenPoint = new Vector3(ms.X, ms.Y, 0.2f);
            Vector3 nearWorldPoint = GraphicsDevice.Viewport.Unproject(nearScreenPoint, projection, view, Matrix.Identity);
            Vector3 farWorldPoint = GraphicsDevice.Viewport.Unproject(farScreenPoint, projection, view, Matrix.Identity);

            Vector3 direction = farWorldPoint - nearWorldPoint;
            direction.Normalize();

            Ray ray = new Ray(nearWorldPoint, direction);
            return ray;

        }


        public static bool FindItemSelected(House currentHouse, GraphicsDevice graphicsDevice, Matrix projection, Matrix view, ref Item itemSelected)
        {



            if (currentHouse == null) { return false; }
            MouseState ms = Mouse.GetState();

            Vector3 nearScreenPoint = new Vector3(ms.X, ms.Y, 0.1f);
            Vector3 farScreenPoint = new Vector3(ms.X, ms.Y, 0.2f);
            Vector3 nearWorldPoint = graphicsDevice.Viewport.Unproject(nearScreenPoint, projection, view, Matrix.Identity);
            Vector3 farWorldPoint = graphicsDevice.Viewport.Unproject(farScreenPoint, projection, view, Matrix.Identity);

            Vector3 direction = farWorldPoint - nearWorldPoint;
            direction.Normalize();

            Ray ray = new Ray(nearWorldPoint, direction);

            

            foreach (Room room  in currentHouse.rooms)
            {
                foreach (Item item in room.items)
                {
                    foreach (Plane plane in item.planes)
                    {
                        var intersection = ray.Intersects(plane);
                        if (intersection.HasValue)
                        {
                            ray.Direction.Normalize();
                            Vector3 intersectionPoint = nearWorldPoint + direction * intersection.Value;

                            if (item.ContainsPoint(intersectionPoint))
                            {
                                itemSelected = item;
                                return true;

                            }

                           
                        }


                    }
                }


            }
            return false;



        }

        public static bool FindHouseSelected(Town.Town town , GraphicsDevice graphicsDevice, Matrix projection, Matrix view, ref House houseSelected)
        {

            if (town == null) { return false; }
            MouseState ms = Mouse.GetState();

            Vector3 nearScreenPoint = new Vector3(ms.X, ms.Y, 0.1f);
            Vector3 farScreenPoint = new Vector3(ms.X, ms.Y, 0.2f);
            Vector3 nearWorldPoint = graphicsDevice.Viewport.Unproject(nearScreenPoint, projection, view, Matrix.Identity);
            Vector3 farWorldPoint = graphicsDevice.Viewport.Unproject(farScreenPoint, projection, view, Matrix.Identity);

            Vector3 direction = farWorldPoint - nearWorldPoint;
            direction.Normalize();

            Ray ray = new Ray(nearWorldPoint, direction);



            foreach (House house in town.houses)
            { 
                foreach (Plane plane in house.planes)
                {
                    var intersection = ray.Intersects(plane);
                    if (intersection.HasValue)
                    {
                        ray.Direction.Normalize();

                        Vector3 intersectionPoint = nearWorldPoint + direction * intersection.Value;
                        if (house.inHouse(intersectionPoint))
                        {
                            houseSelected = house;
                        
                            return true;

                        }
                        

                        
                    }


                }



            }
               


            
            return false;



        }


        public static Button  GetButtonPressed(List<Button> buttonsToCheck)
        {
            MouseState ms = Mouse.GetState();
            Console.WriteLine(ms.X+" "+ms.Y);
            foreach (Button button in buttonsToCheck)
            {

                if (MouseOverButton(button, ms))
                {
                    return button;
                }
                

            }

            return null;
        }


        private static bool MouseOverButton(Button button, MouseState ms)
        {

            Rectangle buttonRect = new Rectangle((int)button.position.X, (int)button.position.Y, button.buttonTexture.Width, button.buttonTexture.Height);

            if (buttonRect.Contains(ms.X, ms.Y))
            {
                return true;
            }

            return false;


        }





    }
}
