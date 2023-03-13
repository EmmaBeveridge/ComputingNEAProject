using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Traits
{
    class TraitLoner : Trait
    {
        public new static string TraitString = "Loner";
        public new static int DBID;


        public TraitLoner()
        {
            
            needsDeceleratedDepletion.Add(NeedNames.Social);
        }

        public override int GetID() { return DBID; }


    }
}
