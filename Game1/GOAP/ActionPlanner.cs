using System.Collections.Generic;
using System.Text;

namespace Game1.GOAP
{
    public class ActionPlanner
    {
        public const int MaxConditions = 64;

        /// <summary>
        /// Names associated with all world state atoms
        /// </summary>
        public string[] ConditionNames = new string[MaxConditions];

        private readonly List<GOAPAction> actions = new List<GOAPAction>();

        private readonly List<GOAPAction> viableActions = new List<GOAPAction>();

        /// <summary>
        /// Preconditions for all actions
        /// </summary>
        private readonly WorldState[] preConditions = new WorldState[MaxConditions];

        /// <summary>
        /// Postconditions for all actions (action effects).
        /// </summary>
        private readonly WorldState[] postConditions = new WorldState[MaxConditions];

        /// <summary>
        /// Number of world state atoms.
        /// </summary>
        private int numConditionNames;

        public People person;

        /// <summary>
        /// Constructor for new ActionPlannner object. Creates and assigns a WorldState objects to preConditions and postConditions arrays; for each of these condition states, the condition name at index of condition in ConditionNames array is set to null.
        /// </summary>
        /// <param name="argPerson"></param>
        public ActionPlanner(People argPerson)
        {
            person = argPerson;

            this.numConditionNames = 0;
            for (var i = 0; i < MaxConditions; ++i)
            {
                this.ConditionNames[i] = null;
                this.preConditions[i] = WorldState.Create(this);
                this.postConditions[i] = WorldState.Create(this);
            }
        }


        /// <summary>
        /// Convenience method for fetching a new WorldState object with action planner instance as argument. 
        /// </summary>
        /// <returns>The world state.</returns>
        public WorldState CreateWorldState()
        {
            return WorldState.Create(this);
        }




        /// <summary>
        /// Returns GOAPAction where person supplied as parameter is the interactionPerson for the action. Used to find the action for the non-initiating person when two characters interact.
        /// </summary>
        /// <param name="interactionPerson"></param>
        /// <returns></returns>
        public GOAPAction FindActionWithPerson(People interactionPerson)
        {
            return actions.Find(a => a.interactionPerson == interactionPerson);
        }



        /// <summary>
        /// Adds list of actions using AddAction method. 
        /// </summary>
        /// <param name="goapActions"></param>
        public void AddRangeAction(List<GOAPAction> goapActions)
        {
            foreach (GOAPAction action in goapActions)
            {
                AddAction(action);
            }
        }



        /// <summary>
        /// Adds the preconditions and the postconditions to the list of all preconditions (preConditions property) and postconditions (postConditions property) at the index of the action id (the index of the action in the list of all actions I.e. actions property). Uses the Set method on the WorldState object to set the desired state of the condition on in the world state.
        /// </summary>
        /// <param name="goapAction"></param>
        public void AddAction(GOAPAction goapAction)
        {
            var actionId = this.FindActionIndex(goapAction);
            if (actionId == -1)
                throw new KeyNotFoundException("could not find or create Action");

            foreach (var preCondition in goapAction.PreConditions)
            {
                var conditionId = this.FindConditionNameIndex(preCondition.Item1);
                if (conditionId == -1)
                    throw new KeyNotFoundException("could not find or create conditionName");

                this.preConditions[actionId].Set(conditionId, preCondition.Item2);
            }

            foreach (var postCondition in goapAction.PostConditions)
            {
                var conditionId = this.FindConditionNameIndex(postCondition.Item1);
                if (conditionId == -1)
                    throw new KeyNotFoundException("could not find conditionName");

                this.postConditions[actionId].Set(conditionId, postCondition.Item2);
            }
        }




        /// <summary>
        /// Validates each of the possible actions, if an action is not valid then it is not added to the list of viable actions to be considered by the GOAP. The cost of each action, which depends on the Euclidean distance to the person, is also updated. The method then calls the Plan method on GOAPWorld class, specifying the start state and the goal state. 
        /// </summary>
        /// <param name="startState"></param>
        /// <param name="goalState"></param>
        /// <param name="selectedNodes"></param>
        /// <returns></returns>
        public Stack<GOAPAction> Plan(WorldState startState, WorldState goalState, List<GOAPNode> selectedNodes = null)
        {
            this.viableActions.Clear();

           
            foreach (GOAPAction action in this.actions)
            {
                if (action.Validate(person))
                {
                    action.UpdateCost(person, person.Needs);
                    this.viableActions.Add(action);
                    
                }
            }
            

            return GOAPWorld.Plan(this, startState, goalState, selectedNodes);
        }


        /// <summary>
        /// Describe the action planner by listing all actions with pre and post conditions. For debugging purpose.
        /// </summary>
        public string Describe()
        {
            var sb = new StringBuilder();
            for (var a = 0; a < this.actions.Count; ++a)
            {
                sb.AppendLine(this.actions[a].GetType().Name);

                var pre = this.preConditions[a];
                var pst = this.postConditions[a];
                for (var i = 0; i < MaxConditions; ++i)
                {
                    if ((pre.DontCare & (1L << i)) == 0)
                    {
                        bool v = (pre.Values & (1L << i)) != 0;
                        sb.AppendFormat("  {0}=={1}\n", this.ConditionNames[i], v ? 1 : 0);
                    }
                }

                for (var i = 0; i < MaxConditions; ++i)
                {
                    if ((pst.DontCare & (1L << i)) == 0)
                    {
                        bool v = (pst.Values & (1L << i)) != 0;
                        sb.AppendFormat("  {0}:={1}\n", this.ConditionNames[i], v ? 1 : 0);
                    }
                }
            }

            return sb.ToString();
        }



        /// <summary>
        /// Returns the index at which the specified condition name is stored in the list ConditionNames, if the condition name does not exist in the list and the list is not at maximum size, then it is added to list at the last index. This condition name index is used as the condition id for used to store the desired value (0 or 1) of the condition using bit masking in WorldState object. 
        /// </summary>
        /// <param name="conditionName"></param>
        /// <returns></returns>
        internal int FindConditionNameIndex(string conditionName)
        {
            int idx;
            for (idx = 0; idx < this.numConditionNames; ++idx)
            {
                if (string.Equals(this.ConditionNames[idx], conditionName))
                    return idx;
            }

            if (idx < MaxConditions - 1)
            {
                this.ConditionNames[idx] = conditionName;
                this.numConditionNames++;
                return idx;
            }

            return -1;
        }



        /// <summary>
        /// Returns the index of the specified GOAPAction in the list actions. If the action is not present in the list of actions, it is added at the last index. This action index is used as the action id, which becomes the index at which the preconditions and postconditions are stored for that action in the preConditions and postConditions lists. 
        /// </summary>
        /// <param name="goapAction"></param>
        /// <returns></returns>
        internal int FindActionIndex(GOAPAction goapAction)
        {
            var idx = this.actions.IndexOf(goapAction);
            if (idx > -1) { return idx; }

            this.actions.Add(goapAction);

            return this.actions.Count - 1;
        }




        /// <summary>
        /// Returns list of all of the GOAPNode objects that can be transitioned to from the specified world state without violating any of the preconditions of the actions. This evaluation is done using bitmasking and bitwise operators. The conditions and their desired boolean values are represented by a long binary string ( 1 being the condition represented at that position in the string should be true, 0 being it should be false). As we can only express explicitly true and false, we have a second binary string that describes whether we actually care about the state of that value (for the variable care defined in the method, obtained by performing XOR operator on DontCare property of the world state and –1 in twos complement notation I.e. 1111111.....). We can then compare if the states of the conditions we actuall care about match in the current world state and the possible transition world state. If they match, that means a transition exists that does not violate any of the preconditions of the transition state. We therefore create a new GOAPNode to represent this transition state, with its world state being that of the current state with the postconditions applied. This GOAPNode is added to a list of the possible transition nodes to be returned. 
        /// </summary>
        /// <param name="fr"></param>
        /// <returns></returns>
        internal List<GOAPNode> GetPossibleTransitions(WorldState fr)
        {
            var result = new List<GOAPNode>();


            foreach (GOAPAction action in viableActions)
            {
                int actionIndex = FindActionIndex(action);
                var pre = this.preConditions[actionIndex];
                var care = pre.DontCare ^ -1L;
                bool met = ((pre.Values & care) == (fr.Values & care));
                if (met)
                {
                    var node = new GOAPNode
                    {
                        Action = action,
                        CostSoFar = action.Cost,
                        WorldState = this.ApplyPostConditions(this, actionIndex, fr)
                    };

                    result.Add(node);
                }

            }
            return result;




            //for (var i = 0; i < this.viableActions.Count; ++i)
            //{
            //    // see if precondition is met


            //    var pre = this.preConditions[i];
            //    var care = (pre.DontCare ^ -1L);
            //    bool met = ((pre.Values & care) == (fr.Values & care));
            //    if (met)
            //    {
            //        var node = new GOAPNode
            //        {
            //            Action = this.viableActions[i],
            //            CostSoFar = this.viableActions[i].Cost,
            //            WorldState = this.ApplyPostConditions(this, i, fr)
            //        };
            //        result.Add(node);
            //    }
            //}
            //return result;
        }



        /// <summary>
        /// Obtains the action’s postconditions using the action’s id – the postconditions list being indexed by the action ids. We determine the conditions that will be unaffected using the DontCare property on the postcondition world state, we then use an XOR operator on the unaffected values and –1 (which is 11111... in twos complement) to get a binary string in which a 1 means the condition represented by that position in the string has been affected by the action and 0 meaning that it hasn’t. In order to apply the postconditions we use another sequence of bitwise operators. First, we perform an AND operator on the postcondition values and the values we know will be unaffected, returning a binary string that represents all conditions that were deemed to be true in original state and should have been unaffected as being true, with all other conditions set to false. We then do the same AND operation on the conditions in the postcondition state and the values we know to have been affected. This will return a binary string in which all of the values that have been affected and are true from the postcondition are set as true, with all others set to false. In order to combine these binary strings and obtain a string that accurately describes the current world state (in which formerly true conditions that were not affected remain true and conditions that were affected and are now also true) we perform an OR operation using these two strings. Finally, we set the DontCare values for the original world state to be those that we also do not care about in the postcondition state using the AND operator between them. 
        /// </summary>
        /// <param name="ap"></param>
        /// <param name="actionnr"></param>
        /// <param name="fr"></param>
        /// <returns></returns>
        internal WorldState ApplyPostConditions(ActionPlanner ap, int actionnr, WorldState fr)
        {
            var pst = ap.postConditions[actionnr];
            long unaffected = pst.DontCare;
            long affected = (unaffected ^ -1L);

            fr.Values = (fr.Values & unaffected) | (pst.Values & affected);
            fr.DontCare &= pst.DontCare;
            return fr;
        }
    }
    }

