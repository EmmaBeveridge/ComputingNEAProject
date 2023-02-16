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

using Game1.UI;

namespace Game1
{
    static class MouseInput
    {


        public static MouseState previousMouseState;
        public static MouseState currentMouseState;

        public static MouseState GetState()
        {
            
            currentMouseState = Mouse.GetState();
            return currentMouseState;
        }


        public static void SetPreviousState()
        {
            previousMouseState = currentMouseState;
        }


        public static bool WasLeftClicked()
        {

            GetState();
            return currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released;
        }



        public static bool IsPressed (bool left)
        {
            if (left) { return currentMouseState.LeftButton == ButtonState.Pressed; }
            else { return currentMouseState.RightButton == ButtonState.Pressed; }
        }


        public static bool HasNotBeenPressed(bool left)
        {
            if (left) { return currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released; }
            else { return currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Released; }
        }


        public static Vector3 MousePosToWorldCoordinates(GraphicsDevice GraphicsDevice, Matrix projection, Matrix view)
        {


            //MouseState ms = Mouse.GetState();

            //MouseState ms = GetState();

            Vector3 nearScreenPoint = new Vector3(currentMouseState.X, currentMouseState.Y, 0.1f);
            Vector3 farScreenPoint = new Vector3(currentMouseState.X, currentMouseState.Y, 0.2f);
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

                Console.WriteLine("\t\t\tCLICKED"+pickedPosition);

                return pickedPosition;
            }
            Console.WriteLine("Click not valid");
            return new Vector3(0, -100, 0); //used as null return as Vector3 non nullable - no valid return will have y!=0
                   
        }





        private static Ray GetRay(GraphicsDevice GraphicsDevice, Matrix projection, Matrix view)
        {
            //MouseState currentMouseState = GetState();

            Vector3 nearScreenPoint = new Vector3(currentMouseState.X, currentMouseState.Y, 0.1f);
            Vector3 farScreenPoint = new Vector3(currentMouseState.X, currentMouseState.Y, 0.2f);
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
            //MouseState currentMouseState = GetState();
            Vector3 nearScreenPoint = new Vector3(currentMouseState.X, currentMouseState.Y, 0.1f);
            Vector3 farScreenPoint = new Vector3(currentMouseState.X, currentMouseState.Y, 0.2f);
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
            //MouseState currentMouseState = GetState();
            Vector3 nearScreenPoint = new Vector3(currentMouseState.X, currentMouseState.Y, 0.1f);
            Vector3 farScreenPoint = new Vector3(currentMouseState.X, currentMouseState.Y, 0.2f);
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




        public static bool FindPersonSelected(List<People> people, GraphicsDevice graphicsDevice, Matrix projection, Matrix view, ref People personSelected)
        {
            Ray ray = GetRay(graphicsDevice, projection, view);
            foreach (People person in people)
            {
                if (person.boundingBox.Intersects(ray) != null)
                {
                    personSelected = person;
                    return true;

                }
            }

            return false;
        }



        public static bool FindBuildingSelected(List<Building> buildings, GraphicsDevice graphicsDevice, Matrix projection, Matrix view, ref Building buildingSelected)
        {
            Ray ray = GetRay(graphicsDevice, projection, view);
            foreach (Building building in buildings)
            {
                if (building.buildingBox.Intersects(ray) != null)
                {
                    buildingSelected = building;
                    return true;

                }
            }

            return false;
        }

        public static bool IsExitButtonPressed(ExitButton exitButton)
        {
            return MouseOverButton(exitButton, currentMouseState);

        }




        public static bool IsEmotionButtonPressed(EmotionButton emotionButton) { return MouseOverButton(emotionButton, currentMouseState); }


        public static Button GetButtonPressed(List<Button> buttonsToCheck)
        {
            currentMouseState = Mouse.GetState();
            if (currentMouseState.X != 0 || currentMouseState.Y != 0)
            {
                
                Console.WriteLine(currentMouseState.X+" "+currentMouseState.Y);

            }
            
            foreach (Button button in buttonsToCheck)
            {

                if (MouseOverButton(button, currentMouseState))
                {
                    return button;
                }
                

            }

            return null;
        }



        public static ToolbarButton GetToolbarButton(List<ToolbarButton> toolbarButtons)
        {
            //MouseState currentMouseState = Mouse.GetState();
            foreach(ToolbarButton button in toolbarButtons)
            {
                if(MouseOverButton(button, currentMouseState))
                {
                    return button;
                }
            }

            return null;
        }


        private static bool MouseOverButton(Button button, MouseState currentMouseState)
        {

            Rectangle buttonRect = new Rectangle((int)button.position.X, (int)button.position.Y, button.buttonTexture.Width, button.buttonTexture.Height);

            return MouseOverRectangle(buttonRect);


        }


        public static bool MouseOverRectangle(Rectangle rectangle)
        {
            if (rectangle.Contains(currentMouseState.X, currentMouseState.Y))
            {
                return true;
            }

            return false;


        }


    }
}
