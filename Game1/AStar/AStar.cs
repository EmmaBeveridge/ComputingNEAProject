using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Game1.AStar
{
    class AStar<T> : IComparer<AStarNode<T>> where T : IMapPosition
    {

        PriorityQueue<AStarNode<T>> open;
        List<AStarNode<T>> closed = new List<AStarNode<T>>();
        Func<T, IEnumerable<T>> GetNeighbours;

        public AStar(Func<T, IEnumerable<T>> argGetNeighbours)
        {
            GetNeighbours = argGetNeighbours;
            open = new PriorityQueue<AStarNode<T>>(this);
        }


        public void FindPath(T start, T goal, ref List<T> resultNodes, Func<T, T, float> heuristic = null)
        {
            resultNodes.Clear();
            open.Clear();
            closed.Clear();
            
            if (GetNeighbours== null)
            {
                return;
            }

            var heuristicResult = HeuristicResult(heuristic, start, goal);
            AStarNode<T> currentNode = new AStarNode<T>(0, heuristicResult, start, start);
            

            open.Push(currentNode);

            while (open.Count > 0)
            {
                currentNode = open.Pop();
                closed.Add(currentNode);

                if (currentNode.position.Equals(goal))
                {
                    closed.Add(currentNode);
                    PrepareResultNodes(ref resultNodes);
                    return;
                }

                foreach (var nextNode in GetNeighbours((T)currentNode.position))
                {
                    var isClosed = false;
                    foreach (var node in closed)
                    {
                        if (node.position.Equals(nextNode))
                        {
                            isClosed = true;
                            break;
                        }

                    }

                    if (isClosed)
                    {
                        continue;
                    }

                    var newCostFromStart = currentNode.costFromStart + nextNode.Cost(currentNode.position);

                    var index = -1;


                    for (int i = 0; i < open.Count; i++)
                    {

                        if (open[i].position.Equals(nextNode))
                        {
                            index = i;
                            break;
                        }

                    }

                    if (index==-1 || (index>-1 && newCostFromStart < open[index].costFromStart))
                    {
                        heuristicResult = HeuristicResult(heuristic, nextNode, goal);
                        AStarNode<T> newNode = new AStarNode<T>(newCostFromStart, heuristicResult, nextNode, currentNode.position);

                        if (index == -1)
                        {
                            open.Push(newNode);
                        }
                        else
                        {
                            open[index] = newNode;
                        }


                    }


                }


            }

            return;

        }

        private float HeuristicResult(Func<T, T, float> heuristic, T currentNode, T goalNode)
        {
            return heuristic == null ? 0 : heuristic(currentNode, goalNode);
        }

        private void PrepareResultNodes(ref List<T> resultNodes)
        {
            var goal = closed.Last();

            for (int i = closed.Count - 1; i >= 0; i--) //removes nodes from closed list thatare not in path by comparing parent position, at each iteration goal becomes next node on path starting from the end
            {
                if (goal.parentPosition.Equals(closed[i].position) || i == closed.Count - 1) 
                {
                    goal = closed[i];
                }

                else
                {
                    closed.RemoveAt(i); //if not on path, remove node
                }

            }


            foreach (AStarNode<T> node in closed)
            {
                resultNodes.Add((T)node.position);
            }

        }

        public int Compare(AStarNode<T> a, AStarNode<T> b)
        {
            if (a.totalEstimatedCost > b.totalEstimatedCost)
            {
                return 1;
            }
            else if (a.totalEstimatedCost < b.totalEstimatedCost)
            {
                return -1;
            }

            return 0;
        }
        








    }
    
}
