﻿using System;
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
using Game1.Careers;
using Game1.Traits;
using Game1.Skills;
using Game1.Characters;

namespace Game1
{
    /// <summary>
    /// Enumeration to store possible user selection states. 
    /// </summary>
    public enum PeopleSelectingState
    {
        none,
        ItemAction,
        BuildingAction
        
    }




    public class Player : People
    {

        PeopleSelectingState selectingState = PeopleSelectingState.none;

       

        /// <summary>
        /// Constructor to make new Player object
        /// </summary>
        /// <param name="_model"></param>
        /// <param name="_position"></param>
        /// <param name="argMesh"></param>
        /// <param name="argTown"></param>
        /// <param name="argGame"></param>
        /// <param name="argIcon"></param>
        /// <param name="argDBID"></param>
        /// <param name="argName"></param>
        /// <param name="argHouse"></param>
        /// <param name="argCareer"></param>
        /// <param name="argTraits"></param>
        /// <param name="argNeeds"></param>
        /// <param name="argSkills"></param>
        public Player(Model _model, Vector3 _position, Mesh argMesh, Town.Town argTown, Game1 argGame, Texture2D argIcon, int argDBID, string argName, House argHouse, Career argCareer, List<Trait> argTraits, Dictionary<NeedNames, Need> argNeeds, List<Skill> argSkills) : base(_model, _position, argMesh, argTown, argGame, argIcon, argDBID, argName, argHouse, argCareer, argTraits, argNeeds, argSkills, true)
        {
            #region Draw Path Points
            drawPathPoints = true;
            #endregion

        }

    /// <summary>
    /// Overrides base class Update(). Calls DepleteNeeds method. Ticks GOAP state machine. Determines if user selection is taking place and, using MouseInput class methods, determines what the user is attempting to select e.g. NPC, item, house, button etc. If the user has selected an action by clicking on a button, this action is loaded into their action queue. Calls methods to handle People movement and update transformation matrix which is then applied to avatar’s world matrix to allow avatar to be rendered properly. Relevant UI changes made using UIHandler class. 
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="graphicsDevice"></param>
    /// <param name="projection"></param>
    /// <param name="view"></param>
    /// <param name="game"></param>
    public override void Update(GameTime gameTime, GraphicsDevice graphicsDevice, Matrix projection, Matrix view, Game1 game)
        {

            #region Draw Path Points
            bool generatePathPoints = false;
            #endregion




            DepleteNeeds(gameTime);
            emotionalState = GetEmotionalState();

            goapStateMachine.Tick(gameTime);


            MouseInput.GetState();

            //problem is being set into moving state even when should be selecting item/building action -- need to fix that
            //add a peopleselecting state (item, building, person) 

            if (MouseInput.WasLeftClicked() && game.IsActive)
            {
                ToolbarButton selectedToolBarButton = MouseInput.GetToolbarButton(game.UIHandler.toolbarButtons);

                if (MouseInput.IsExitButtonPressed(game.UIHandler.exitButton))
                {
                    // End game, save game state
                    ExitButton.SaveAndExit(people, game);

                }
                else if (MouseInput.IsCareerFeedbackButtonPressed(game.UIHandler.careerFeedbackButton))
                {
                    CloseFeedback();
                }

                else if (MouseInput.IsEmotionButtonPressed(game.UIHandler.emotionButton))
                {
                    DisplayEmotion(game.UIHandler.emotionButton);
                }

                else if (selectingState == PeopleSelectingState.ItemAction)
                {
                    Button selectedButton = MouseInput.GetButtonPressed(selectedItem.actionButtons);
                    if (selectedButton != null)
                    {
                        Console.WriteLine("Selected action:" + selectedButton.buttonLabel);
                        game.UIHandler.ClearButtons();
                        goapPerson.PushNewAction(selectedButton.buttonAction);
                        //actionState = PeopleActionStates.moving;


                    }
                    else
                    {
                        game.UIHandler.ClearButtons();
                        actionState = PeopleActionStates.idle;
                        selectingState = PeopleSelectingState.none;
                    }

                }

                else if (selectingState == PeopleSelectingState.BuildingAction)
                {
                    Button selectedButton = MouseInput.GetButtonPressed(selectedBuilding.actionButtons);
                    if (selectedButton != null)
                    {
                        Console.WriteLine("Selected action:" + selectedButton.buttonLabel);
                        game.UIHandler.ClearButtons();
                        goapPerson.PushNewAction(selectedButton.buttonAction);
                        //actionState = PeopleActionStates.moving;


                    }
                    else
                    {
                        game.UIHandler.ClearButtons();
                        actionState = PeopleActionStates.idle;
                        selectingState = PeopleSelectingState.none;
                    }

                }





                else if (selectedToolBarButton != null)
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

                    selectingState = PeopleSelectingState.ItemAction;
                    DisplayItemLabels(selectedItem);
                    Console.WriteLine("Selected:" + selectedItem.id);
                }

                else if (MouseInput.FindHouseSelected(town, graphicsDevice, projection, view, ref goalHouse) && goalHouse != currentHouse) //is a new house selected
                {
                    goal = goalHouse.TownLocation;
                    actionState = PeopleActionStates.moving;
                    Console.WriteLine("Selected:" + goalHouse.id);

                    BuildPath();

                    #region Draw Path Points
                    generatePathPoints = true;
                    #endregion


                }

                else if (MouseInput.FindBuildingSelected(town.buildings, graphicsDevice, projection, view, ref selectedBuilding))
                {
                    selectingState = PeopleSelectingState.BuildingAction;
                    DisplayBuildingLabels(selectedBuilding);
                    Console.WriteLine("Selected:" + selectedBuilding.id);

                }

                else
                {
                    goal = MouseInput.MousePosToWorldCoordinates(graphicsDevice, projection, view); //finds the position user has clicked in the 3d world and sets this as the avatar's target position

                    if (goal.Y != -100)
                    {
                        goalHouse = House.getHouseContainingPoint(goal);
                        actionState = PeopleActionStates.moving;

                        BuildPath();

                        #region Draw Path Points
                        generatePathPoints = true;
                        #endregion

                    }

                }


            }





            else if (actionState == PeopleActionStates.beginMoving)
            {
                actionState = PeopleActionStates.moving;
                BuildPath();

                #region Draw Path Points
                generatePathPoints = true;
                #endregion


                MovePerson(gameTime);

            }

            else if (actionState == PeopleActionStates.moving)
            {

                MovePerson(gameTime);
            }


            #region Draw Path Points
            if (drawPathPoints)
            {
                if (generatePathPoints)
                {
                    PathPoint.BuildRangePathPoints(pathPoints);
                }
            }
            #endregion






            // prevMouseState = MouseInput.currentMouseState;

            updateTransformationMatrix(); //updates transformation matrix with transformations occured in current frame 

            avatar.worldMatrix *= transformationMatrix; //transforms world matrix by transformations specified in transformation matrix
            viewVector = avatar.worldMatrix.Forward; //adjusts view vector of model
            position = avatar.worldMatrix.Translation; //adjusts position variable to position in model's world matrix

        }


        /// <summary>
        /// Overriden method to display career feedback to player. Calls CreateNewCareerFeedbackButton in UI class to display feedback to user.
        /// </summary>
        public override void DisplayCareerFeedback(FeedbackScore feedback)
        {
            game.UIHandler.CreateNewCareerFeedbackButton(feedback);

        }

        /// <summary>
        /// Sets UIHandler.careerFeedbackButton to null to close career feedback view.
        /// </summary>
        public void CloseFeedback()
        {
            this.game.UIHandler.careerFeedbackButton = null;
        }

        /// <summary>
        /// Sets UIHandler.displayTextbox attribute to true to display textbox for user input.
        /// </summary>
        /// <param name="game"></param>
        public void DisplayTextbox(Game1 game)
        {
            game.UIHandler.displayTextbox = true;
        }

        /// <summary>
        /// Calls DisplayActions method on selected item in order to display action labels for user to select from.
        /// </summary>
        /// <param name="selectedItem"></param>
        public void DisplayItemLabels(Item selectedItem)
        {
            selectedItem.DisplayActions(game);
        }

        /// <summary>
        /// Calls DisplayActions method on selected building in order to display action labels for user to select from.
        /// </summary>
        /// <param name="selectedBuilding"></param>
        public void DisplayBuildingLabels(Building selectedBuilding)
        {
            selectedBuilding.DisplayActions(game, this);
        }


        /// <summary>
        /// Toggles emotion button panel display – showing/hiding emotional state from user.
        /// </summary>
        /// <param name="button"></param>
        public void DisplayEmotion(EmotionButton button)
        {
            button.panel.IsDisplayed = !button.panel.IsDisplayed;
           
        }

        /// <summary>
        /// Toggles toolbar button panel display. If a new panel is being opened, all panels closed first. If user has selected an already opened panel, the panel is closed.
        /// </summary>
        /// <param name="button"></param>
        public void DisplayToolBarPanel(ToolbarButton button)
        {
            if (!button.panel.IsDisplayed) //if opening a new panel, close open panel first
            {
                game.UIHandler.ClosePanels();
            }


            button.panel.IsDisplayed = !button.panel.IsDisplayed;
        }


        /// <summary>
        /// Creates new SentimentData obect with user input conversation text. Uses MLMain.PredictWithSubject to determine if the user has input dialogue with a positive or negative sentiment towards the subject.
        /// </summary>
        /// <param name="text"></param>
        public void ReceiveConversationData(string text)
        {

            SentimentData sentimentData = new SentimentData { SentimentText = text};
            SentimentPrediction prediction = MLMain.PredictWithSubject(sentimentData);
            




            UpdateRelationship(prediction.Prediction);


            
            actionState = PeopleActionStates.idle;
            
     

        }




        /// <summary>
        /// Updates the player’s relationship with the person selected from interaction using sentiment prediction on the player’s input text. A relationship is created if it does not already exist. Relationship is incremented if the sentiment was deemed positive and decremented if deemed negative.
        /// </summary>
        /// <param name="prediction"></param>
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

        /// <summary>
        /// Very similar to base method however has added functionality of removing and adding roofs to houses depending on if the player is currently inside the house to allow them to obtain a better view of the house interior.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void MovePerson(GameTime gameTime)
        {


            if (pathPoints.Count == 0)
            {

                motionState = PeopleMotionStates.idle;
                actionState = PeopleActionStates.idle;
                //currentHouse = House.getHouseContainingPoint(position);
                reachedGoal = true;
                currentHouse = goalHouse;
                //currentBuilding = goalBuilding;

                #region Draw Path Points
                if (drawPathPoints)
                {
                    PathPoint.ClearPathPoints();
                }                
                #endregion


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
                    //currentBuilding = goalBuilding;


                    #region Draw Path Points
                    if (drawPathPoints)
                    {
                        PathPoint.ClearPathPoints();
                    }
                    #endregion

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
