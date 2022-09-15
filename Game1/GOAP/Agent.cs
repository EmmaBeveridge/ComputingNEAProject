using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GOAP
{
    public abstract class Agent
    {
        public Stack<GOAPAction> Actions;
        protected ActionPlanner Planner;

       
        public bool Plan(bool debugPlan = false)
        {
            List<GOAPNode> nodes = null;
            if (debugPlan)
                nodes = new List<GOAPNode>();

            this.Actions = this.Planner.Plan(this.GetWorldState(), this.GetGoalState(), nodes);

            if (nodes == null || nodes.Count <= 0)
            {
                return this.HasActionPlan();
            }

            //---- ActionPlanner plan ----
            //plan cost = {nodes[nodes.Count - 1].CostSoFar}
            //start    {this.GetWorldState().Describe(this.Planner)}
            //for ( var i = 0; i < nodes.Count; i++ )
            //{
            //{i}: {nodes[i].Action.GetType().Name}    {nodes[i].WorldState.Describe(this.Planner)}"
            //}

            return this.HasActionPlan();
        }


        public bool HasActionPlan()
        {
            return this.Actions != null && this.Actions.Count > 0;
        }


        /// <summary>
        /// current WorldState
        /// </summary>
        /// <returns>The world state.</returns>
        public abstract WorldState GetWorldState();


        /// <summary>
        /// the goal state that the agent wants to achieve
        /// </summary>
        /// <returns>The goal state.</returns>
        public abstract WorldState GetGoalState();
    }
}

