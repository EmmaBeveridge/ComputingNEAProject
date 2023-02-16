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


        public GOAPActionWithBuilding(ActionAbstract _action)
        {


            Action = _action;
            Name = _action.Name;



        }

        public GOAPActionWithBuilding(ActionAbstract _action, int cost) : this(_action)
        {
            this.Cost = cost;
        }

        public override void UpdateCost(People person, Dictionary<NeedNames, Need> needs)
        {
            Cost = (float)Math.Sqrt(Math.Pow(MathHelper.Distance(person.position.X, building.TownLocation.X), 2) + Math.Pow(MathHelper.Distance(person.position.Z, building.TownLocation.Z), 2)); ;

            


        }


    }
}
