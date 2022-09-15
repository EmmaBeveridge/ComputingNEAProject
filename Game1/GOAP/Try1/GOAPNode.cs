using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GOAP
{
    public class GOAPNode<T, W> : INode<GOAPState<T, W>>
    {
        private float cost;
        private float costToParent;
        
        private GOAPPlanner<T, W> planner;
        private GOAPNode<T, W> parent;
        private IGOAPAction<T, W> action;
        private GOAPState<T, W> actionSettings;
        private GOAPState<T, W> state;
        private readonly List<INode<GOAPState<T, W>>> expandList;
        private GOAPState<T, W> goalMergedWithWorld;
        private static Stack<GOAPNode<T, W>> cachedNodes;


        public string name { get { return action != null ? action.GetName() : "No Action"; } }
        public GOAPState<T, W> goal { get; private set; }
        public GOAPState<T, W> effects { get; private set; }
        public GOAPState<T, W> preconditions { get; private set; }

        public float priority { get; set; }
        public float insertionIndex { get; set; }
        public int queueIndex { get; set; }





        private GOAPNode()
        {
            expandList = new List<INode<GOAPState<T, W>>>();
        }

        private void Init (GOAPPlanner<T, W> argPlanner, GOAPState<T, W> newGoal, GOAPNode<T, W> argParent, IGOAPAction<T, W> argAction, GOAPState<T, W> argSettings)
        {
            expandList.Clear();

            planner = argPlanner;
            parent = argParent;
            action = argAction;

            if (argSettings != null)
            {
                actionSettings = argSettings.Clone();
            }


            if (parent != null)
            {
                state = parent.GetState().Clone();

                costToParent = parent.GetPathCost();

            }


            else
            {
                state = planner.GetCurrentAgent().GetMemory().GetWorldState().Clone();

            }

            var nextAction = parent == null ? null : parent.action;

            if (action != null)
            {
                goal = GOAPState<T, W>.Instantiate(newGoal);
                GOAPActionStack<T, W> stackData;

                stackData.currentState = state;
                stackData.goalState = goal;
                stackData.next = action;
                stackData.agent = planner.GetCurrentAgent();
                stackData.settings = actionSettings;

                preconditions = action.GetPreconditions(stackData);

                effects = action.GetEffects(stackData);
                cost += action.GetCosts(stackData);

                state.AddFromState(effects); // adds action's effects to node's state

                goal.ReplaceWithMissingDifferences(effects); //removes from goal conditions action's effect fulfilled

                goal.AddFromState(preconditions); //adds actions preconditions to goal




            }

            else
            {
                goal = newGoal;
            }

            var difference = GOAPState<T, W>.Instantiate();
            goal.MissingDifference(planner.GetCurrentAgent().GetMemory().GetWorldState(), ref difference);
            goalMergedWithWorld = difference;





        }







        public static void CacheStackWarmup (int count)
        {

            cachedNodes = new Stack<GOAPNode<T, W>>(count);
            for (int i = 0; i < count; i++)
            {
                cachedNodes.Push(new GOAPNode<T, W>());
            }


        }


        public void Recycle()
        {
            state.Recycle();
            state = null;
            goal.Recycle();
            goal = null;

            lock (cachedNodes)
            {
                cachedNodes.Push(this);
            }

        }


        public static GOAPNode<T, W> Instantiate (GOAPPlanner<T, W> planner, GOAPState<T, W> newGoal, GOAPNode<T, W> parent, IGOAPAction<T, W> action, GOAPState<T,W> actionSettings)
        {
            GOAPNode<T, W> node;

            if (cachedNodes == null)
            {
                cachedNodes = new Stack<GOAPNode<T, W>>();
            }

            lock (cachedNodes)
            {
                node = cachedNodes.Count > 0 ? cachedNodes.Pop() : new GOAPNode<T, W>();

            }
            node.Init(planner, newGoal, parent, action, actionSettings);

            return node;
        }

        public float GetPathCost() { return cost; }
        public GOAPState<T, W> GetState() { return state; }

        public List<INode<GOAPState<T, W>>> Expand()
        {
            expandList.Clear();

            var agent = planner.GetCurrentAgent();
            var actions = agent.GetActionsSet();

            GOAPActionStack<T, W> stackData;

            stackData.currentState = state;
            stackData.goalState = goal;
            stackData.next = action;
            stackData.agent = agent;

            stackData.settings = null;

            for (int i = actions.Count() - 1; i >= 0; i--) 
            {
                var possibleAction = actions[i];
                possibleAction.Precalculations(stackData);
                var settingsList = possibleAction.GetSettings(stackData);
                
                foreach (var settings in settingsList)
                {
                    stackData.settings = settings;

                    GOAPState<T, W> preconditions = possibleAction.GetPreconditions(stackData);
                    GOAPState<T,W> effects = possibleAction.GetEffects(stackData);


                    if (effects.HasAny(goal) && !goal.HasAnyConflict(effects, preconditions) && !goal.HasAnyConflict(effects) && possibleAction.CheckProceduralCondition(stackData))
                    {
                        var newGoal = goal;

                        expandList.Add(Instantiate(planner, newGoal, this, possibleAction, settings));




                    }

                }




            }


            return expandList;

        }

        private IGOAPAction<T, W> GetAction() { return action; }
        public Queue<GOAPActionState<T, W>> CalculatePath()
        {
            Queue<GOAPActionState<T, W>> resultPath = new Queue<GOAPActionState<T, W>>();
            CalculatePath(ref resultPath);
            return resultPath;
        }

        public void CalculatePath( ref Queue<GOAPActionState<T, W>> resultPath)
        {
            var node = this;

            while (node.GetParent() != null)
            {
                resultPath.Enqueue(new GOAPActionState<T, W>(node.action, node.actionSettings));
                node = (GOAPNode<T, W>)node.GetParent();

            }


        }

        public int CompareTo(INode<GOAPState<T, W>> other)
        {
            return cost.CompareTo(other.GetCost()); 
        }



        public float GetCost() { return cost; }
        public INode<GOAPState<T, W>> GetParent() { return parent; }
        public bool IsGoal(GOAPState<T, W > goal)
        {
            return goalMergedWithWorld.Count <= 0;
        }


    }

    public interface INode<T>
    {
        T GetState();
        List<INode<T>> Expand();
        int CompareTo(INode<T> other);
        float GetCost();
        INode<T> GetParent();
        bool IsGoal(T goal);

        string name { get; }
        T goal { get; }
        T effects { get; }
        T preconditions { get; }

        int queueIndex { get; set; }
        float priority { get; set; }
        void Recycle();
    }
}
