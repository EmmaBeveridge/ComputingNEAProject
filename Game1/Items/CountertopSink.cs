using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Items
{
    class CountertopSink:Sink
    {

        

        public CountertopSink(string argId, int[] argLocation, string argRoomClass) : base(argId, argLocation, argRoomClass)
        {
            modelName = "CountertopSink";

        }



    }
}
