using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GOAP
{
    public class GOAPGoal<T, W>
    {
        public string name = "genericGoal";
        public float priority = 1;
        public float errorDelay = 0.5f;


        public bool warnPossibleGoal = true;


        protected GOAPState<T, W> goal;
        protected Queue<GOAPActionState<T, W>> plan;
        protected GOAPPlanner<T, W> planner;


        public virtual string GetName() { return name; }
        public virtual float GetPriority() { return priority; }


        public virtual bool IsGoalPossible() { return warnPossibleGoal; }


        public virtual Queue<GOAPActionState<T, W>> GetPlan() { return plan; }

        public virtual GOAPState<T, W> GetGoalState() { return goal; }

        public virtual void SetPlan(Queue<GOAPActionState<T, W>> path) { plan = path; }

        public void Run(Action<GOAPGoal<T, W >> callback)
        {

        }

        public virtual void Precalculations (GOAPPlanner<T, W> argPlanner)
        {
            planner = argPlanner;
        }

        public virtual float GetErrorDelay() { return errorDelay; }



       



    }
}
