using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Traits
{

    class TraitClean : Trait
    {
        public new static string TraitString = "Clean";
        public new static int DBID;

        /// <summary>
        /// Constructor to create new clean trait 
        /// </summary>
        public TraitClean()
        {
            needsPrioritised.Add(NeedNames.Hygiene);
            needsDeceleratedDepletion.Add(NeedNames.Hygiene);
        }

        /// <summary>
        /// Returns DBID for TraitClean class 
        /// </summary>
        /// <returns></returns>
        public override int GetID() { return DBID; }


    }
}
