using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game1.GOAP
{

    public class GOAPPerson
    {
        public People person;
        public GOAPPersonState goapPersonState;
        public StateMachine<GOAPPersonState> stateMachine;
        public const string LowHunger = "lowHunger";
        public  const string LowSleep = "lowSleep";
        public const string LowToilet = "lowToilet";
        public const string LowHygiene = "lowHygiene";
        public const string LowSocial = "lowSocial";
        public const string LowFun = "lowFun";


        public GOAPPerson(People argPerson) { person = argPerson;}

        public class GOAPPersonState:Agent
        {
            //public PersonState personState = new PersonState();
            //public float distanceToNextLocation;


            public Dictionary<NeedNames, Need> Needs;
            public Vector3 destinationLocation;
            public Vector3 currentLocation;
            public Stack<GOAPAction> actionPlan;
            public ActionPlanner planner;
            

            public GOAPPersonState(Dictionary<NeedNames, Need> _needs)
            {
                Needs = _needs;

            }




            public override WorldState GetWorldState()
            {
                var worldState = this.planner.CreateWorldState();

                //worldState.Set(LowHunger, Needs[NeedNames.Hunger].Level == NeedLevel.Low);
                worldState.Set(LowSleep, Needs[NeedNames.Sleep].Level == NeedLevel.Low);
                worldState.Set(LowToilet, Needs[NeedNames.Toilet].Level == NeedLevel.Low);
                //worldState.Set(LowHygiene, Needs[NeedNames.Hygiene].Level == NeedLevel.Low);
                //worldState.Set(LowFun, Needs[NeedNames.Fun].Level == NeedLevel.Low);
                //worldState.Set(LowSocial, Needs[NeedNames.Social].Level == NeedLevel.Low);




                //worldState.Set(IsHungry, this.personState.Hunger <= PersonState.HUNGER_THRESHOLD);
                //worldState.Set(IsTired, this.personState.Sleep <= PersonState.SLEEP_THRESHOLD);
                //worldState.Set(NeedsToilet, this.personState.Toilet <= PersonState.TOILET_THRESHOLD);

                return worldState;
            }

            public override WorldState GetGoalState() 
            {//will change later to use more complex way of selecting next goal state i.e next need to fulfil
                var goalState = this.planner.CreateWorldState();


                //do you want to use decision tree or ID3 - need to get data for ID3 but could generate prpgramatically like write a quick python prgram using decision tree in teams doc - outcome being if it should be added to random pool or not?














                //if(Needs[NeedNames.Toilet].Level == NeedLevel.Low)
                //{
                //    goalState.Set(LowToilet, false);
                //}
                //else if (Needs[NeedNames.Sleep].Level == NeedLevel.Low)
                //{
                //    goalState.Set(LowSleep, false);

                //}
                



                //if (this.personState.Hunger <= PersonState.HUNGER_THRESHOLD)
                //{
                //    goalState.Set(IsHungry, false);
                //}
                //else if (this.personState.Sleep <= PersonState.SLEEP_THRESHOLD)
                //{
                //    goalState.Set(IsTired, false);
                //}
                //else if (this.personState.Toilet <= PersonState.TOILET_THRESHOLD)
                //{
                //    goalState.Set(NeedsToilet, false);
                //}

                return goalState;

            }

        }

        public StateMachine<GOAPPersonState> BuildAI()
        {
            goapPersonState = new GOAPPersonState(person.Needs);
            goapPersonState.planner = new ActionPlanner(person);

            goapPersonState.planner.AddRangeAction(person.town.GOAPActions);

            stateMachine = new StateMachine<GOAPPersonState>(goapPersonState, new Idle());
            stateMachine.AddState(new GoTo());
            stateMachine.AddState(new PerformAction());
            stateMachine.AddState(new Wait());
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
                person = this.Context.planner.person;

                if (action.doingAction.FirstOrDefault() != person)
                {
                    location = item.room.townLocation;
                    

                }

                else
                {
                    location = item.townLocation;
                }

                
                
               
                this.Context.destinationLocation = location;
                
               
                
                person.actionState = PeopleActionStates.beginMoving;
                person.goal = location;


            }

            public override void Update(GameTime gameTime)
            {
                this.Context.currentLocation = person.position;

                if (person.reachedGoal)
                {
                    transitionOnNextTick = true;

                    if (this.Context.actionPlan.Peek().doingAction.FirstOrDefault() != person)
                    {
                        this.Machine.SetNextState<Wait>();
                    }

                    else //not tested but think putting else here is right??
                    {
                        this.Machine.SetNextState<PerformAction>();
                    }
                }


            }

            public override void End()
            {

                person.currentHouse = item.house;

            }






        }


        public class Wait : State<GOAPPersonState>
        {
            GOAPAction action;
            public Wait() { }

            public override void Begin()
            {
                transitionOnNextTick = false;
                action = this.Context.actionPlan.Peek();
            }

            public override void Update(GameTime gameTime)
            {
                
                if (action.item.IsAvailable)
                {
                    transitionOnNextTick = true;
                    
                    this.Machine.SetNextState<GoTo>();

                }


            }
        }





        public class PerformAction : State<GOAPPersonState>
        {

            People person;
            GameTime timer;
            Action<GameTime, Dictionary<NeedNames, Need>> currentActionMethod;



            GOAPAction action;
            Item actionItem;
            public PerformAction()
            {
                

            }

            public override void Begin()
            {
                transitionOnNextTick = false;

                action = this.Context.actionPlan.Peek();
                actionItem = action.item;
                person = this.Context.planner.person;

                currentActionMethod = action.Action.ActionMethod;
                action.Action.BeginAction();

                //currentAction = actionItem.BeginAction(action.Name, person);



            }



            public override void Update(GameTime gameTime)
            {

                if (action.Action.ActionComplete)
                {
                    action.Action.ActionComplete = false;

                    transitionOnNextTick = true;
                    this.Context.actionPlan.Pop();

                    if (this.Context.actionPlan.Count > 0)
                    {
                        this.Machine.SetNextState<GoTo>();

                    }
                    else
                    {
                        this.Machine.SetNextState<Idle>();

                    }


                }
                else
                {
                    currentActionMethod(gameTime, person.Needs);


                }







                //if (actionItem.actionComplete)
                //{
                //    actionItem.actionComplete = false;

                //    transitionOnNextTick = true;
                //    this.Context.actionPlan.Pop();

                //    if (this.Context.actionPlan.Count > 0)
                //    {
                //        this.Machine.SetNextState<GoTo>();

                //    }
                //    else
                //    {
                //        this.Machine.SetNextState<Idle>(); 

                //    }


                //}
                //else
                //{
                //    currentAction(gameTime, person.Needs);


                //}






            }

            public override void End()
            {
                action.doingAction.Dequeue();
            }






        }







    }

   


    //public class PersonState
    //{
    //    public const int HUNGER_THRESHOLD = 20;
    //    public const int SLEEP_THRESHOLD = 20;
    //    public const int TOILET_THRESHOLD = 20;

    //    public int Hunger = 90;
    //    public int Sleep = 0;
    //    public int Toilet = 10;

        





    //}





}
