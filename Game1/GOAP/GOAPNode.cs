using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GOAP
{
    public class GOAPNode : IComparable<GOAPNode>, IEquatable<GOAPNode>
    {
        /// <summary>
        /// The state of the world at this node.
        /// </summary>
        public WorldState WorldState;

        /// <summary>
        /// The cost so far. Cost to reach node from start state. 
        /// </summary>
        public float CostSoFar;

        /// <summary>
        /// The heuristic for remaining cost (don't overestimate!). Estimated cost to reach the final state. 
        /// </summary>
        public int HeuristicCost;

        /// <summary>
        /// costSoFar + heuristicCost (g+h) combined.
        /// </summary>
        public float CostSoFarAndHeuristicCost;

        /// <summary>
        /// the Action associated with this node
        /// </summary>
        public GOAPAction Action;

        // Where did we come from?
        public GOAPNode Parent;
        public WorldState ParentWorldState;
        public int Depth;

       

        /// <summary>
        /// Returns if WorldState for instance of GOAPNode and GOAPNode supplied as parameter are equal using WOrldState.Equals() method. 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(GOAPNode other)
        {
            return (this.WorldState.Equals(other.WorldState));


            //var care = this.WorldState.DontCare ^ -1L;
            ///return (this.WorldState.Values & care) == (other.WorldState.Values & care);
        }


        /// <summary>
        /// Compares CostSoFarAndHeuristicCost for GOAPNode instance to CostSoFarAndHeuristicCost for GOAPNode supplied as parameter.
        /// </summary>
        /// <param name="other"></param>
        /// <returns> If instance is less than parameter, returns –1; if instance if greater than parameter, returns 1; if instance and parameter are equal, returns 0.</returns>
        public int CompareTo(GOAPNode other)
        {
            return this.CostSoFarAndHeuristicCost.CompareTo(other.CostSoFarAndHeuristicCost);
        }



        /// <summary>
        ///  Resets the GOAPNode by setting Action and Parent parameters to null. 
        /// </summary>
        public void Reset()
        {
            this.Action = null;
            this.Parent = null;
        }


        /// <summary>
        /// Returns a shallow copy of GOAPNode
        /// </summary>
        /// <returns></returns>
        public GOAPNode Clone()
        {
            return (GOAPNode)this.MemberwiseClone();
        }


        /// <summary>
        /// Returns string representation of GOAPNode.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[cost: {this.CostSoFar} | heuristic: {this.HeuristicCost}]: {this.Action}";
        }
    }
}
