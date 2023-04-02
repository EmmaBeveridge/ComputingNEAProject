using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GOAP
{
    class GOAPActionWithBuilding:GOAPAction
    {

        /// <summary>
        /// Constructor for new GOAPActionWithBuilding object. 
        /// </summary>
        /// <param name="_action"></param>
        public GOAPActionWithBuilding(ActionAbstract _action)
        {


            Action = _action;
            Name = _action.Name;



        }


        /// <summary>
        /// Constructor for new GOAPActionWithBuilding object. 
        /// </summary>
        /// <param name="_action"></param>
        /// <param name="cost"></param>
        public GOAPActionWithBuilding(ActionAbstract _action, int cost) : this(_action)
        {
            this.Cost = cost;
        }


        /// <summary>
        ///  Overrides method in parent class. Estimated cost of completing the action is calculated as the Euclidean distance from the person’s town location to the building’s town location.
        /// </summary>
        /// <param name="person"></param>
        /// <param name="needs"></param>
        public override void UpdateCost(People person, Dictionary<NeedNames, Need> needs)
        {
            Cost = (float)Math.Sqrt(Math.Pow(MathHelper.Distance(person.position.X, building.TownLocation.X), 2) + Math.Pow(MathHelper.Distance(person.position.Z, building.TownLocation.Z), 2)); ;

            


        }


    }
}
