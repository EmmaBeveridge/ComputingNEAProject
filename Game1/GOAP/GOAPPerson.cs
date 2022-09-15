using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Game1.GOAP
{

    public class GOAPPerson
    {
        public People person;
        public GOAPPersonState goapPersonState;
        public StateMachine<GOAPPersonState> stateMachine;
        public const string IsHungry = "hungry";
        public  const string IsTired = "tired";
        public const string NeedsToilet = "needsToilet";


        public GOAPPerson(People argPerson) { person = argPerson;}

        public class GOAPPersonState:Agent
        {
            public PersonState personState = new PersonState();
            //public float distanceToNextLocation;
            public Vector3 destinationLocation;
            public Vector3 currentLocation;
            public Stack<GOAPAction> actionPlan;
            public ActionPlanner planner;
            

            public override WorldState GetWorldState()
            {
                var worldState = this.planner.CreateWorldState();
                worldState.Set(IsHungry, this.personState.Hunger <= PersonState.HUNGER_THRESHOLD);
                worldState.Set(IsTired, this.personState.Sleep <= PersonState.SLEEP_THRESHOLD);
                worldState.Set(NeedsToilet, this.personState.Toilet <= PersonState.TOILET_THRESHOLD);

                return worldState;
            }

            public override WorldState GetGoalState() 
            {//will change later to use more complex way of selecting next goal state i.e next need to fulfil
                var goalState = this.planner.CreateWorldState();

                if (this.personState.Hunger <= PersonState.HUNGER_THRESHOLD)
                {
                    goalState.Set(IsHungry, false);
                }
                else if (this.personState.Sleep <= PersonState.SLEEP_THRESHOLD)
                {
                    goalState.Set(IsTired, false);
                }
                else if (this.personState.Toilet <= PersonState.TOILET_THRESHOLD)
                {
                    goalState.Set(NeedsToilet, false);
                }

                return goalState;

            }

        }

        public StateMachine<GOAPPersonState> BuildAI()
        {
            goapPersonState = new GOAPPersonState();
            goapPersonState.planner = new ActionPlanner(person);

            goapPersonState.planner.AddRangeAction(person.town.GOAPActions);

            stateMachine = new StateMachine<GOAPPersonState>(goapPersonState, new Idle());
            stateMachine.AddState(new GoTo());
            stateMachine.AddState(new PerformAction());
            return stateMachine;

        }


        public void PushNewAction(GOAPAction action)
        {
            if (goapPersonState.actionPlan.Count == 0)
            {
                goapPersonState.actionPlan.Push(action);

            }
            else
            {   GOAPAction current = goapPersonState.actionPlan.Pop();
                goapPersonState.actionPlan.Push(action);
                goapPersonState.actionPlan.Push(current);

            }
           

            
        }




        public class Idle: State<GOAPPersonState>
        {
            
            public Idle()
            {
                
            }
            public override void Begin()
            {
                transitionOnNextTick = false;

                Console.WriteLine("Beginning planning");
                this.Context.actionPlan = this.Context.planner.Plan(this.Context.GetWorldState(), this.Context.GetGoalState());

                //if (this.Context.actionPlan != null && this.Context.actionPlan.Count>0)
                //{
                //    this.Machine.ChangeState<GoTo>();
                //    person.actionState = PeopleActionStates.moving;
                   
                //}

            }

            public override void Update(GameTime gameTime)
            {
                if (this.Context.actionPlan != null && this.Context.actionPlan.Count > 0)
                {
                    transitionOnNextTick = true;
                    this.Machine.SetNextState<GoTo>();

                }
            }

        }

        public class GoTo: State<GOAPPersonState>
        {

            Vector3 location;
            People person;
            Item item; 


            public GoTo()
            {
                
            }



            public override void Begin()
            {
                transitionOnNextTick = false;

                var action = this.Context.actionPlan.Peek();

                item = action.item;


                location = item.townLocation;
                this.Context.destinationLocation = location;
                
                person = this.Context.planner.person;
                
                person.actionState = PeopleActionStates.beginMoving;
                person.goal = location;


            }

            public override void Update(GameTime gameTime)
            {
                this.Context.currentLocation = person.position;

                if (person.reachedGoal)
                {
                    transitionOnNextTick = true;
                    this.Machine.SetNextState<PerformAction>();

                }


            }

            public override void End()
            {

                person.currentHouse = item.house;

            }






        }

        public class PerformAction : State<GOAPPersonState>
        {

            People person;
            GameTime timer;
            Action<GameTime> currentAction;
            Item actionItem;
            public PerformAction()
            {
                

            }

            public override void Begin()
            {
                transitionOnNextTick = false;

                var action = this.Context.actionPlan.Peek();
                actionItem = action.item;

                
                
                person = this.Context.planner.person;

                currentAction = actionItem.BeginAction(action.Name, person);



            }



            public override void Update(GameTime gameTime)
            {

                if (actionItem.actionComplete)
                {
                    
                    //return;
                    transitionOnNextTick = true;
                    this.Context.actionPlan.Pop();

                    if (this.Context.actionPlan.Count > 0)
                    {
                        this.Machine.SetNextState<GoTo>();
                        
                    }
                    else
                    {
                        this.Machine.SetNextState<Idle>(); //NEED TO CHANGE SO ACTION COMPLETES WHEN COMPLETED 
                        
                    }
                   

                }
                else
                {
                    currentAction(gameTime);
               

                }
                




                
            }

           





        }







    }

   


    public class PersonState
    {
        public const int HUNGER_THRESHOLD = 20;
        public const int SLEEP_THRESHOLD = 20;
        public const int TOILET_THRESHOLD = 20;

        public int Hunger = 90;
        public int Sleep = 0;
        public int Toilet = 10;

        





    }





}
