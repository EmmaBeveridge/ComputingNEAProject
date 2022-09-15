using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GOAP
{
    public class GOAPAction
    {
        /// <summary>
        /// optional name for the Action. Used for debugging purposes
        /// </summary>
        public string Name;

        /// <summary>
        /// The cost of performing the action: currently uses Euclidean Distance to item only
        /// </summary>
        public int Cost; ///esitmate so high accuracy not necessary

        public Item item;


        internal HashSet<Tuple<string, bool>> PreConditions = new HashSet<Tuple<string, bool>>();

        internal HashSet<Tuple<string, bool>> PostConditions = new HashSet<Tuple<string, bool>>();


        public GOAPAction()
        { }


        public GOAPAction(string name)
        {
            this.Name = name;
        }


        public void UpdateCost(Vector3 personPosition)
        {
            Cost = (int)Math.Sqrt(Math.Pow(MathHelper.Distance(personPosition.X, item.townLocation.X), 2) + Math.Pow(MathHelper.Distance(personPosition.Z, item.townLocation.Z), 2));
        }


        public GOAPAction(string name, int cost) : this(name)
        {
            this.Cost = cost;
        }


        public void SetPrecondition(string conditionName, bool value)
        {
            this.PreConditions.Add(new Tuple<string, bool>(conditionName, value));
        }


        public void SetPostcondition(string conditionName, bool value)
        {
            this.PostConditions.Add(new Tuple<string, bool>(conditionName, value));
        }


        /// <summary>
        /// called before the Planner does its planning. Gives the Action an opportunity to set its score or to opt out if it isnt of use.
        /// eg if action is to get food from fridge but no food in fridge, action is not considered by ActionPlanner
        /// </summary>
        public virtual bool Validate()
        {
            return true;
        }


        public override string ToString()
        {
            return $"[Action] {this.Name} - cost: {this.Cost}";
        }

    }
}


