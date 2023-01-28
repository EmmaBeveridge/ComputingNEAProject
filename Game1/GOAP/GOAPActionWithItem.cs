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


        public GOAPActionWithItem(ActionAbstract _action)
        {


            Action = _action;
            Name = _action.Name;



        }

        public GOAPActionWithItem(ActionAbstract _action, int cost) : this(_action)
        {
            this.Cost = cost;
        }

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
