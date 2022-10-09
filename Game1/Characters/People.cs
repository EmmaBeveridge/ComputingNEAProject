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
        typingChat
    }



    public class People
    {
        protected MouseState prevMouseState = new MouseState();

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

        public PeopleMotionStates motionState;
        public PeopleActionStates actionState;

        protected List<Vector3> pathPoints;
        protected Mesh mesh;
        protected Path pathFinder;
        public Vector3 goal;

        public House currentHouse;
        protected House goalHouse;
        protected Item selectedItem;
        protected People selectedPerson;
        public Town.Town town;
        protected Game1 game;


        //protected Agent agent;
        public GOAPPerson goapPerson;
        public StateMachine<GOAPPerson.GOAPPersonState> goapStateMachine;
        public bool reachedGoal = false;

        public BoundingBox boundingBox { get { return avatar.UpdateBoundingBox(); } }





        public People(Model _model, Vector3 _position, Mesh argMesh, Town.Town argTown, Game1 argGame)
        {
            position = _position;
            avatar = new Avatar (_model, _position);
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

            goapPerson = new GOAPPerson(this);
            goapStateMachine = goapPerson.BuildAI();
            
            

        }



       



        public virtual void Update(GameTime gameTime, GraphicsDevice graphicsDevice, Matrix projection, Matrix view, Game1 game)
        {
            //currentHouse = House.getHouseContainingPoint(position);
            
            goapStateMachine.Tick(gameTime);


            if (actionState==PeopleActionStates.idle && game.IsActive) 
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

            House goalHouse = House.getHouseContainingPoint(goal);
            House currentHouse = House.getHouseContainingPoint(position);
            pathPoints.Clear();


            bool currentlyOutside = currentHouse == null ? true : false;
            bool goalOutside = goalHouse == null ? true : false;

            if (currentlyOutside && goalOutside)
            {
                mesh = Town.Town.navMesh;
                pathFinder = new Path(mesh);
                pathFinder.FindPath(position, goal, ref pathPoints);
                

            }

            else if (currentlyOutside && !goalOutside)
            {
                List<Vector3> pathToHouse = new List<Vector3>();
                
                mesh = Town.Town.navMesh;
                pathFinder = new Path(mesh);
                pathFinder.FindPath(position, goalHouse.townLocation, ref pathToHouse);
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

            else if (!currentlyOutside && goalOutside)
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

                List<Vector3> outsideToGoal = new List<Vector3>();

                mesh = Town.Town.navMesh;
                pathFinder = new Path(mesh);
                pathFinder.FindPath(currentHouse.townLocation, goal, ref outsideToGoal);

                pathPoints.AddRange(outsideToGoal);

                 
            }

            else if (!currentlyOutside && !goalOutside)
            {
                if (currentHouse.id == goalHouse.id)
                {
                    Console.WriteLine((new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().ToString());

                    mesh = House.navMesh;
                    pathFinder = new Path(mesh);

                    Vector3 localHouseStart = (Matrix.CreateTranslation(position) * Matrix.Invert(currentHouse.houseToTownTransformation)).Translation;
                    Vector3 localHouseGoal = (Matrix.CreateTranslation(goal) * Matrix.Invert(currentHouse.houseToTownTransformation)).Translation;

                    List<Vector3> localHousePoints = new List<Vector3>();

                    pathFinder.FindPath(localHouseStart, localHouseGoal, ref localHousePoints);

                    Console.WriteLine("localHousePoints;" + localHousePoints.Count);
                    Console.WriteLine("pp:" + pathPoints.Count);
                    foreach (Vector3 point in localHousePoints)
                    {
                        pathPoints.Add((Matrix.CreateTranslation(point) * currentHouse.houseToTownTransformation).Translation);
                        Console.WriteLine((Matrix.CreateTranslation(point) * currentHouse.houseToTownTransformation).Translation.ToString());
                        Console.WriteLine("pp:" + pathPoints.Count);
                    }

                    Console.WriteLine("Path length:" + pathPoints.Count);

                }

                else if (currentHouse.id != goalHouse.id)
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

                    List<Vector3> outsideToGoalHouse = new List<Vector3>();

                    mesh = Town.Town.navMesh;
                    pathFinder = new Path(mesh);
                    pathFinder.FindPath(currentHouse.townLocation, goalHouse.townLocation, ref outsideToGoalHouse);

                    pathPoints.AddRange(outsideToGoalHouse);

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




        public virtual void MovePerson( GameTime gameTime)
        {
           
            if (pathPoints.Count == 0)
            {
               
                motionState = PeopleMotionStates.idle;
                actionState = PeopleActionStates.idle;
                //currentHouse = House.getHouseContainingPoint(position);
                currentHouse = goalHouse;

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


                    motionState = PeopleMotionStates.idle; //changes motion state back to idle as object completes its movement 
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
