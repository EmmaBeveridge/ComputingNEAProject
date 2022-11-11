using Microsoft.Xna.Framework;
using Newtonsoft.Json;
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
        public float Cost; ///esitmate so high accuracy not necessary

        public Item item;

        public ActionAbstract Action;


        //public bool WaitToStart = false;

        internal HashSet<Tuple<string, bool>> PreConditions = new HashSet<Tuple<string, bool>>();

        internal HashSet<Tuple<string, bool>> PostConditions = new HashSet<Tuple<string, bool>>();

        public Queue<People> doingAction = new Queue<People>();
        public GOAPAction()
        { }


        public GOAPAction(ActionAbstract _action)
        {


            Action = _action;
            Name = _action.Name;

            
               
        }


        public void UpdateCost(People person, Dictionary<NeedNames, Need> needs)
        {
            Cost = (float) Math.Sqrt(Math.Pow(MathHelper.Distance(person.position.X, item.townLocation.X), 2) + Math.Pow(MathHelper.Distance(person.position.Z, item.townLocation.Z), 2));

            var selectedNeed = from need in needs
                               where need.Key == Action.NeedAffected
                               select need.Value;

            Cost += Action.Cost(selectedNeed.FirstOrDefault<Need>());

           

            if (doingAction.FirstOrDefault()!=person)
            {
                Cost += Action.EstTimeToFinish + Action.Duration*(doingAction.Count-1);
            }

            
        }


        public GOAPAction(ActionAbstract _action, int cost) : this(_action)
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

            //check if item is available herre???


           

            return true;

            
        }

       


        public override string ToString()
        {
            return $"[Action] {this.Name} - cost: {this.Cost}";
        }

    }
}


