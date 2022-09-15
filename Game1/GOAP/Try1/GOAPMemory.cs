using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GOAP
{
    public class GOAPMemory<T, W>
    {
        protected GOAPState<T, W> state;

        public virtual GOAPState<T, W> GetWorldState() { return state; }




    }
}
