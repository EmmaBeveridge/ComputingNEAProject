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


        public TraitClean()
        {
            needsPrioritised.Add(NeedNames.Hygiene);
            needsDeceleratedDepletion.Add(NeedNames.Hygiene);
        }

        public override int GetID() { return DBID; }


    }
}
