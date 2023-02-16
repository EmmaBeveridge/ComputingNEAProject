using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Careers
{
    public class Career
    {
        public static string Name;
        public static string Description;
        public Career() { }



        public virtual string CareerName { get { return Name; } }
        public virtual string CareerDescription { get { return Description; } }



        public static Career GetCareerFromString (string careerString)
        {

            switch (careerString.ToLower())
            {
                case "store clerk":
                    return new StoreClerk();
                    break;
                default:
                    return null;
                    break;
            }


        }


    }
}
