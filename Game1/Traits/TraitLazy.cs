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


        public TraitLazy()
        {
            needsPrioritised.Add(NeedNames.Sleep);
            needsAcceleratedDepletion.Add(NeedNames.Sleep);
        }



    }
}
