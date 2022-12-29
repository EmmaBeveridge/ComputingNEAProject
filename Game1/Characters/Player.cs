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

using Game1.ML;
using Game1.UI;

namespace Game1
{
    public class Player : People
    {

        public Player(Model _model, Vector3 _position, Mesh argMesh, Town.Town argTown, Game1 argGame, Texture2D argIcon) : base(_model, _position, argMesh, argTown, argGame, argIcon, true)
        {

        }


        public override void Update(GameTime gameTime, GraphicsDevice graphicsDevice, Matrix projection, Matrix view, Game1 game)
        {


            goapStateMachine.Tick(gameTime);


            MouseInput.GetState();


            if (MouseInput.currentMouseState.LeftButton == ButtonState.Pressed && MouseInput.previousMouseState.LeftButton == ButtonState.Released && game.IsActive)
            {
                ToolbarButton selectedToolBarButton = MouseInput.GetToolbarButton(game.UIHandler.toolbarButtons);

                if (actionState == PeopleActionStates.selectingItemAction)
                {
                    Button selectedButton = MouseInput.GetButtonPressed(selectedItem.actionButtons);
                    if (selectedButton != null)
                    {
                        Console.WriteLine("Selected action:" + selectedButton.buttonLabel);
                        game.UIHandler.ClearButtons();
                        goapPerson.PushNewAction(selectedButton.buttonAction);
                        actionState = PeopleActionStates.moving;


                    }
                    else
                    {
                        game.UIHandler.ClearButtons();
                        actionState = PeopleActionStates.idle;
                    }

                }
                
                

                else if(selectedToolBarButton != null)
                {
                    DisplayToolBarPanel(selectedToolBarButton);

                }
                
                else if (actionState == PeopleActionStates.typingChat) { }




                else if (MouseInput.FindPersonSelected(game.people, graphicsDevice, projection, view, ref selectedPerson)) //is person selected
                {
                    Console.WriteLine("Selected Person");
                    actionState = PeopleActionStates.typingChat;
                    DisplayTextbox(game);

                }
                

                else if (MouseInput.FindItemSelected(currentHouse, graphicsDevice, projection, view, ref selectedItem)) //is item selected
                {

                    actionState = PeopleActionStates.selectingItemAction;
                    DisplayItemLabels(selectedItem);
                    Console.WriteLine("Selected:" + selectedItem.id);
                }

                else if (MouseInput.FindHouseSelected(town, graphicsDevice, projection, view, ref goalHouse) && goalHouse != currentHouse) //is a new house selected
                {
                    goal = goalHouse.TownLocation;
                    actionState = PeopleActionStates.moving;
                    BuildPath();
                    Console.WriteLine("Selected:" + goalHouse.id);



                }
                else
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

            

           // prevMouseState = MouseInput.currentMouseState;

            updateTransformationMatrix(); //updates transformation matrix with transformations occured in current frame 

            avatar.worldMatrix *= transformationMatrix; //transforms world matrix by transformations specified in transformation matrix
            viewVector = avatar.worldMatrix.Forward; //adjusts view vector of model
            position = avatar.worldMatrix.Translation; //adjusts position variable to position in model's world matrix

        }

        public void DisplayTextbox(Game1 game)
        {
            game.UIHandler.displayTextbox = true;
        }
        public void DisplayItemLabels(Item selectedItem)
        {
            selectedItem.DisplayActions(game);
        }


        public void DisplayToolBarPanel(ToolbarButton button)
        {
            if (!button.panel.IsDisplayed) //if opening a new panel, close open panel first
            {
                game.UIHandler.ClosePanels();
            }


            button.panel.IsDisplayed = !button.panel.IsDisplayed;
        }


        public void ReceiveConversationData(string text)
        {

            SentimentData sentimentData = new SentimentData { SentimentText = text};
            SentimentPrediction prediction = MLMain.PredictWithSubject(sentimentData);
            




            UpdateRelationship(prediction.Prediction);


            
            actionState = PeopleActionStates.idle;
            
     

        }





        private void UpdateRelationship(bool prediction)
        {
            if (!Relationships.ContainsKey(selectedPerson))
            {
                Relationships.Add(selectedPerson, 50);
            }
            if (prediction)
            {
                Relationships[selectedPerson] = Math.Min(Relationships[selectedPerson]+1, 100);
            }
            else
            {
                Relationships[selectedPerson] = Math.Max(Relationships[selectedPerson]-1, 0);
            }
            Console.WriteLine($"Relationship: {Relationships[selectedPerson]}");
            selectedPerson = null;
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

                if (!(targetVector.X == 0 && targetVector.Z == 0)) //need margin of error here???
                {
                    motionState = PeopleMotionStates.rotating;

                    //getNewRotationMatrix(gameTime);

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
