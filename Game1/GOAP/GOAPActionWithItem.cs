using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GOAP
{
    class GOAPActionWithItem:GOAPAction
    {

        public GOAPActionWithItem()
        { }

        /// <summary>
        /// Constructor for new GOAPActionWithItem object.
        /// </summary>
        /// <param name="_action"></param>
        public GOAPActionWithItem(ActionAbstract _action)
        {


            Action = _action;
            Name = _action.Name;



        }
        /// <summary>
        /// Constructor for new GOAPActionWithItem object.
        /// </summary>
        /// <param name="_action"></param>
        /// <param name="cost"></param>
        public GOAPActionWithItem(ActionAbstract _action, int cost) : this(_action)
        {
            this.Cost = cost;
        }

        /// <summary>
        /// Overrides method in parent class. Estimated cost of completing action calculated as the sum of the Euclidean distance from the person’s town location to the item’s town location, the estimated cost (time) to fulfil the need affected by the value, and the estimated time taken for person to reach front of queue for action by summing the estimated time for the current action to finish and the duration of the action multiplied by the number of the rest of the people in the queue. 
        /// </summary>
        /// <param name="person"></param>
        /// <param name="needs"></param>
        public override void UpdateCost(People person, Dictionary<NeedNames, Need> needs)
        {
            Cost = (float)Math.Sqrt(Math.Pow(MathHelper.Distance(person.position.X, item.townLocation.X), 2) + Math.Pow(MathHelper.Distance(person.position.Z, item.townLocation.Z), 2));

            var selectedNeed = from need in needs
                               where need.Key == Action.NeedAffected
                               select need.Value;

            Cost += Action.Cost(selectedNeed.FirstOrDefault<Need>());



            if (doingAction.FirstOrDefault() != person)
            {
                Cost += Action.EstTimeToFinish + Action.Duration * (doingAction.Count - 1);
            }


        }



    }
}
