using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Game1.NavMesh;

namespace Game1.AStar
{
    public class AStarNode<T>
    {
        public float costFromStart;
        public float heuristicToGoal;
        public float totalEstimatedCost;

        public IMapPosition position;
        public IMapPosition parentPosition;

        public AStarNode(float argCostFromStart, float argHeuristicToGoal, IMapPosition argPosition, IMapPosition argParentPosition)
        {
            costFromStart = argCostFromStart;
            heuristicToGoal = argHeuristicToGoal;
            totalEstimatedCost = costFromStart + heuristicToGoal;

            parentPosition = argParentPosition;
            position = argPosition;



        }






    }
}
