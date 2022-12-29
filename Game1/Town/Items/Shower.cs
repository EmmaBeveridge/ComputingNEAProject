using Game1.Actions;
using Game1.GOAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Items
{
    class Shower : Item
    {


        public Shower(string argId, int[] argLocation, string argRoomClass) : base(argId, argLocation, argRoomClass)
        {
            modelName = "Shower";
            DefineActions();

        }

        public override void DefineActions()
        {

            TakeShowerAction takeshower = new TakeShowerAction(this);
            GOAPAction takeshowerGOAP = takeshower.DefineGOAPAction();
            GOAPActions.Add(takeshowerGOAP);
            actionLabels.Add("take shower", takeshowerGOAP);

            

        }




    }
}
