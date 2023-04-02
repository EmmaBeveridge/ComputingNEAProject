using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Traits
{
    class TraitGourmand : Trait
    {
        public new static string TraitString = "Gourmand";
        public new static int DBID;

        /// <summary>
        /// Constructor to create new gourmand trait 
        /// </summary>
        public TraitGourmand()
        {
            needsPrioritised.Add(NeedNames.Hunger);
            needsAcceleratedDepletion.Add(NeedNames.Hunger);
        }

        /// <summary>
        /// Returns DBID for TraitGourmand class 
        /// </summary>
        /// <returns></returns>
        public override int GetID() { return DBID; }

    }
}
