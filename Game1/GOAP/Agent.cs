using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GOAP
{
    public abstract class Agent
    {
        /// <summary>
        /// Stack of GOAPActions for agent to execute 
        /// </summary>
        public Stack<GOAPAction> Actions;
        protected ActionPlanner Planner;

        /// <summary>
        /// Calls Plan() method on ActionPlanner field of class inheriting abstract Agent class using current world state and goal world state stored for Agent.  
        /// </summary>
        /// <param name="debugPlan"></param>
        /// <returns>Returns if agent now has action plan using HasActionPlan method</returns>
        public bool Plan(bool debugPlan = false)
        {
            List<GOAPNode> nodes = null;

            if (debugPlan)
            {
                nodes = new List<GOAPNode>();
            }


            

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

        /// <summary>
        /// Returns if agent has an action plan I.e. if Actions attribute is not null and has a count greater than 0. 
        /// </summary>
        /// <returns></returns>
        public bool HasActionPlan()
        {
            return this.Actions != null && this.Actions.Count > 0;
        }


        /// <summary>
        /// Abstract method returning the current world state
        /// </summary>
        /// <returns>The world state.</returns>
        public abstract WorldState GetWorldState();


        /// <summary>
        /// Abstract method returning the goal world state. 
        /// </summary>
        /// <param name="needToFulfill"> reference param, need determined as needing to be fulfilled</param>
        /// <returns>Goal state agent wishes to achieve</returns>
        public abstract WorldState GetGoalState(ref NeedNames needToFulfill);

        /// <summary>
        /// Abstract method returning the goal world state. 
        /// </summary>
        /// <returns></returns>
        public abstract WorldState GetGoalState();
    }
}

