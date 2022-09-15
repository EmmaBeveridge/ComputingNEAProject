using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Game1.GOAP
{
    public class GOAPAgent<T, W>
    {
        public string name;
        public float calculationDelay = 0.5f;
        public bool blackListGoalIfFailed;

        public bool calculateNewGoalOnStart = true;

        protected GameTime gameTime = new GameTime();

        protected double lastCalculationTime = 0;

        protected List<GOAPGoal<T, W>> goals;
        protected List<IGOAPAction<T, W>> actions;
        protected GOAPMemory<T, W> memory;
        protected GOAPGoal<T, W> currentGoal;

        protected GOAPActionState<T, W> currentActionState;

        protected Dictionary<GOAPGoal<T, W>, float> goalBlacklist;
        protected List<GOAPGoal<T, W>> possibleGoals;
        protected bool possibleGoalsDirty;

        protected List<GOAPActionState<T, W>> startingPlan;
        protected Dictionary<T, W> planValues;
        protected bool interruptOnNextTransition;


        protected bool startedPlanning;
        protected GOAPPlanWork<T, W> currentPlanWorker;

        public bool IsPlanning { get { return startedPlanning && currentPlanWorker.newGoal == null; } }

       


        protected virtual void Init()
        {
            goalBlacklist = new Dictionary<GOAPGoal<T, W>, float>();
            RefreshGoalsSet();
            RefreshActionsSet();
            RefreshMemory();

            if (calculateNewGoalOnStart) { CalculateNewGoal(true); }


        }


        public virtual void OnDisable()
        {
            if (currentActionState!= null)
            {
                currentActionState.action.Exit(null);
                currentActionState = null;
                currentGoal = null;

            }
        }





        protected virtual void UpdatePossibleGoals()
        {
            possibleGoalsDirty = false;

            if (goalBlacklist.Count > 0)
            {
                possibleGoals = new List<GOAPGoal<T, W>>(goals.Count);
                double time = gameTime.TotalGameTime.TotalSeconds;

                foreach (var goal in goals)
                {
                    if (!goalBlacklist.ContainsKey(goal)) { possibleGoals.Add(goal); }
                    else if (goalBlacklist[goal] < time)
                    {
                        goalBlacklist.Remove(goal);
                        possibleGoals.Add(goal);
                    }

                }

            }

            else { possibleGoals = goals; }


        }


        protected virtual bool CalculateNewGoal(bool forceStart = false)
        {
            if (IsPlanning) { return false; }

            double time = gameTime.TotalGameTime.TotalSeconds;

            if (! forceStart && (time - lastCalculationTime <= calculationDelay))
            {
                return false;
            }


            lastCalculationTime = gameTime.TotalGameTime.TotalSeconds;
            interruptOnNextTransition = false;

            UpdatePossibleGoals();

            startedPlanning = true;

            currentPlanWorker = GOAPPlannerManager<T, W>.instance.Plan(this, blackListGoalIfFailed ? currentGoal : null, currentGoal != null ? currentGoal.GetPlan() : null, OnDonePlanning);
            return true;

        }

        protected virtual void OnDonePlanning(GOAPGoal<T,W> newGoal)
        {
            startedPlanning = false;
            currentPlanWorker = default(GOAPPlanWork<T, W>);
            if (newGoal == null)
            {
                return;
            }

            if (currentActionState != null)
            {
                currentActionState.action.Exit(null);
            }

            currentActionState = null;
            currentGoal = newGoal;

            if (startingPlan != null)
            {
                for (int i = 0; i < startingPlan.Count; i++)
                {
                    startingPlan[i].action.PlanExit(i > 0 ? startingPlan[i - 1].action : null, i + 1 < startingPlan.Count ? startingPlan[i + 1].action : null, startingPlan[i].settings, currentGoal.GetGoalState());

                }


            }

            startingPlan = currentGoal.GetPlan().ToList();
            ClearPlanValues();
            for (int i = 0; i < startingPlan.Count; i++)
            {
                startingPlan[i].action.PlanEnter(i > 0 ? startingPlan[i - 1].action : null, i + 1 < startingPlan.Count ? startingPlan[i + 1].action : null, startingPlan[i].settings, currentGoal.GetGoalState());


            }

            currentGoal.Run(WarnGoalEnd);
            PushAction();


        }


        public virtual void WarnActionEnd(IGOAPAction<T, W > thisAction)
        {
            if (currentActionState != null && thisAction != currentActionState.action)
            {
                return;
            }

            PushAction();
        }

        protected virtual void PushAction()
        {
            if (interruptOnNextTransition)
            {
                CalculateNewGoal();
                return;
            }

            var plan = currentGoal.GetPlan();
            if (plan.Count == 0)
            {
                if (currentActionState != null)
                {
                    currentActionState.action.Exit(currentActionState.action);
                    currentActionState = null;
                }

                CalculateNewGoal();

            }
            else
            {
                var previous = currentActionState;
                currentActionState = plan.Dequeue();
                IGOAPAction<T, W> next = null;
                if (plan.Count>0)
                {
                    next = plan.Peek().action;
                }
                if (previous != null)
                {
                    previous.action.Exit(currentActionState.action);

                }

                currentActionState.action.Run(previous != null ? previous.action : null, next, currentActionState.settings, currentGoal.GetGoalState(), WarnActionEnd, WarnActionFailure);


            }


        }




        public virtual void WarnActionFailure(IGOAPAction<T, W > thisAction)
        {
            if (blackListGoalIfFailed)
            {
                goalBlacklist[currentGoal] = (float) gameTime.TotalGameTime.TotalSeconds + currentGoal.GetErrorDelay();
            }
            CalculateNewGoal(true);
        }

      
        protected virtual void TryWarnActionFailure(IGOAPAction<T,W> action)
        {
            if (action.isInterruptable())
            {
                WarnActionFailure(action);
            }

            else { action.AskForInterruption(); }


        }


        public virtual void WarnGoalEnd(GOAPGoal<T, W > goal)
        {
            CalculateNewGoal();
        }

        public virtual void WarnPossibleGoal(GOAPGoal<T, W> goal)
        {
            if ((currentGoal != null) && (goal.GetPriority() <= currentGoal.GetPriority()))
            {
                return;
            }

            if (currentActionState != null && !currentActionState.action.isInterruptable())
            {
                interruptOnNextTransition = true;
                currentActionState.action.AskForInterruption();

            }
            else { CalculateNewGoal(); }


        }


       public virtual List<GOAPActionState<T, W>> GetStartingPlan() { return startingPlan; }
        protected virtual void ClearPlanValues()
        {
            if (planValues == null)
            {
                planValues = new Dictionary<T, W>();
            }
            else { planValues.Clear(); }
        }

        public virtual W GetPlanValue(T key) { return planValues[key]; }

        public virtual bool HasPlanValue(T key) { return planValues.ContainsKey(key); }

        public virtual void SetPlanValue(T key, W value) { planValues[key] = value;  }

        public virtual void RefreshMemory() { memory = GetComponent<GOAPMemory<T, W>>(); }
        public virtual void RefreshGoalsSet()
        {
            goals = new List<GOAPGoal<T, W>>(GetComponents<GOAPGoal<T, W>>());
            possibleGoalsDirty = true;


        }


        public virtual void RefreshActionsSet()
        {
            actions = new List<IGOAPAction<T, W>>(GetComponents<IGOAPAction<T, W>>());

        }



        public List<GOAPGoal<T, W>> GetGoalsSet() 
        {
            if (possibleGoalsDirty)
            {
                UpdatePossibleGoals();
            }
            return possibleGoals;
        }

        public List<IGOAPAction<T, W >> GetActionsSet() { return actions; }
        public virtual GOAPMemory<T, W > GetMemory() { return memory; }
        public virtual GOAPGoal<T, W> GetCurrentGoal() { return currentGoal; }
        public virtual GOAPState<T, W> InstantiateNewState() { return GOAPState<T, W>.Instantiate(); }


    }
}
