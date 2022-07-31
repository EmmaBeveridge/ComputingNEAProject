using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Game1.Items
{
    class Bed : Item
    {

        

        public Bed(string argId, int[] argLocation, string argRoomClass) : base(argId, argLocation, argRoomClass)
        {
            modelName = "Bed";

            actionLabels.Add("sleep");
            actionLabels.Add("nap");
            actionLabels.Add("relax");

        }



    }
}
