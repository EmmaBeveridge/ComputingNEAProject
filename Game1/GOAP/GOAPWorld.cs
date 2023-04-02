using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GOAP
{
    public class GOAPWorld
    {
        private static readonly GOAPStorage Storage = new GOAPStorage();

        /* from: http://theory.stanford.edu/~amitp/GameProgramming/ImplementationNotes.html
        OPEN = priority queue containing START
        CLOSED = empty set
        while lowest rank in OPEN is not the GOAL:
          current = remove lowest rank item from OPEN
          add current to CLOSED
          for neighbors of current:
            cost = g(current) + movementcost(current, neighbor)
            if neighbor in OPEN and cost less than g(neighbor):
              remove neighbor from OPEN, because new path is better
            if neighbor in CLOSED and cost less than g(neighbor): **
              remove neighbor from CLOSED
            if neighbor not in OPEN and neighbor not in CLOSED:
              set g(neighbor) to cost
              add neighbor to OPEN
              set priority queue rank to g(neighbor) + h(neighbor)
              set neighbor's parent to current
        */

        /// <summary>
        /// Uses A* algorithm to find shortest sequence of GOAP actions to reach goal world state from start state. Make a plan of actions that will reach desired world state
        /// </summary>
        /// <param name="ap">Ap.</param>
        /// <param name="start">Start.</param>
        /// <param name="goal">Goal.</param>
        /// <param name="selectedNodes">Storage.</param>
        public static Stack<GOAPAction> Plan(ActionPlanner ap, WorldState start, WorldState goal, List<GOAPNode> selectedNodes = null)
        {
            Storage.Clear();

            var currentNode = new GOAPNode();
            currentNode.WorldState = start;
            currentNode.ParentWorldState = start;
            currentNode.CostSoFar = 0; // g
            currentNode.HeuristicCost = CalculateHeuristic(start, goal); // h
            currentNode.CostSoFarAndHeuristicCost = currentNode.CostSoFar + currentNode.HeuristicCost; // f
            currentNode.Depth = 1;

            Storage.AddToOpenList(currentNode);

            while (true)
            {
                // nothing left open so we failed to find a path
                if (!Storage.HasOpened())
                {
                    Storage.Clear();
                    return null;
                }

                currentNode = Storage.RemoveCheapestOpenNode();

                Storage.AddToClosedList(currentNode);

                // all done. we reached our goal
                if (goal.Equals(currentNode.WorldState))
                {           
                    var plan = ReconstructPlan(currentNode, selectedNodes, ap);
                    Storage.Clear();
                    return plan;
                }

                var neighbors = ap.GetPossibleTransitions(currentNode.WorldState);
                for (var i = 0; i < neighbors.Count; i++)
                {
                    var cur = neighbors[i];
                    var opened = Storage.FindOpened(cur);
                    var closed = Storage.FindClosed(cur);
                    var cost = currentNode.CostSoFar + cur.CostSoFar;

                    // if neighbor in OPEN and cost less than g(neighbor):
                    if (opened != null && cost < opened.CostSoFar)
                    {
                        // remove neighbor from OPEN, because new path is better
                        Storage.RemoveOpened(opened);
                        opened = null;
                    }

                    // if neighbor in CLOSED and cost less than g(neighbor):
                    if (closed != null && cost < closed.CostSoFar)
                    {
                        // remove neighbor from CLOSED
                        Storage.RemoveClosed(closed);
                    }

                    // if neighbor not in OPEN and neighbor not in CLOSED:
                    if (opened == null && closed == null)
                    {
                        var nb = new GOAPNode
                        {
                            WorldState = cur.WorldState,
                            CostSoFar = cost,
                            HeuristicCost = CalculateHeuristic(cur.WorldState, goal),
                            Action = cur.Action,
                            ParentWorldState = currentNode.WorldState,
                            Parent = currentNode,
                            Depth = currentNode.Depth + 1
                        };
                        nb.CostSoFarAndHeuristicCost = nb.CostSoFar + nb.HeuristicCost;
                        Storage.AddToOpenList(nb);
                    }
                }
            }
        }


        /// <summary>
        /// Traces sequence of actions to achieve goal state using parent node pointer property of GOAPNode and adds them to GOAPAction plan stack. Action on the top of the stack is the first action to be completed.
        /// </summary>
        /// <returns>The plan.</returns>
        private static Stack<GOAPAction> ReconstructPlan(GOAPNode goalNode, List<GOAPNode> selectedNodes, ActionPlanner ap)
        {
            var totalActionsInPlan = goalNode.Depth - 1;
            var plan = new Stack<GOAPAction>(totalActionsInPlan);

            var curnode = goalNode;
            for (var i = 0; i <= totalActionsInPlan - 1; i++)
            {
                // optionally add the node to the List if we have been passed one
                selectedNodes?.Add(curnode.Clone());
                plan.Push(curnode.Action);
                ReserveItem(action: curnode.Action, actionPlanner: ap );
                curnode = curnode.Parent;
            }

            // our nodes went from the goal back to the start so reverse them
            selectedNodes?.Reverse();

            return plan;
        }



        /// <summary>
        /// Reserves item used to execute determined action. If the action is with an item: item availability set to false, person is enqueued into action’s doingAction queue. If the action is with another person, interaction person’s availability set to false, person is enqueued to action’s doingAction queue, action’s initiator set as person. Equivalent action for other person is determined using FindActionWithPerson method on ActionPlanner object. Initiator for equivalent action for other person set as person, and other person enqueued to equivalent action’s doingAction queue. 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="actionPlanner"></param>
        public static void ReserveItem(GOAPAction action, ActionPlanner actionPlanner)
        {
            //if (action.item != null)
            //{
            //    action.item.IsAvailable = false;
            //    action.doingAction.Enqueue(actionPlanner.person);
            //}

            //if (action.interactionPerson != null)
            //{
            //    action.interactionPerson.IsAvailable = false;
            //    action.doingAction.Enqueue(actionPlanner.person);
            //}


            if (action.GetType() == typeof(GOAPActionWithItem))
            {
                action.item.IsAvailable = false;
                action.doingAction.Enqueue(actionPlanner.person);
            }

            else if (action.GetType() == typeof(GOAPActionWithPerson))
            {
                action.interactionPerson.IsAvailable = false;
                action.doingAction.Enqueue(actionPlanner.person);
                action.Action.initiator = actionPlanner.person;

                GOAPAction actionForOtherPerson = actionPlanner.FindActionWithPerson(actionPlanner.person);

                actionForOtherPerson.Action.initiator = actionPlanner.person;
                actionForOtherPerson.doingAction.Enqueue(action.interactionPerson);




               //do i need to add it so interactionPerson is in actionPlanner.persons talktopersonaction queue???


            }



        }












        /// <summary>
        /// Uses bitmask to determine the number of bits for which we care about the state and the state does not match that of the goal state. If a world state achieved by the action has very few condition bit states matching the goal state, it is determined to be at a further ‘distance’ from the goal and therefore of lower priority for algorithm to consider. Heuristic is nr of mismatched atoms that matter.
        /// </summary>
        /// <returns>The heuristic.</returns>
        private static int CalculateHeuristic(WorldState @from, WorldState to)
        {
            long care = (to.DontCare ^ -1L);// We first determine the binary string encoding the conditions we care about for goal world state using XOR operator with –1. As -1 in two's complement = 1111111111... when XOR performed with DontCare, all conditions that we care about (0 in DontCare) will be a 1 in care binary string and all values that we don't care about (1 in DontCare) will become 0 in care binary string.
            long diff = (@from.Values & care) ^ (to.Values & care);//We then calculate the difference between the current node state and goal node state for the conditions we care about and encode this in a binary string value. We calculate this difference by first applying the AND on the current node’s WorldState values and care and the goal node’s WorldState values and care. To find the difference between these values, we use the XOR operator which will produce a binary number with a 1 at each place value/condition index where we care about the condition and the values of the condition differs in the world states (as 1 XOR 0 = 1 and 0 XOR 1 = 1) and a 0 at each place value/condition index where we either don’t care about the condition or we do care about the condition but the values of the condition are the same for both world states. 
            int dist = 0;

            // Finally, we count the number of 1s in the binary string (stored as a long type value) to count the number of differences between states and return this as our heuristic. To count the number of 1s, we execute a loop for the number of bits in the binary string (the maximum number of conditions set as a constant in ActionPlanner class). We use the count of this loop to isolate each bit in the long value encoding the difference by left shifting 1 the number of places of the current count of the loop to produce a binary string where every bit is 0 except for the bit at the index of the condition to be examined. Applying the AND operator between this shifted value and the difference value, if the bit in the difference value is 1, I.e. goal and current nodes states differ for this condition and we care, the operator will evaluate to 1 (as 1 AND 1 = 1), if the bit difference value is 0, I.e. goal and current node states are the same for this condition and we care or we don’t care about the condition at all, the operator will evaluate to 0 (as 0 AND 1 = 0). If the operator evaluates to 1, we increment the count for number of 1s in difference string. When the loop is finished executing, we return this count.

            for (var i = 0; i < ActionPlanner.MaxConditions; ++i)//
            {
                if ((diff & (1L << i)) != 0)
                {
                    dist++;
                }
            }
            return dist;
        }

    }
}
