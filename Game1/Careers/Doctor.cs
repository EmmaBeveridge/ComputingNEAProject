using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Careers
{
    class Doctor : Career
    {
        public static string Name = "doctor";
        public static string Description = "It's a beautiful day to save lives";
        public override string CareerName { get { return Name; } }
        public override string CareerDescription { get { return Description; } }



        public Doctor() : base() { }


    }
}
