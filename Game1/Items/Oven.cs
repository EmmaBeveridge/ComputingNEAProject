using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Items
{
    class Oven:Item
    {

        
        public Oven(string argId, int[] argLocation, string argRoomClass) : base(argId, argLocation, argRoomClass)
        {
            modelName = "Oven";


        }

    }
}
