using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Careers
{
    class Politician : Career
    {
        public static string Name = "politician";
        public static string Description = "The exciting world of local government awaits";
        public override string CareerName { get { return Name; } }
        public override string CareerDescription { get { return Description; } }



        public Politician() : base() { }


    }
}
