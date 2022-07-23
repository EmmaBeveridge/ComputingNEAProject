using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Game1.NavMesh
{
    class DataVertex()
    {
        public T data;

        public DataVertex(Vector3 argPosition, int argIndex): base(argPosition, argIndex)
        {
            data = default(T).Default();

        }

        public DataVertex(Vector3 argPosition, int argIndex, T argData):base(argPosition, argIndex)
        {
            data = argData;

        }


        public static implicit operator Vector3(DataVertex<T> vertex)
        {
            return vertex.position;
        }



    }
}
