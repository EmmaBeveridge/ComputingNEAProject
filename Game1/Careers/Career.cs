using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Careers
{
    public class Career
    {
        /// <summary>
        /// Static field. String name of career.
        /// </summary>
        public static string Name;

        /// <summary>
        /// Static field. String description of career.
        /// </summary>
        public static string Description;

        /// <summary>
        /// Constructor for new career object.
        /// </summary>
        public Career() { }


        /// <summary>
        /// Get method for Name class attribute.
        /// </summary>
        public virtual string CareerName { get { return Name; } }
        /// <summary>
        /// Get method for Description attribute.
        /// </summary>
        public virtual string CareerDescription { get { return Description; } }


        /// <summary>
        /// Returns new child career object of correct derived type given string name of career.
        /// </summary>
        /// <param name="careerString"></param>
        /// <returns></returns>
        public static Career GetCareerFromString (string careerString)
        {

            switch (careerString.ToLower())
            {
                case "store clerk":
                    return new StoreClerk();
                    break;
                case "office worker":
                    return new OfficeWorker();
                    break;
                case "doctor":
                    return new Doctor();
                    break;
                case "teacher":
                    return new Teacher();
                    break;
                default:
                    return null;
                    break;
            }


        }


    }
}
