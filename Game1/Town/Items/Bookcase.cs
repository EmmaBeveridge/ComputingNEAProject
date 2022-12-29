using Game1.Actions;
using Game1.GOAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Items
{
    class Bookcase : Item
    {

        

        public Bookcase(string argId, int[] argLocation, string argRoomClass) : base(argId, argLocation, argRoomClass)
        {
            modelName = "Bookcase";
            DefineActions();
        }


        public override void DefineActions()
        {
            ReadBookAction readbook = new ReadBookAction(this);
            GOAPAction readbookGOAP = readbook.DefineGOAPAction();
            GOAPActions.Add(readbookGOAP);
            actionLabels.Add("read book", readbookGOAP);
        }




    }
}