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

        /// <summary>
        ///  Sets and returns currentMouseState 
        /// </summary>
        /// <returns></returns>
        public static MouseState GetState()
        {
            
            currentMouseState = Mouse.GetState();
            return currentMouseState;
        }

        /// <summary>
        /// Sets previousMouseState to currentMouseState 
        /// </summary>
        public static void SetPreviousState()
        {
            previousMouseState = currentMouseState;
        }


        /// <summary>
        /// Returns if the left button of the mouse is being clicked and updates currentMouseState. 
        /// </summary>
        /// <returns></returns>
        public static bool WasLeftClicked()
        {

            GetState();
            return currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released;
        }


        /// <summary>
        /// Returns if the specified mouse button is currently held down 
        /// </summary>
        /// <param name="left"></param>
        /// <returns></returns>
        public static bool IsPressed (bool left)
        {
            if (left) { return currentMouseState.LeftButton == ButtonState.Pressed; }
            else { return currentMouseState.RightButton == ButtonState.Pressed; }
        }

        /// <summary>
        /// Returns if specified mouse button is being clicked
        /// </summary>
        /// <param name="left"></param>
        /// <returns></returns>
        public static bool HasNotBeenPressed(bool left)
        {
            if (left) { return currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released; }
            else { return currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Released; }
        }

        /// <summary>
        /// Determines the 3D world coordinates or a user mouse click on a 2D screen. Uses mouse X and Y coordinates to create a near and far screen point which is then unprojected using the Camera object’s projection and view matrices in order to produce near and far world points. A direction vector is then calculated between the near and far world points in order to produce a Ray object. The intersection between this ray and ground plane of the world is determined to be the selected point. 
        /// </summary>
        /// <param name="GraphicsDevice"></param>
        /// <param name="projection"></param>
        /// <param name="view"></param>
        /// <returns></returns>
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




        /// <summary>
        /// Builds Ray object using near and far screen points and near and far world points. 
        /// </summary>
        /// <param name="GraphicsDevice"></param>
        /// <param name="projection"></param>
        /// <param name="view"></param>
        /// <returns></returns>
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

        /// <summary>
        ///  Examines each item in the current house to determine if the user has selected the item. Uses list of planes constructed by item to determine if the mouse ray intersects one of these planes and if the point of intersection is within the item; if both of these condition are true, the item has been selected.  
        /// </summary>
        /// <param name="currentHouse"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="projection"></param>
        /// <param name="view"></param>
        /// <param name="itemSelected"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Applies similar principle to FindItemSelected method but instead iterates over each house in the town and checks planes of house. 
        /// </summary>
        /// <param name="town"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="projection"></param>
        /// <param name="view"></param>
        /// <param name="houseSelected"></param>
        /// <returns></returns>
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



        /// <summary>
        /// Examines each person and determines if mouse ray intersects with that persons BoundingBox. The get method on each person’s BoundingBox will automatically update the BoundingBox using method in Avatar class. This added step of updating the structure used to determine if an intersection has occurred is only necessary for the people objects as they have dynamic positions. 
        /// </summary>
        /// <param name="people"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="projection"></param>
        /// <param name="view"></param>
        /// <param name="personSelected"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Examines each building and determines if mouse ray intersects with the building’s BoundingBox I.e. has been selected. 
        /// </summary>
        /// <param name="buildings"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="projection"></param>
        /// <param name="view"></param>
        /// <param name="buildingSelected"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns if exit button has been pressed.
        /// </summary>
        /// <param name="exitButton"></param>
        /// <returns></returns>
        public static bool IsExitButtonPressed(ExitButton exitButton)
        {
            return MouseOverButton(exitButton, currentMouseState);

        }



        /// <summary>
        /// Returns if emotion button has been pressed.
        /// </summary>
        /// <param name="emotionButton"></param>
        /// <returns></returns>
        public static bool IsEmotionButtonPressed(EmotionButton emotionButton) { return MouseOverButton(emotionButton, currentMouseState); }


        /// <summary>
        /// Returns if career feedback button has been pressed.
        /// </summary>
        /// <param name="careerFeedbackButton"></param>
        /// <returns></returns>
        public static bool IsCareerFeedbackButtonPressed(CareerFeedbackButton careerFeedbackButton) { if (careerFeedbackButton == null) { return false; } return MouseOverButton(careerFeedbackButton, currentMouseState); }



        /// <summary>
        /// Returns button user has pressed. 
        /// </summary>
        /// <param name="buttonsToCheck"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Returns toolbar button user has pressed. 
        /// </summary>
        /// <param name="toolbarButtons"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Determines if the users mouse position is with the bounds of the button’s rectangle.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="currentMouseState"></param>
        /// <returns></returns>
        private static bool MouseOverButton(Button button, MouseState currentMouseState)
        {

            Rectangle buttonRect = new Rectangle((int)button.position.X, (int)button.position.Y, button.buttonTexture.Width, button.buttonTexture.Height);

            return MouseOverRectangle(buttonRect);


        }

        /// <summary>
        ///  Determines if the user’s mouse position is within the bounds of the supplied rectangle. 
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
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
