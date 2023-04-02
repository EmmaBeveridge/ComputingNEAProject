using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.AStar
{
    /// <summary>
    /// Interface for positions on map on which A* algorithm is performed. 
    /// </summary>
    public interface IMapPosition
    {
        float Cost(IMapPosition parent);
        bool Equals(IMapPosition b);



    }
}
