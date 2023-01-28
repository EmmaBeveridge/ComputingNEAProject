using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Careers
{
    public class Career
    {
        string Name;

        public Career() { }


        public static Career GetCareerFromString (string careerString)
        {

            switch (careerString.ToLower())
            {
                case "thief":
                    return new Thief();
                    break;
                default:
                    return null;
                    break;
            }


        }


    }
}
