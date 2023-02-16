using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.NavMesh;
using Game1.AStar;
using Game1.Town;
using Game1.GOAP;
using Game1.Town.Districts;
using Game1.ID3;
using System.Data;
using Game1.DataClasses;
using Game1.Actions;
using Game1.Careers;
using Game1.Traits;

namespace Game1
{
    public enum PeopleMotionStates
    {
        idle,
        moving,
        rotating
    }

    public enum PeopleActionStates
    {
        idle,
        moving,
        engaged,
        beginMoving,
        selectingItemAction,
        selectingHouseAction,
        selectingBuildingAction,
        typingChat,
        finishedTypingChat
    }

    public enum PeopleEmotionalState
    {
        Comfortable, Uncomfortable, Playful, Tense, Energised, Lonely
    }
    public struct DBPerson //halfway person, stores data about person from SQLite DataBase
    {
        public int DBID;
        public string Name;
        public bool IsPlayer;
        public Career Career;
        public House House;
        public string ModelName;
        


        public DBPerson(int dbid, string name, bool isPlayer, Career career, House house, string modelName)
        {
            DBID = dbid;
            Name = name;
            IsPlayer = isPlayer;
            Career = career;
            House = house;
            ModelName = modelName;
        }




    }



    public class People
    {
        protected MouseState prevMouseState = new MouseState();



        public string Name;
        public House House;
        public Career Career;
        public int DBID;
        public bool isPlayer;



        public Vector3 position;
        //protected Vector3 targetPosition = Vector3.Zero;
        protected Vector3 targetVector = Vector3.Zero;
        protected Vector3 viewVector = Vector3.Forward;
        protected const float velocity = 100f;
        protected const float angularVelocity = 20f;

        protected Matrix transformationMatrix;
        protected Matrix rotationMatrix;
        protected Matrix translationMatrix;


        protected const float yValue = 0f;



        public Avatar avatar;
        public Texture2D icon;
        public string modelName;



        public PeopleMotionStates motionState;
        public PeopleActionStates actionState;

        public List<Vector3> pathPoints;
        protected Mesh mesh;
        protected Path pathFinder;
        public Vector3 goal;

        public House currentHouse;
        protected House goalHouse;

        public Building currentBuilding;
        public Building goalBuilding;


        protected Item selectedItem;
        protected People selectedPerson;
        protected Building selectedBuilding;

        public Town.Town town;
        protected Game1 game;


        //protected Agent agent;
        public GOAPPerson goapPerson;
        public StateMachine<GOAPPerson.GOAPPersonState> goapStateMachine;
        public bool reachedGoal = false;

        public BoundingBox boundingBox { get { return avatar.UpdateBoundingBox(); } }


        public Dictionary<People, float> Relationships;
        public Dictionary<NeedNames, Need> Needs;


        public List<Trait> Traits = new List<Trait>();

        public PeopleEmotionalState emotionalState;
       
        public static Tree decisionTree;
        public static List<People> people = new List<People>();



        public bool IsAvailable = true;



        public People(Model _model, Vector3 _position, Mesh argMesh, Town.Town argTown, Game1 argGame, Texture2D argIcon, int argDBID, string argName, House argHouse, Career argCareer, List<Trait> argTraits, Dictionary<NeedNames, Need> argNeeds, bool _isPlayer = false)
        {
            position = _position;
            avatar = new Avatar(_model, _position);
            motionState = PeopleMotionStates.idle;
            actionState = PeopleActionStates.idle;
            transformationMatrix = Matrix.Identity;
            translationMatrix = Matrix.CreateTranslation(_position);
            rotationMatrix = Matrix.Identity;
            avatar.worldMatrix = Matrix.Identity;
            viewVector = avatar.worldMatrix.Forward; //direction vector avatar is looking in
            mesh = argMesh;
            currentHouse = null;
            pathFinder = new Path(argMesh);
            pathPoints = new List<Vector3>();
            town = argTown;
            game = argGame;
            icon = argIcon;

            DBID = argDBID;
            Name = argName;
            House = argHouse;
            Career = argCareer;
            isPlayer = _isPlayer;



            Relationships = new Dictionary<People, float>();



            Traits = argTraits;
            Needs = argNeeds;

            //ConstructNeeds(_isPlayer); //need to move line somewhere else


            goapPerson = new GOAPPerson(this);
            //goapStateMachine = goapPerson.BuildAI();




        }


        /// <summary>
        /// Decrements person's need to cause depletion over time
        /// </summary>
        protected void DepleteNeeds(GameTime gameTime)
        {
            
            foreach (Need need in Needs.Values)
            {

                need.DepleteNeed(gameTime);


            }


        }

        protected PeopleEmotionalState GetEmotionalState()
        {

            var orderedNeedsAscending = Needs.OrderBy(kvp => kvp.Value.CurrentNeed);

            KeyValuePair<NeedNames, Need> mostUnfulfilled = orderedNeedsAscending.FirstOrDefault(kvp => kvp.Value.Level == NeedLevel.Low);

            if (mostUnfulfilled.Value == null)
            {
                var orderedNeedsDescending = Needs.OrderByDescending(kvp => kvp.Value.CurrentNeed);
                KeyValuePair<NeedNames, Need> mostFulfilled = orderedNeedsDescending.FirstOrDefault(kvp => kvp.Value.Level == NeedLevel.High);
                switch (mostFulfilled.Key)
                {
                   
                    case NeedNames.Sleep:
                        return PeopleEmotionalState.Energised;                 
                    case NeedNames.Fun:
                        return PeopleEmotionalState.Playful;
                    default:
                        return PeopleEmotionalState.Comfortable;
                }
            }
            else
            {
                switch (mostUnfulfilled.Key)
                {                                     
                    case NeedNames.Social:
                        return PeopleEmotionalState.Lonely;                 
                    case NeedNames.Fun:
                        return PeopleEmotionalState.Tense;
                    default:
                        return PeopleEmotionalState.Uncomfortable;
                }

            }

        }



        public void BuildAI()
        {
            //needs to be called after all talk to person actions created and added to town goap actions list
            goapStateMachine = goapPerson.BuildAI();
        }




        public void SendRSVP(GOAPAction recipientAction)
        {
            //should already be pushed onto initiator's action queue

            recipientAction.interactionPerson.ReceiveRSVP(this);
            recipientAction.Action.initiator = this;







        }



        public void ReceiveRSVP(People sender)
        {
            GOAPAction talkToSenderAction = town.GOAPActions.Find(a => a.interactionPerson == sender);

            talkToSenderAction.Action.initiator = sender;

            goapPerson.PushNewAction(talkToSenderAction);





        }


        public GOAPAction DefineActions()
        {
            TalkToPersonAction talk = new TalkToPersonAction(this);
            GOAPAction talkGOAP = talk.DefineGOAPAction();
            return talkGOAP;

        }




        public void UpdateRelationsAutonomous(People personInteractingWith)
        {
            const float relationshipAdjustment = 0.01f; //should be small as applied every update frame


            if (Relationships.ContainsKey(personInteractingWith))
            {
                if (Relationships[personInteractingWith] >= 50)
                {
                    Relationships[personInteractingWith] += relationshipAdjustment;

                }

                else
                {
                    Relationships[personInteractingWith] -= relationshipAdjustment;
                }

            }

            else
            {
                Relationships[personInteractingWith] = 50 + relationshipAdjustment;
            }




        }





        public static void BuildDecisionTree()
        {
            DataTable ID3Data = ExcelFileManager.ReadExcelFile("ID3Data.xlsx");
            decisionTree = new Tree();
            DataTable discreteData = decisionTree.DiscretiseData(ID3Data);

            decisionTree.Root = decisionTree.Learn(discreteData, "");





        }


        private void ConstructNeeds(bool _generateNeedsBar) //only generate needsbar display if player
        {
            Needs = new Dictionary<NeedNames, Need>();
            Needs.Add(NeedNames.Hunger, new Need(_name: NeedNames.Hunger, generateNeedBar: _generateNeedsBar));
            Needs.Add(NeedNames.Sleep, new Need(_name: NeedNames.Sleep, generateNeedBar: _generateNeedsBar));
            Needs.Add(NeedNames.Toilet, new Need(_name: NeedNames.Toilet, generateNeedBar: _generateNeedsBar));
            Needs.Add(NeedNames.Hygiene, new Need(_name: NeedNames.Hygiene, generateNeedBar: _generateNeedsBar));
            Needs.Add(NeedNames.Social, new Need(_name: NeedNames.Social, generateNeedBar: _generateNeedsBar));
            Needs.Add(NeedNames.Fun, new Need(_name: NeedNames.Fun, generateNeedBar: _generateNeedsBar));


        }






        public virtual void Update(GameTime gameTime, GraphicsDevice graphicsDevice, Matrix projection, Matrix view, Game1 game)
        {
            //currentHouse = House.getHouseContainingPoint(position);
            
            DepleteNeeds(gameTime);
            goapStateMachine.Tick(gameTime);

            


            if (actionState == PeopleActionStates.idle && game.IsActive)
            {
                //Random random = new Random();
                //goal = new Vector3(random.Next(-1000, 1000), yValue, random.Next(-1000, 1000)); //temporary for npc
                //actionState = PeopleActionStates.moving;

                //BuildPath();
                //MovePerson(gameTime);




            }


            else if (actionState == PeopleActionStates.beginMoving)
            {

                //set into beginMoving state and goal assigned in GoTo class in GOAPPerson

                actionState = PeopleActionStates.moving;
                BuildPath();
                MovePerson(gameTime);

            }





            else if (actionState == PeopleActionStates.moving)
            {
                MovePerson(gameTime);
            }



            updateTransformationMatrix(); //updates transformation matrix with transformations occured in current frame 

            avatar.worldMatrix *= transformationMatrix; //transforms world matrix by transformations specified in transformation matrix
            viewVector = avatar.worldMatrix.Forward; //adjusts view vector of model
            position = avatar.worldMatrix.Translation; //adjusts position variable to position in model's world matrix

        }





        public void BuildPath()
        {
            reachedGoal = false;
            pathPoints.Clear();

            House goalHouse = House.getHouseContainingPoint(goal);
            House currentHouse = House.getHouseContainingPoint(position);
            bool currentlyInHouse = currentHouse == null ? false : true;
            bool goalInHouse = goalHouse == null ? false : true;

            bool currentlyInBuilding = currentBuilding == null ? false : true;
            bool goalInBuilding = goalBuilding == null ? false : true;

            bool currentlyOutside = !(currentlyInHouse ||  currentlyInBuilding);
            bool goalOutside = !(goalInHouse || goalInBuilding);

                        

            if (!currentlyOutside)
            {
                if (currentlyInHouse)
                {
                    if (goalInHouse && goalHouse.id == currentHouse.id)//same house, doesn't need to go to frint door
                    {
                        mesh = House.navMesh;
                        pathFinder = new Path(mesh);

                        Vector3 localHouseStart = (Matrix.CreateTranslation(position) * Matrix.Invert(currentHouse.houseToTownTransformation)).Translation;
                        Vector3 localHouseGoal = (Matrix.CreateTranslation(goal) * Matrix.Invert(currentHouse.houseToTownTransformation)).Translation;

                        List<Vector3> localHousePoints = new List<Vector3>();

                        pathFinder.FindPath(localHouseStart, localHouseGoal, ref localHousePoints);


                        foreach (Vector3 point in localHousePoints)
                        {
                            pathPoints.Add((Matrix.CreateTranslation(point) * currentHouse.houseToTownTransformation).Translation);

                        }

                    }

                    else //need to find path to front door first
                    {
                        List<Vector3> pathToOutside = new List<Vector3>();
                        mesh = House.navMesh;
                        pathFinder = new Path(mesh);
                        Vector3 localHouseStart = (Matrix.CreateTranslation(position) * Matrix.Invert(currentHouse.houseToTownTransformation)).Translation;

                        pathFinder.FindPath(localHouseStart, Vector3.Zero, ref pathToOutside);

                        foreach (Vector3 point in pathToOutside)
                        {
                            pathPoints.Add((Matrix.CreateTranslation(point) * currentHouse.houseToTownTransformation).Translation);
                        }

                        if (goalInHouse && goalHouse.id != currentHouse.id) //different house as goal
                        {
                            District startDistrict = town.districts.Find(x => x.streets.Contains(currentHouse.street));

                            pathPoints.AddRange(startDistrict.FindStreetPathPoints(currentHouse, goalHouse)); //houses in same district

                            List<Vector3> houseToGoal = new List<Vector3>();
                            mesh = House.navMesh;
                            pathFinder = new Path(mesh);
                            Vector3 localHouseGoal = (Matrix.CreateTranslation(goal) * Matrix.Invert(goalHouse.houseToTownTransformation)).Translation;

                            pathFinder.FindPath(Vector3.Zero, localHouseGoal, ref houseToGoal);

                            foreach (Vector3 point in houseToGoal)
                            {
                                pathPoints.Add((Matrix.CreateTranslation(point) * goalHouse.houseToTownTransformation).Translation);

                            }
                        }

                        else if (goalInBuilding)
                        {

                            District startDistrict = town.districts.Find(x => x.streets.Contains(currentHouse.street));
                            pathPoints.AddRange(startDistrict.StreetPathToDistrictOrigin(currentHouse));

                            District endDistrict = town.districts.Find(x => x.streets.Contains(goalBuilding.street));

                            List<Vector3> goalToOrigin = endDistrict.StreetPathToDistrictOrigin(goalBuilding);

                            goalToOrigin.Reverse();

                            pathPoints.AddRange(goalToOrigin);

                            //need to add something to get inside building , maybe use vector perpendicualr to street of building

                            pathPoints.Add(pathPoints.Last() + (goalBuilding.side? 1 : -1)*Building.StreetOffset * goalBuilding.street.FindPerpendicularVector());


                        }
                        else //goal in open space
                        {

                            List<Vector3> outsideToGoal = new List<Vector3>();

                            mesh = Town.Town.navMesh;
                            pathFinder = new Path(mesh);
                            pathFinder.FindPath(currentHouse.TownLocation, goal, ref outsideToGoal);

                            pathPoints.AddRange(outsideToGoal);

                        }


                    }


                }


                else if (currentlyInBuilding)
                {
                    pathPoints.Add(currentBuilding.TownLocation);

                    if (goalOutside)
                    {
                        List<Vector3> outsideToGoal = new List<Vector3>();

                        mesh = Town.Town.navMesh;
                        pathFinder = new Path(mesh);
                        pathFinder.FindPath(currentBuilding.TownLocation, goal, ref outsideToGoal);

                        pathPoints.AddRange(outsideToGoal);
                    }

                    else if (goalInBuilding)
                    {
                        District startDistrict = town.districts.Find(x => x.streets.Contains(currentBuilding.street));

                        pathPoints.AddRange(startDistrict.FindStreetPathPoints(currentBuilding, goalBuilding)); //buildings in same district

                        //need to add something to get inside building , maybe use vector perpendicualr to street of building
                        pathPoints.Add(pathPoints.Last() + (goalBuilding.side ? 1 : -1) * Building.StreetOffset * goalBuilding.street.FindPerpendicularVector());




                    }

                    else if (goalInHouse)
                    {
                        District startDistrict = town.districts.Find(x => x.streets.Contains(currentBuilding.street));
                        pathPoints.AddRange(startDistrict.StreetPathToDistrictOrigin(currentBuilding));
                        District endDistrict = town.districts.Find(x => x.streets.Contains(goalHouse.street));
                        List<Vector3> goalToOrigin = endDistrict.StreetPathToDistrictOrigin(goalHouse);
                        goalToOrigin.Reverse();
                        pathPoints.AddRange(goalToOrigin);

                        List<Vector3> houseToGoal = new List<Vector3>();
                        mesh = House.navMesh;
                        pathFinder = new Path(mesh);
                        Vector3 localHouseGoal = (Matrix.CreateTranslation(goal) * Matrix.Invert(goalHouse.houseToTownTransformation)).Translation;

                        pathFinder.FindPath(Vector3.Zero, localHouseGoal, ref houseToGoal);

                        foreach (Vector3 point in houseToGoal)
                        {
                            pathPoints.Add((Matrix.CreateTranslation(point) * goalHouse.houseToTownTransformation).Translation);

                        }



                    }



                }

                

            }
            else //currently outside
            {
                if (goalOutside)
                {
                    mesh = Town.Town.navMesh;
                    pathFinder = new Path(mesh);
                    pathFinder.FindPath(position, goal, ref pathPoints);
                }
                else if (goalInBuilding)
                {
                    mesh = Town.Town.navMesh;
                    pathFinder = new Path(mesh);
                    pathFinder.FindPath(position, goalBuilding.TownLocation, ref pathPoints);

                    //need to add something to get inside building , maybe use vector perpendicualr to street of building
                    pathPoints.Add(pathPoints.Last() + (goalBuilding.side ? 1 : -1) * Building.StreetOffset * goalBuilding.street.FindPerpendicularVector());



                }
                else if (goalInHouse)
                {
                    List<Vector3> pathToHouse = new List<Vector3>();
                
                    mesh = Town.Town.navMesh;
                    pathFinder = new Path(mesh);
                    pathFinder.FindPath(position, goalHouse.TownLocation, ref pathToHouse);
                    pathPoints.AddRange(pathToHouse);

                    List<Vector3> houseToGoal = new List<Vector3>();
                    mesh = House.navMesh;
                    pathFinder = new Path(mesh);
                    Vector3 localHouseGoal = (Matrix.CreateTranslation(goal) * Matrix.Invert(goalHouse.houseToTownTransformation)).Translation;

                    pathFinder.FindPath(Vector3.Zero, localHouseGoal, ref houseToGoal);

                    foreach (Vector3 point in houseToGoal)
                    {
                        pathPoints.Add((Matrix.CreateTranslation(point) * goalHouse.houseToTownTransformation).Translation);

                    }

                }
            }


            

        }


    











        //public void BuildPath()
        //{

        //    reachedGoal = false;

        //    House goalHouse = House.getHouseContainingPoint(goal);
        //    House currentHouse = House.getHouseContainingPoint(position);
        //    pathPoints.Clear();


        //    bool currentlyOutside = currentHouse == null ? true : false;
        //    bool goalOutside = goalHouse == null ? true : false;

        //    if (currentlyOutside && goalOutside)
        //    {
        //        mesh = Town.Town.navMesh;
        //        pathFinder = new Path(mesh);
        //        pathFinder.FindPath(position, goal, ref pathPoints);
                

        //    }

        //    else if (currentlyOutside && !goalOutside)
        //    {
        //        List<Vector3> pathToHouse = new List<Vector3>();
                
        //        mesh = Town.Town.navMesh;
        //        pathFinder = new Path(mesh);
        //        pathFinder.FindPath(position, goalHouse.TownLocation, ref pathToHouse);
        //        pathPoints.AddRange(pathToHouse);

        //        List<Vector3> houseToGoal = new List<Vector3>();
        //        mesh = House.navMesh;
        //        pathFinder = new Path(mesh);
        //        Vector3 localHouseGoal = (Matrix.CreateTranslation(goal) * Matrix.Invert(goalHouse.houseToTownTransformation)).Translation;

        //        pathFinder.FindPath(Vector3.Zero, localHouseGoal, ref houseToGoal);

        //        foreach (Vector3 point in houseToGoal)
        //        {
        //            pathPoints.Add((Matrix.CreateTranslation(point) * goalHouse.houseToTownTransformation).Translation);

        //        }


                

        //    }

        //    else if (!currentlyOutside && goalOutside)
        //    {
        //        List<Vector3> pathToOutside = new List<Vector3>();
        //        mesh = House.navMesh;
        //        pathFinder = new Path(mesh);
        //        Vector3 localHouseStart = (Matrix.CreateTranslation(position) * Matrix.Invert(currentHouse.houseToTownTransformation)).Translation;

        //        pathFinder.FindPath(localHouseStart, Vector3.Zero, ref pathToOutside);

        //        foreach (Vector3 point in pathToOutside)
        //        {
        //            pathPoints.Add((Matrix.CreateTranslation(point) * currentHouse.houseToTownTransformation).Translation);
        //        }

        //        List<Vector3> outsideToGoal = new List<Vector3>();

        //        mesh = Town.Town.navMesh;
        //        pathFinder = new Path(mesh);
        //        pathFinder.FindPath(currentHouse.TownLocation, goal, ref outsideToGoal);

        //        pathPoints.AddRange(outsideToGoal);

                 
        //    }

        //    else if (!currentlyOutside && !goalOutside)
        //    {
        //        if (currentHouse.id == goalHouse.id)
        //        {
        //            Console.WriteLine((new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().ToString());

        //            mesh = House.navMesh;
        //            pathFinder = new Path(mesh);

        //            Vector3 localHouseStart = (Matrix.CreateTranslation(position) * Matrix.Invert(currentHouse.houseToTownTransformation)).Translation;
        //            Vector3 localHouseGoal = (Matrix.CreateTranslation(goal) * Matrix.Invert(currentHouse.houseToTownTransformation)).Translation;

        //            List<Vector3> localHousePoints = new List<Vector3>();

        //            pathFinder.FindPath(localHouseStart, localHouseGoal, ref localHousePoints);

        //            Console.WriteLine("localHousePoints;" + localHousePoints.Count);
        //            Console.WriteLine("pp:" + pathPoints.Count);
        //            foreach (Vector3 point in localHousePoints)
        //            {
        //                pathPoints.Add((Matrix.CreateTranslation(point) * currentHouse.houseToTownTransformation).Translation);
        //                Console.WriteLine((Matrix.CreateTranslation(point) * currentHouse.houseToTownTransformation).Translation.ToString());
        //                Console.WriteLine("pp:" + pathPoints.Count);
        //            }

        //            Console.WriteLine("Path length:" + pathPoints.Count);

        //        }

        //        else if (currentHouse.id != goalHouse.id)
        //        {

        //            List<Vector3> pathToOutside = new List<Vector3>();
        //            mesh = House.navMesh;
        //            pathFinder = new Path(mesh);
        //            Vector3 localHouseStart = (Matrix.CreateTranslation(position) * Matrix.Invert(currentHouse.houseToTownTransformation)).Translation;

        //            pathFinder.FindPath(localHouseStart, Vector3.Zero, ref pathToOutside);

        //            foreach (Vector3 point in pathToOutside)
        //            {
        //                pathPoints.Add((Matrix.CreateTranslation(point) * currentHouse.houseToTownTransformation).Translation);
        //            }






        //            /* using roads here 
        //            List<Vector3> outsideToGoalHouse = new List<Vector3>();

        //            mesh = Town.Town.navMesh;
        //            pathFinder = new Path(mesh);
        //            pathFinder.FindPath(currentHouse.TownLocation, goalHouse.TownLocation, ref outsideToGoalHouse);

        //            pathPoints.AddRange(outsideToGoalHouse);
        //            */

        //            //for now, assumes both in residential district

        //            //NEED TO CHANGE WHEN IT DECIDES ITS INSIDE THE CURRENT HOUSE AS IF JUST GO TO FRONT DOOR, DOESN'T THINK ITS IN THE HOUSE SO IT WONT GO TO ANOTHER HOUSE VIA THE ROADS.

        //            District startDistrict = town.districts.Find(x => x.streets.Contains(currentHouse.street));

        //            pathPoints.AddRange(startDistrict.FindStreetPathPoints(currentHouse, goalHouse));








        //            List<Vector3> houseToGoal = new List<Vector3>();
        //            mesh = House.navMesh;
        //            pathFinder = new Path(mesh);
        //            Vector3 localHouseGoal = (Matrix.CreateTranslation(goal) * Matrix.Invert(goalHouse.houseToTownTransformation)).Translation;

        //            pathFinder.FindPath(Vector3.Zero, localHouseGoal, ref houseToGoal);

        //            foreach (Vector3 point in houseToGoal)
        //            {
        //                pathPoints.Add((Matrix.CreateTranslation(point) * goalHouse.houseToTownTransformation).Translation);

        //            }

                   



        //        }





        //    }





        //}




        public virtual void MovePerson( GameTime gameTime)
        {
           
            if (pathPoints.Count == 0)
            {
               
                motionState = PeopleMotionStates.idle;
                actionState = PeopleActionStates.idle;
                //currentHouse = House.getHouseContainingPoint(position);
                currentHouse = goalHouse;
                currentBuilding = goalBuilding;

                reachedGoal = true;
                return;
            }

            if (motionState == PeopleMotionStates.idle && pathPoints.Count > 0)
            { 
                Vector3 nextTarget = pathPoints[0];
                targetVector = getTargetVector(gameTime, nextTarget);




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
                    currentHouse = goalHouse;
                    currentBuilding = goalBuilding;
                    reachedGoal = true;
                    return;
                }

                Vector3 currentTarget = pathPoints[0];
                
                targetVector = getTargetVector(gameTime, currentTarget);
                
                rotationMatrix = Matrix.Identity;
                getNewTranslationMatrix(gameTime);



            }
            
           

        }


        public void updateTransformationMatrix()
        {
            transformationMatrix = Matrix.CreateTranslation(-position)*rotationMatrix*translationMatrix; //updates transformation matrix by first moving model to orgin to allow rotation to be applied properly, then moving model back to current position
           
        }


        public void getNewRotationMatrix(GameTime gameTime)
        {
            
            
            float angleLeftToRotate = (float)Math.Acos( (Vector3.Dot( viewVector, targetVector ) ) / ( targetVector.Length() * viewVector.Length() ) ); //calculates angle between viewVector and target vector i.e. angle through which model must rotate to face towards target
            float angleToRotateThisFrame = (float)(angularVelocity * gameTime.ElapsedGameTime.TotalSeconds); //determines the maximum angle that can be rotated through this frame
            
            if (angleLeftToRotate>= Math.PI)
            {
                angleLeftToRotate = -angleLeftToRotate;

            }


           
            if (angleToRotateThisFrame> angleLeftToRotate) //if the maximum angle that can be rotated through for this frame is greater than the angle that needs to be rotated through, the angle to rotate through this frame is changed to be the angle left to rotate to prevent model over-rotating
            {   
                //if (angleLeftToRotate > Math.PI) //if angle calculated is reflex, it returns the non-reflex angle e.g. 270 degrees => -90 degrees
                //{
                //    angleLeftToRotate = (float)(Math.PI - angleLeftToRotate);
                //}
                
                angleToRotateThisFrame = angleLeftToRotate;
                motionState = PeopleMotionStates.moving;
            }


            
            rotationMatrix = Matrix.CreateRotationY(angleToRotateThisFrame); //constructs rotation matrix applied to transformation matrix
            
            //targetVector.Normalize();


            //Console.WriteLine($"{targetVector.ToString()} , {targetPosition.ToString()} , {angleLeftToRotate}");
            


            //if (Math.Round(viewVector.X, 2) == Math.Round(targetVector.X, 2) && Math.Round(viewVector.Z, 2) == Math.Round(targetVector.Z, 2)) //finds if vectors are approximately alligned, must use slightly rounded values due to cos function rounding and making the 8th, 9th, 10th etc decimal places slightly different so vectors were never exactly equal
            //{
            //    rotationMatrix = Matrix.Identity; //resets rotation matrix to identity so does not continue to rotate oncereached target when transformation matrix updated
            //    motionState = PeopleMotionStates.moving; //changes state to moving as finished rotating to target
            //}

            //Thread.Sleep(1000);
        }



        public Vector3 getTargetVector(GameTime gameTime, Vector3 targetPosition)
        {
            if (targetPosition.Y == -100) //returned when point not on plane selected
            {
                return targetVector; //continues to use previous target vector
            }
            else
            {

                targetVector = targetPosition - position; //recalculate target vector from current position
                float maxDistanceCanCover = (float)(velocity * gameTime.ElapsedGameTime.TotalSeconds); //calculates maximum distance that can be travelled between updates of frame using velocity
                if (targetVector.Length() < maxDistanceCanCover) // prevents problem of avatar overshooting target position and then recalculating target vector and overshooting again causing it to oscillate about its target point.
                                                                 // if the object can cover the entire distance to target point in next frame, then it should 
                {
                    pathPoints.RemoveAt(0);


                    
                    //motionState = PeopleMotionStates.idle; //changes motion state back to idle as object completes its movement 
                    //actionState = PeopleActionStates.idle; //changes action state back to idle as object completes its movement
                    return targetVector; //returns the target vector of full length as all of vector can be moved through next frame

                }
                return Vector3.Normalize(targetVector); //returns normalised target vector, so this can be scaled by distance covered in next frame

            }
              
        }


        public void getNewTranslationMatrix(GameTime gameTime)
        {
            Vector3 newPos= position + (float)(velocity * gameTime.ElapsedGameTime.TotalSeconds) * targetVector;  //calculates new position for movement this frame

            translationMatrix = Matrix.CreateTranslation(newPos); //creates translation matrix for new position


        }



    }
}
