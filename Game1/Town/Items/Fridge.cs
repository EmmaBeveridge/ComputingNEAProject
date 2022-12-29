using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.Actions;
using Game1.GOAP;

namespace Game1.Items
{
    class Fridge:Item
    {


        

        public Fridge(string argId, int[] argLocation, string argRoomClass) : base(argId, argLocation, argRoomClass)
        {
            modelName = "Fridge";
            DefineActions();

        }


        public override void DefineActions()
        {
            EatFromFridgeAction eatFromFridge = new EatFromFridgeAction(this);
            GOAPAction eatFromFridgeGOAP = eatFromFridge.DefineGOAPAction();
            GOAPActions.Add(eatFromFridgeGOAP);
            actionLabels.Add("eat from fridge", eatFromFridgeGOAP);
        }





    }
}
