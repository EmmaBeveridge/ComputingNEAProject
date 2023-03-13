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


        public TraitSociable()
        {
            needsPrioritised.Add(NeedNames.Social);
            needsAcceleratedDepletion.Add(NeedNames.Social);
        }

        public override int GetID() { return DBID; }

    }
}
