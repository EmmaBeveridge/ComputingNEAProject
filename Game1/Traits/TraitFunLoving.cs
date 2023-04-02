using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Traits
{
    class TraitFunLoving : Trait
    {
        public new static string TraitString = "FunLoving";
        public new static int DBID;

        /// <summary>
        /// Constructor to create new fun-loving trait 
        /// </summary>
        public TraitFunLoving()
        {
            needsPrioritised.Add(NeedNames.Fun);
            needsAcceleratedDepletion.Add(NeedNames.Fun);
        }

        /// <summary>
        /// Returns DBID for TraitFunLoving class 
        /// </summary>
        /// <returns></returns>
        public override int GetID() { return DBID; }


    }
}
