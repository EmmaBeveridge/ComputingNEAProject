using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Traits
{
    class TraitLazy:Trait
    {
        public new static string TraitString = "Lazy"; 
        public new static int DBID;

        /// <summary>
        /// Constructor to create new lazy trait 
        /// </summary>
        public TraitLazy()
        {
            needsPrioritised.Add(NeedNames.Sleep);
            needsAcceleratedDepletion.Add(NeedNames.Sleep);
        }


        /// <summary>
        /// Returns DBID for TraitLazy class 
        /// </summary>
        /// <returns></returns>
        public override int GetID() { return DBID; }


    }
}
