using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.NavMesh;
using Game1.AStar;
using Game1.Town;
using Game1.Characters;

namespace Game1
{
    class Player: People
    {
        
        public Player(Model _model, Vector3 _position, Mesh argMesh, Town.Town argTown, Game1 argGame) :base(_model, _position, argMesh, argTown, argGame)
        {
            
        }


        public override void Update(GameTime gameTime, GraphicsDevice graphicsDevice, Matrix projection, Matrix view, Game1 game)
        {
            
            goapStateMachine.Tick(gameTime);


            MouseState currentMouseState = Mouse.GetState();


            if ()

            
            if (actionState == PeopleActionStates.selectingItemAction && game.IsActive)
            {

                if (currentMouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
                {
                    Button selectedButton = MouseInput.GetButtonPressed(selectedItem.actionButtons);
                    if (selectedButton != null)
                    {
                        Console.WriteLine("Selected action:" + selectedButton.buttonLabel);
                        game.buttons.Clear();


                        goapPerson.PushNewAction(selectedButton.buttonAction);

                        //goal = selectedItem.townLocation;
                        actionState = PeopleActionStates.moving;
                        //BuildPath();

                    }
                    else
                    {
                        game.buttons.Clear();
                        actionState = PeopleActionStates.idle;
                    }
 
                }

            }


            else if (currentMouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released && game.IsActive) //if user has clicked mouse, indicating they want to move the avatar
            {
                bool isItemSelected = MouseInput.FindItemSelected(currentHouse, graphicsDevice, projection, view, ref selectedItem);
                if (isItemSelected)
                { 
                    //goal = selectedItem.townLocation;
                    actionState = PeopleActionStates.selectingItemAction;
                    DisplayItemLabels(selectedItem);
                    Console.WriteLine("Selected:"+selectedItem.id);
                }

                else
                {
                    
                    bool isHouseSelected = MouseInput.FindHouseSelected(town, graphicsDevice, projection, view, ref goalHouse);
                    if (isHouseSelected)
                    {   if (goalHouse == currentHouse)
                        {
                            isHouseSelected = false;
                        }
                        else
                        {
                            goal = goalHouse.townLocation;
                            actionState = PeopleActionStates.moving;
                            BuildPath();
                            Console.WriteLine("Selected:" + goalHouse.id); 

                        }
                        
                    }
                    if (!isHouseSelected)
                    {
                        goal = MouseInput.MousePosToWorldCoordinates(graphicsDevice, projection, view); //finds the position user has clicked in the 3d world and sets this as the avatar's target position

                        if (goal.Y != -100)
                        {
                            goalHouse = House.getHouseContainingPoint(goal);
                            actionState = PeopleActionStates.moving;
                            BuildPath();
                        }

                        

                        
                    }

                }
                
                
              
                //if (goal.Y != -100)
                //{

                //    //pathFinder.FindPath(position, goal, ref pathPoints);
                //    actionState = PeopleActionStates.moving;
                  
                //    BuildPath();

                    

                //    //foreach (Vector3 point in pathPoints)
                //    //{
                //    //    Console.WriteLine(point.ToString());

                //    //}
                    


                //    //MovePerson(gameTime);


                //    //targetVector = getTargetVector(gameTime); //assigns target vector to variable
                //    //motionState = PeopleMotionStates.rotating; //avatar must first rotate to look at the point they are going to move towards so the avatar state is set to rotating
                //    //getNewRotationMatrix(gameTime); // updates rotation matrix

                //}
                ////else
                ////{
                ////    motionState = PeopleMotionStates.moving;
                ////}
                
                

            }
            
            else if (actionState == PeopleActionStates.beginMoving)
            {
                actionState = PeopleActionStates.moving;
                BuildPath();
                MovePerson(gameTime);

            }

            else if (actionState == PeopleActionStates.moving)
            {
                
                MovePerson(gameTime);
            }

            

            prevMouseState = currentMouseState;

            updateTransformationMatrix(); //updates transformation matrix with transformations occured in current frame 

            avatar.worldMatrix *= transformationMatrix; //transforms world matrix by transformations specified in transformation matrix
            viewVector = avatar.worldMatrix.Forward; //adjusts view vector of model
            position = avatar.worldMatrix.Translation; //adjusts position variable to position in model's world matrix

        }


        public void DisplayItemLabels(Item selectedItem)
        {
            selectedItem.DisplayActions(game);
        }


        public override void MovePerson(GameTime gameTime)
        {


            if (pathPoints.Count == 0)
            {

                motionState = PeopleMotionStates.idle;
                actionState = PeopleActionStates.idle;
                //currentHouse = House.getHouseContainingPoint(position);
                reachedGoal = true;
                currentHouse = goalHouse;
                return;
            }

            if (motionState == PeopleMotionStates.idle && pathPoints.Count > 0)
            {
                Vector3 nextTarget = pathPoints[0];
                targetVector = getTargetVector(gameTime, nextTarget);

                House nextHouse = House.getHouseContainingPoint(nextTarget);



                if ( nextHouse == null && currentHouse!= null)
                {
                    game.AddAvatar(currentHouse.wallAvatars.Find(a => a.model == currentHouse.roofModel));
                }
                else if (nextHouse != null && nextHouse != currentHouse)
                {
                    game.RemoveAvatar(nextHouse.wallAvatars.Find(a => a.model == nextHouse.roofModel));
                }

                if (!(targetVector.X == 0 && targetVector.Z == 0))
                {
                    motionState = PeopleMotionStates.rotating;

                    getNewRotationMatrix(gameTime);

                }



            }


            if (motionState == PeopleMotionStates.rotating)
            {
                getNewRotationMatrix(gameTime);
            }


            if (motionState == PeopleMotionStates.moving)
            {
                if (pathPoints.Count == 0)
                {
                    motionState = PeopleMotionStates.idle;
                    actionState = PeopleActionStates.idle;
                    //currentHouse = House.getHouseContainingPoint(position);
                    reachedGoal = true;
                    currentHouse = goalHouse;
                    return;
                }


                Vector3 currentTarget = pathPoints[0];

                

                targetVector = getTargetVector(gameTime, currentTarget);

                rotationMatrix = Matrix.Identity;
                getNewTranslationMatrix(gameTime);



            }

        }









    }
}
