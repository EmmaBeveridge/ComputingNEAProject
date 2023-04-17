using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Careers
{
    /// <summary>
    /// Implements the Teacher career inheriting from Career parent
    /// </summary>
    class Teacher : Career
    {
        public static string Name = "teacher";
        public static string Description = "Inspire the next generation";
        public override string CareerName { get { return Name; } }
        public override string CareerDescription { get { return Description; } }



        public Teacher() : base() { }


    }
}
