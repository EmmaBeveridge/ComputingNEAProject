using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Traits
{
    class TraitSociable:Trait
    {

        public new static string TraitString = "Sociable";
        public new static int DBID;

        /// <summary>
        /// Constructor to create new sociable trait
        /// </summary>
        public TraitSociable()
        {
            needsPrioritised.Add(NeedNames.Social);
            needsAcceleratedDepletion.Add(NeedNames.Social);
        }

        /// <summary>
        /// Returns DBID for TraitSociable class 
        /// </summary>
        /// <returns></returns>
        public override int GetID() { return DBID; }

    }
}
