using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GOAP
{
    public class GOAPActionState<T, W>
    {
        public IGOAPAction<T, W> action;
        public GOAPState<T, W> settings;


        public GOAPActionState(IGOAPAction<T, W> argAction, GOAPState<T, W> argSettings)
        {
            action = argAction;
            settings = argSettings;
        }



    }
}
