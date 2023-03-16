using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Careers
{
    class OfficeWorker:Career
    {
        public static string Name = "office worker";
        public static string Description = "Coffee, copiers, and cubicles";
        public override string CareerName { get { return Name; } }
        public override string CareerDescription { get { return Description; } }



        public OfficeWorker() : base() { }


    }
}
