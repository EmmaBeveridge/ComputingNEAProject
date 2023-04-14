using Game1.Town;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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


            /// <summary>
            /// Returns GOAP string condition name for fulfilment of need supplied as parameter. 
            /// </summary>
            /// <param name="needName">Low need to be fulfilled</param>
            /// <returns></returns>
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

            /// <summary>
            ///Overloaded method. If parameter supplied specifying need to fulfil, a world state in which that need is fulfilled is returned. WorldState object contains data regarding the GOAPPerson’s fulfilment of needs. 
            /// </summary>
            /// <param name="needToFulfill"></param>
            /// <returns></returns>
            public WorldState GetWorldState(NeedNames needToFulfill)
            {
                //doesnt deal with chance of needToFulfill being null
                var worldState = this.planner.CreateWorldState();
                worldState.Set(GetConditionNameFromNeedName(needToFulfill), true);
                return worldState;

            }


            /// <summary>
            /// Overloaded method. If no parameter specified, a world state representing current character need state is returned. WorldState object contains data regarding the GOAPPerson’s fulfilment of needs. 
            /// </summary>
            /// <returns></returns>
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


            /// <summary>
            ///  Calls GetResult method on People.decisionTree with charcater’s needs dictionary as parameter to obtain need to fulfill. Returns a WorldState object where this need is set to be fulfilled. This goal state is what the GOAP will attempt to find a sequence of actions that will change the current world state into this goal state. 
            /// </summary>
            /// <returns></returns>
            public override WorldState GetGoalState()
            {
                NeedNames tempRef = NeedNames.Null;



                return GetGoalState(ref tempRef);
            }

            /// <summary>
            /// Returns a WorldState object where the desired need to fulfil supplied as parameter is set to fulfilled. This goal state is what the GOAP will attempt to find a sequence of actions that will change the current world state into this goal state.
            /// </summary>
            /// <param name="needToFulfill"></param>
            /// <returns></returns>
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

            /// <summary>
            ///   Calls GetResult method on People.decisionTree with charcater’s needs dictionary as parameter to obtain need to fulfill. Returns a WorldState object where this need is set to be fulfilled. This goal state is what the GOAP will attempt to find a sequence of actions that will change the current world state into this goal state. 
            /// </summary>
            /// <param name="needToFulfill"></param>
            /// <returns></returns>
            public override WorldState GetGoalState(ref NeedNames needToFulfill) 
            {
                
                var goalState = this.planner.CreateWorldState();

                bool printTrace = true; //this.planner.person.isPlayer ? true : false;
                if (printTrace) { Console.WriteLine($"\n\nID3 Query Trace for {this.planner.person.Name}\nTraits: {this.planner.person.Traits[0]} and {this.planner.person.Traits[1]}\n"); }

                NeedNames need = People.decisionTree.GetResult(Needs, printTrace) ;


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


        /// <summary>
        /// Creates new GOAPPersonState object and ActionPlanner object. Adds all available actions in the town to the ActionPlanner object. Initialises new FSM adding Idle, GoTo, PerformAction, WaitForItem, WaitForPerson, LeaveBuilding states.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Pushes new action onto the action stack. If an action is currently taking place, it is pushed to just underneath the current action. If action is with an item, GOAPWorld.ReserveItem used to reserve the item for character. 
        /// </summary>
        /// <param name="action"></param>
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

            /// <summary>
            /// Uses GetGoalState method on GOAPPersonState object to find goal state. Calls Plan method on GOAPPersonState ActionPlanner attribute to obtain action plan stack to achieve goal state. If the action plan contains a GOAPActionWithPerson, then, if the person has been initialised, the SendRSVP method is used on the person to inform the NPC of the plan to interact.
            /// </summary>
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
                    Thread.Sleep(10);
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


            /// <summary>
            /// As planning is all done in one tick of machine, machine is set to transition on its next tick and the GoTo state is set as the machine’s next state. 
            /// </summary>
            /// <param name="gameTime"></param>
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


            /// <summary>
            /// Determines location character should go to depending on the type of action to be carried out (action obtained by peeking action stack). If the character is to interact with an item and they are first in the queue to use the item, they should go to the location of the item, if they must wait for other characters to use the item first, they should go to the origin of the room the item is in and wait there until it is their turn to use the item. As multiple people can use a building at once, if the action is with a building the character should go straight into the building. If the action is with another person and this character was the initiator of the action, they should navigate to the other person by accessing the position of the interactionPerson for the action ; if however they were not the initiator of the action, they should remain where they are (if they are in a building they should exit it first). The person.actionState is then set to People.ActionStates.beginMoving.  
            /// </summary>
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


            /// <summary>
            /// Called each update cycle as character moves to goal location and sets person into the appropriate next state when the goal is reached. (As the player character will also enter the GoTo state when navigating to a player picked spot without any action to undertake: if this is the case, the character is then set into the Idle state upon reaching the goal). If the person has reached goal and their next action is with an item: if they are the first in the queue to use the item, their next state is set as PerformAction, if they are not first in the queue, their next state is set as WaitForItem.  If the action is with a building, the person's state is set to PerformAction. If the action is with another person: if the character is not the initiator of the action, they must check if the initiator has reached the goal in which case they can transition to the PerformAction state, if not, they transition to the WaitForPersonState until the initiator has reached the goal. If the character is the initiator and they have reached the other person, the action’s AbstractAction field’s property of initiatorReachedGoal is set to true and the NotifyInitiatorReachedGoal() method is called. If the other person is available, the initiator is set to transition to the PerformAction state, otherwise their next state is set as WaitForPerson.
            /// </summary>
            /// <param name="gameTime"></param>
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


            /// <summary>
            ///  Called just before transitioning out of GoTo state. Used to set currentBuilding, currentHouse attributes of person to the correct buildings/houses. 
            /// </summary>
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


            /// <summary>
            /// Assigns people and action attributes. Determines action by peeking action stack.
            /// </summary>
            public override void Begin()
            {
                person = this.Context.planner.person;
                transitionOnNextTick = false;
                action = this.Context.actionPlan.Peek();
            }


            /// <summary>
            /// Checks if person has reached front of queue for action, if they have: the machine transitions to go to state. 
            /// </summary>
            /// <param name="gameTime"></param>
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

            /// <summary>
            /// If the character is the initiator: checks if the other person is now available, if so, machine is set to PerformAction state. If the character is not the initiator: checks if the initiator has reached them, if so, machine is set to PerformAction state. 
            /// </summary>
            /// <param name="gameTime"></param>
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


            /// <summary>
            /// Sets location variables to building’s origin town location. Sets people.actionState to beginMoving state. 
            /// </summary>
            public override void Begin()
            {
                transitionOnNextTick = false;

                
                person = this.Context.planner.person;
                
                location = person.currentBuilding.TownLocation;

                this.Context.destinationLocation = location;

                

                person.actionState = PeopleActionStates.beginMoving;
                person.goal = location;

            }

            /// <summary>
            /// Checks if person has reached the goal outside of building. If they have reached their goal and the action stack is empty, machine is set to Idle state. If they have reached their goal and the action stack is not empty, the machine is set to GoTo state. 
            /// </summary>
            /// <param name="gameTime"></param>
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

            /// <summary>
            /// Sets person’s currentBuilding attribute to null as no longer in building. 
            /// </summary>
            public override void End()
            {
                person.currentBuilding = null;
            }
        }




        public class PerformAction : State<GOAPPersonState>
        {

            People person;

            /// <summary>
            /// Method to be called each update cycle to simulate action. 
            /// </summary>
            Action<GameTime, Dictionary<NeedNames, Need>, People> currentActionMethod;



            GOAPAction action;
            //Item actionItem;
            
            /// <summary>
            /// Constructor for new PerformAction state 
            /// </summary>
            public PerformAction()
            {
                

            }


            /// <summary>
            /// Peeks action stack to assign action and currentActionMethod attributes. Calls BeginAction method on ActionAbstract Action attribute on GOAPAction action variable. 
            /// </summary>
            public override void Begin()
            {
                transitionOnNextTick = false;



                action = this.Context.actionPlan.Peek();
                //actionItem = action.item;
                person = this.Context.planner.person;


                person.IsAvailable = false;

                currentActionMethod = action.Action.ActionMethod;
                action.Action.BeginAction(person);

                //currentAction = actionItem.BeginAction(action.Name, person);



            }


            /// <summary>
            /// If the action is not complete, currentActionMethod is called to progress action. If the action is completed, action is removed from the action stack by popping the stack. If the next action in the stack is not an interaction with another person, the machine is set into the GoTo state. If the action stack is empty, if the character is finishing interacting with a building, they must then leave the building, so the FSM is set into a LeaveBuilding state, otherwise it is set into the Idle state. 
            /// </summary>
            /// <param name="gameTime"></param>
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


            /// <summary>
            ///  Called before transitioning state. Person is dequeued from action’s doingAction queue. Person is set to be available. 
            /// </summary>
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
