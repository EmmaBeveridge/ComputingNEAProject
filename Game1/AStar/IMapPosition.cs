using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.AStar
{
    public interface IMapPosition
    {
        float Cost(IMapPosition parent);
        bool Equals(IMapPosition b);



    }
}
