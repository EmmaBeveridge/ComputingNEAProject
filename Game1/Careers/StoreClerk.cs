using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Careers
{
    class StoreClerk:Career
    {
        public static string Name = "store clerk";
        public static string Description = "Serve customers at the\ngeneral store";
        public override string CareerName { get { return Name; } }
        public override string CareerDescription { get { return Description; } }



        public StoreClerk():base(){ }


    }
}
