using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Traits
{
    public class Trait
    {

        public Trait()
        {

        }



        /// <summary>
        /// Returns list of all needs that are prioritised by the specified traits
        /// </summary>
        /// <param name="traits">List of person's trait</param>
        /// <returns></returns>
        public static List<NeedNames> GetNeedsPrioritised(List<Trait> traits)
        {
            return new List<NeedNames>();
        }


        /// <summary>
        /// Returns trait object given string name of trait
        /// </summary>
        /// <param name="TraitString">string name of trait</param>
        /// <returns></returns>

        public static Trait GetTraitFromString (string TraitString)
        {
            return null;
        }





    }
}
