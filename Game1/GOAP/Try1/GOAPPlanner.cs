using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GOAP
{
    public class GOAPPlanner<T, W>
    {
        private GOAPAgent<T, W> agent;
        private GOAPGoal<T, W> currentGoal;
        public bool calculated;

        private readonly Pathfinder<GOAPState<T, W>> pathfinder;
        private readonly GOAPPlannerSettings settings;


        public GOAPPlanner(GOAPPlannerSettings argSettings = null)
        {
            this.settings = settings ?? new GOAPPlannerSettings();
            pathfinder = new Pathfinder<GOAPState<T, W>>();


        }


        public GOAPGoal<T, W> Plan(GOAPAgent<T, W> argAgent, GOAPGoal<T, W> blacklistGoal = null, Queue<GOAPActionState<T, W>> argCurrentPlan = null, Action<GOAPGoal<T, W>> callback = null)
        {
            agent = argAgent;
            calculated = false;
            currentGoal = null;

            List<GOAPGoal<T, W>> possibleGoals = new List<GOAPGoal<T, W>>();

            foreach (var goal in agent.GetGoalsSet())
            {
                if (goal == blacklistGoal) { continue; }
                goal.Precalculations(this);
                if (goal.IsGoalPossible())
                {
                    possibleGoals.Add(goal);
                }

            }

            possibleGoals.Sort((x, y) => x.GetPriority().CompareTo(y.GetPriority()));

            var currentState = agent.GetMemory().GetWorldState();

            while (possibleGoals.Count> 0)
            {
                currentGoal = possibleGoals[possibleGoals.Count - 1];
                possibleGoals.RemoveAt(possibleGoals.Count - 1);
                var goalState = currentGoal.GetGoalState();

                if (!settings.usingDyanimcActions)
                {
                    var wantedGoalCheck = currentGoal.GetGoalState();
                    GOAPActionStack<T, W> stackData;

                    stackData.agent = agent;
                    stackData.currentState = currentState;
                    stackData.goalState = goalState;
                    stackData.next = null;
                    stackData.settings = null;


                    foreach (var action in agent.GetActionsSet())
                    {

                        action.Precalculations(stackData);

                        if (!action.CheckProceduralCondition(stackData))
                        {
                            continue;
                        }


                        var previous = wantedGoalCheck;
                        wantedGoalCheck = GOAPState<T, W>.Instantiate();
                        previous.MissingDifference(action.GetEffects(stackData), ref wantedGoalCheck);



                    }

                    var current = wantedGoalCheck;
                    wantedGoalCheck = GOAPState<T, W>.Instantiate();
                    current.MissingDifference(GetCurrentAgent().GetMemory().GetWorldState(), ref wantedGoalCheck);

                    if (wantedGoalCheck.Count > 0)
                    {
                        currentGoal = null;
                        continue;
                    }

                }

                goalState = goalState.Clone();
                var leaf = (GOAPNode<T, W>)pathfinder.Run(GOAPNode<T, W>.Instantiate(this, goalState, null, null, null), goalState, settings.maxIterations, settings.planningEarlyExit);

                if (leaf == null)
                { 
                
                    currentGoal = null;
                    continue;
                }


                var result = leaf.CalculatePath();


                if (argCurrentPlan != null && argCurrentPlan == result)
                {
                    currentGoal = null;
                    break;
                }

                if (result.Count == 0)
                {
                    currentGoal = null;
                    continue;
                }

                currentGoal.SetPlan(result);
                break;



            }

            calculated = true; 

            if (callback != null)
            {
                callback(currentGoal);
            }


            return currentGoal;


        }



        public GOAPGoal<T, W> GetCurrentGoal() { return currentGoal; }
        public GOAPAgent<T, W> GetCurrentAgent() { return agent; }
        public bool IsPlanning() { return !calculated; }
        public GOAPPlannerSettings GetSettings() { return settings; }

    }
}
