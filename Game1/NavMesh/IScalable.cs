using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.NavMesh
{
    interface IScalable<T>
    {
        T Multiply(float scale);
        T Add(T t);
        T Default();



    }
}
