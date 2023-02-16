using Game1.Town;
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



            public string GetConditionNameFromNeedName(NeedNames needName)
            {
                switch (needName)
                {
                    
                    case NeedNames.Hunger:
                        return LowHunger;
                        break;
                    case NeedNames.Sleep:
                        return LowSleep;
                        break;
                    case NeedNames.Toilet:
                        return LowToilet;
                        break;
                    case NeedNames.Social:
                        return LowSocial;
                        break;
                    case NeedNames.Hygiene:
                        return LowHygiene;
                        break;
                    case NeedNames.Fun:
                        return LowFun;
                        break;
                    default:
                        return null;
                        break;
                }
            }


            public WorldState GetWorldState(NeedNames needToFulfill)
            {
                //doesnt deal with chance of needToFulfill being null
                var worldState = this.planner.CreateWorldState();
                worldState.Set(GetConditionNameFromNeedName(needToFulfill), true);
                return worldState;

            }

            public override WorldState GetWorldState()
            {
                var worldState = this.planner.CreateWorldState();





                worldState.Set(LowHunger, Needs[NeedNames.Hunger].Level == NeedLevel.Low);
                worldState.Set(LowSleep, Needs[NeedNames.Sleep].Level == NeedLevel.Low);
                worldState.Set(LowToilet, Needs[NeedNames.Toilet].Level == NeedLevel.Low);
                worldState.Set(LowHygiene, Needs[NeedNames.Hygiene].Level == NeedLevel.Low);
                worldState.Set(LowFun, Needs[NeedNames.Fun].Level == NeedLevel.Low);
                worldState.Set(LowSocial, Needs[NeedNames.Social].Level == NeedLevel.Low);

                

                //worldState.Set(IsHungry, this.personState.Hunger <= PersonState.HUNGER_THRESHOLD);
                //worldState.Set(IsTired, this.personState.Sleep <= PersonState.SLEEP_THRESHOLD);
                //worldState.Set(NeedsToilet, this.personState.Toilet <= PersonState.TOILET_THRESHOLD);

                return worldState;
            }


            public override WorldState GetGoalState()
            {
                NeedNames tempRef = NeedNames.Null;



                return GetGoalState(ref tempRef);
            }


            public WorldState GetGoalStateForNeed(NeedNames needToFulfill)
            {
                var goalState = this.planner.CreateWorldState();


                switch (needToFulfill)
                {

                    case NeedNames.Sleep:
                        goalState.Set(LowSleep, false);

                        break;
                    case NeedNames.Toilet:
                        goalState.Set(LowToilet, false);
                        break;
                    case NeedNames.Hunger:
                        goalState.Set(LowHunger, false);
                        break;
                    case NeedNames.Hygiene:
                        goalState.Set(LowHygiene, false);
                        break;
                    case NeedNames.Fun:
                        goalState.Set(LowFun, false);
                        break;
                    case NeedNames.Social:
                        goalState.Set(LowSocial, false);
                        break;




                    default:
                        break;
                }

                return goalState;

            }

            public override WorldState GetGoalState(ref NeedNames needToFulfill) 
            {
                
                var goalState = this.planner.CreateWorldState();


                NeedNames need = People.decisionTree.GetResult(Needs);


                needToFulfill = need;


                switch (need)
                {

                    case NeedNames.Sleep:
                        goalState.Set(LowSleep, false);
                       
                        break;
                    case NeedNames.Toilet:
                        goalState.Set(LowToilet, false);
                        break;
                    case NeedNames.Hunger:
                        goalState.Set(LowHunger, false);
                        break;
                    case NeedNames.Hygiene:
                        goalState.Set(LowHygiene, false);
                        break;
                    case NeedNames.Fun:
                        goalState.Set(LowFun, false);
                        break;
                    case NeedNames.Social:
                        goalState.Set(LowSocial, false);
                        break;




                    default:
                        break;
                }

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
            stateMachine.AddState(new WaitForItem());
            stateMachine.AddState(new WaitForPerson());
            stateMachine.AddState(new LeaveBuilding());
            return stateMachine;

        }


        public void PushNewAction(GOAPAction action)
        {
            if (goapPersonState.actionPlan==null||goapPersonState.actionPlan.Count == 0)
            {
                goapPersonState.actionPlan.Push(action);

            }
            else
            {   GOAPAction current = goapPersonState.actionPlan.Pop();
                goapPersonState.actionPlan.Push(action);
                goapPersonState.actionPlan.Push(current);

            }

            if (action.GetType() == typeof(GOAPActionWithItem))
            {              
                GOAPWorld.ReserveItem(action, goapPersonState.planner); //hopefully fixes reassigning initiator
            }

           
            
        }




        public class Idle: State<GOAPPersonState>
        {
            People person;
            public Idle()
            {
                
            }
            public override void Begin()
            {
                transitionOnNextTick = false;

                person = this.Context.planner.person;

                Console.WriteLine("Beginning planning");

                NeedNames needToFulfill = NeedNames.Null;
                WorldState goalState = this.Context.GetGoalState(ref needToFulfill);

                this.Context.actionPlan = this.Context.planner.Plan(this.Context.GetWorldState(needToFulfill), goalState);


                if (this.Context.actionPlan == null)
                {
                    this.Context.actionPlan = new Stack<GOAPAction>();
                    this.Machine.SetNextState<Idle>();
                }



                else if (this.Context.actionPlan != null && this.Context.actionPlan.Any(x => x.GetType() == typeof(GOAPActionWithPerson)))
                {



                    GOAPAction personAction = this.Context.actionPlan.First(x => x.GetType() == typeof(GOAPActionWithPerson));



                    if (personAction.interactionPerson.goapPerson.goapPersonState == null)
                    {

                        NeedNames newNeedToFulfill = person.Needs.OrderBy(kvp => kvp.Value.CurrentNeed).First(kvp => kvp.Key != NeedNames.Social).Key;
                        goalState = this.Context.GetGoalStateForNeed(newNeedToFulfill); //fulfills non-social need until other person initialised

                        this.Context.actionPlan = this.Context.planner.Plan(this.Context.GetWorldState(newNeedToFulfill), goalState);


                    }
                    else { person.SendRSVP(personAction); };



                }
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
            Building building;

            public GoTo()
            {
                
            }



            public override void Begin()
            {
                transitionOnNextTick = false;

                var action = this.Context.actionPlan.Peek();                
                person = this.Context.planner.person;


                if (action.GetType() == typeof(GOAPActionWithItem))
                {
                    
                    item = action.item;

                    if (action.doingAction.FirstOrDefault() != person && action.doingAction.Count != 0)
                    {
                        location = item.room.townLocation;
                    

                    }

                    else
                    {
                        location = item.townLocation;
                    }

                }

                else if (action.GetType() == typeof(GOAPActionWithBuilding))
                {
                    building = action.building;
                    location = building.TownLocation;
                    person.goalBuilding = action.building;
                }

                

                else if (action.GetType() == typeof(GOAPActionWithPerson))
                {
                    //check if they chose to start interaction, i.e. should go to the person or stay still
                    
                    if (action.Action.initiator == person)
                    {
                        

                        Stack<GOAPAction> interactionPersonPlan = action.interactionPerson.goapPerson.goapPersonState.actionPlan;

                        if (interactionPersonPlan.Count != 0 && interactionPersonPlan.Peek().GetType() == typeof(GOAPActionWithBuilding))
                        {
                            location = interactionPersonPlan.Peek().building.TownLocation; //interaction person will have to leave building before conversation
                        }
                        else { location = action.interactionPerson.position; }

                        
                    }
                    else
                    {
                        location = person.position;
                    }





                }


                
                
               
                this.Context.destinationLocation = location;
                
               
                
                person.actionState = PeopleActionStates.beginMoving;
                person.goal = location;


            }

            public override void Update(GameTime gameTime)
            {
                this.Context.currentLocation = person.position;

                person.actionState = PeopleActionStates.moving; //resets action state in case interrupted by selecting item etc that changes state in Player.Update

                var action = this.Context.actionPlan.Peek();

                


                if (action.GetType() == typeof(GOAPActionWithItem))
                {


                    if (person.reachedGoal)
                    {
                        transitionOnNextTick = true;

                        if (person.goal != action.item.townLocation)
                        {
                            this.Machine.SetNextState<Idle>();
                        }

                        else
                        {
                            if (this.Context.actionPlan.Peek().doingAction.FirstOrDefault() != person && this.Context.actionPlan.Peek().doingAction.Count != 0)
                            {
                                this.Machine.SetNextState<WaitForItem>();
                            }

                            else //not tested but think putting else here is right??
                            {
                                this.Machine.SetNextState<PerformAction>();
                            }

                        }
                        
                    }

                }


                else if (action.GetType() == typeof(GOAPActionWithBuilding))
                {
                    if (person.reachedGoal)
                    {
                        transitionOnNextTick = true;
                        if (person.goal != action.building.TownLocation)
                        {
                            this.Machine.SetNextState<Idle>();
                        }
                        else
                        {
                            
                            person.goalBuilding = null;
                            this.Machine.SetNextState<PerformAction>();

                        }
                        
                    }
                }

                else if (action.GetType() == typeof(GOAPActionWithPerson))
                {
                    if (action.Action.initiator != person) //not initiator so should stay still
                    {
                        transitionOnNextTick = true;
                        if (action.Action.initiatorReachedGoal)
                        {

                            this.Machine.SetNextState<PerformAction>();
                        }
                        else
                        {
                            this.Machine.SetNextState<WaitForPerson>();
                        }


                    }

                    else //is initiator
                    {
                        if (person.reachedGoal)
                        {
                            action.Action.initiatorReachedGoal = true;
                            action.Action.NotifyInitiatorReachedGoal();
                            transitionOnNextTick = true;
                            if (action.interactionPerson.IsAvailable)
                            {
                                this.Machine.SetNextState<PerformAction>();

                            }
                            else
                            {
                                this.Machine.SetNextState<WaitForPerson>();
                            }


                        }
                    }


                }



            }

            public override void End()
            {
                var action = this.Context.actionPlan.Peek();

                if (action.GetType() == typeof(GOAPActionWithItem))
                {
                    person.currentHouse = item.house;
                    person.currentBuilding = null;
                }
                else if (action.GetType() == typeof(GOAPActionWithBuilding))
                {
                    person.currentHouse = null;
                    person.currentBuilding = action.building;

                }
                else
                {
                    person.currentHouse = House.getHouseContainingPoint(location);
                }
                    

            }






        }


        public class WaitForItem : State<GOAPPersonState>
        {
            GOAPAction action;
            People person;
            public WaitForItem() { }

            public override void Begin()
            {
                person = this.Context.planner.person;
                transitionOnNextTick = false;
                action = this.Context.actionPlan.Peek();
            }

            public override void Update(GameTime gameTime)
            {
                
                if (action.doingAction.FirstOrDefault()==person) //(action.doingAction.FirstOrDefault()==person && action.item.IsAvailable)
                {
                    transitionOnNextTick = true;
                    
                    this.Machine.SetNextState<GoTo>();

                }


            }
        }


        public class WaitForPerson : State<GOAPPersonState>
        {
            GOAPAction action;
            People person;
            
            public WaitForPerson() { }

            public override void Begin()
            {
                person = this.Context.planner.person;
                
                transitionOnNextTick = false;
                action = this.Context.actionPlan.Peek();
                

            }

            public override void Update(GameTime gameTime)
            {
                if (action.Action.initiator != person) //not initiator so has to wait for initiator to reach goal
                {

                    if (action.Action.initiatorReachedGoal)
                    {
                        transitionOnNextTick = true;

                        this.Machine.SetNextState<PerformAction>();

                    }
                }

                else //is initiator so has to wait for other person to finish what they are doing
                {
                    if (action.interactionPerson.IsAvailable)
                    {
                        transitionOnNextTick = true;
                        this.Machine.SetNextState<PerformAction>();
                    }


                }


            }
        }

        public class LeaveBuilding : State<GOAPPersonState>
        {
            People person;
            Vector3 location;

            public override void Begin()
            {
                transitionOnNextTick = false;

                
                person = this.Context.planner.person;

                this.Context.destinationLocation = location;

                location = person.currentBuilding.TownLocation;

                person.actionState = PeopleActionStates.beginMoving;
                person.goal = location;

            }

            public override void Update(GameTime gameTime)
            {
                this.Context.currentLocation = person.position;

                person.actionState = PeopleActionStates.moving; //rests action state in case interrupted by selecting item etc that changes state in Player.Update


                if (person.reachedGoal)
                {
                    transitionOnNextTick = true;
                    if (this.Context.actionPlan.Count == 0)
                    {
                        this.Machine.SetNextState<Idle>();

                    }
                    else { this.Machine.SetNextState<GoTo>(); } //may be talking to person in which case would have to leave building first before starting action
                    
                }
                
            }


            public override void End()
            {
                person.currentBuilding = null;
            }
        }




        public class PerformAction : State<GOAPPersonState>
        {

            People person;
            GameTime timer;
            Action<GameTime, Dictionary<NeedNames, Need>, People> currentActionMethod;



            GOAPAction action;
            //Item actionItem;
            public PerformAction()
            {
                

            }

            public override void Begin()
            {
                transitionOnNextTick = false;



                action = this.Context.actionPlan.Peek();
                //actionItem = action.item;
                person = this.Context.planner.person;


                person.IsAvailable = false;

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

                   
                    if (this.Context.actionPlan.Count > 0 && this.Context.actionPlan.Peek().GetType()!= typeof(GOAPActionWithPerson)) 
                    {
                        this.Machine.SetNextState<GoTo>();

                    }
                    else
                    {
                        if (action.GetType() == typeof(GOAPActionWithBuilding))
                        {
                            this.Machine.SetNextState<LeaveBuilding>(); //only needs to leave building if not going somewhere else anyway
                        }
                        else
                        {
                            this.Machine.SetNextState<Idle>();
                        }

                    }


                }
                else
                {
                    currentActionMethod(gameTime, person.Needs, person);


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

                if (action.GetType() != typeof(GOAPActionWithBuilding)) {action.doingAction.Dequeue(); }               
                person.IsAvailable = true;

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
