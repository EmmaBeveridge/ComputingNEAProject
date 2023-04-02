using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.UI;

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
        /// Emotional states leading to positive work scores
        /// </summary>
        public static List<PeopleEmotionalState> positiveWorkEmotions = new List<PeopleEmotionalState> { PeopleEmotionalState.Comfortable, PeopleEmotionalState.Playful, PeopleEmotionalState.Energised };

        /// <summary>
        /// Emotional states leading to negative work scores
        /// </summary>
        public static List<PeopleEmotionalState> negativeWorkEmotions = new List<PeopleEmotionalState> { PeopleEmotionalState.Uncomfortable, PeopleEmotionalState.Tense, PeopleEmotionalState.Lonely };


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



        /// <summary>
        /// Static method. Returns an increment value for work score using person's emotional state. If their emotional state is considered positive, returns 1. If negative, returns -1. If neutral, returns 0.
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public static int ReturnWorkScoreIncrement(People person)
        {

            if (positiveWorkEmotions.Contains(person.emotionalState)) { return 1; }
            if (negativeWorkEmotions.Contains(person.emotionalState)) { return -1; }
            return 0;

        }





    }
}
