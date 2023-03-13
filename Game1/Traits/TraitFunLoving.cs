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


        public TraitFunLoving()
        {
            needsPrioritised.Add(NeedNames.Fun);
            needsAcceleratedDepletion.Add(NeedNames.Fun);
        }

        public override int GetID() { return DBID; }


    }
}
