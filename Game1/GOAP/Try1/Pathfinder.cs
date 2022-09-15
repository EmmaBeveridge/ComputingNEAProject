using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.AStar;

namespace Game1.GOAP
{


    public class nodeComparer<T> : IComparer<INode<T>>
    {
        int IComparer<INode<T>>.Compare(INode<T> a, INode<T> b)
        {
            if (a.GetCost() < b.GetCost()) { return -1; }
            if (a.GetCost() > b.GetCost()) { return 1; }
            return 0;
        }
    }





    public class Pathfinder<T>
    {

        private readonly Dictionary<T, INode<T>> stateToNode;
        private readonly Dictionary<T, INode<T>> explored;
        private readonly List<INode<T>> createdNodes;
        private PriorityQueue<INode<T>> frontier = new PriorityQueue<INode<T>>(new nodeComparer<T>());


        public Pathfinder()
        {
            stateToNode = new Dictionary<T, INode<T>>();
            explored = new Dictionary<T, INode<T>>();
            createdNodes = new List<INode<T>>();
        }



       




        void ClearNodes()
        {
            foreach (var node in createdNodes)
            {
                node.Recycle();

            }
            createdNodes.Clear();
        }


        public INode<T> Run(INode<T> start, T goal, int maxIterations = 100, bool earlyExit = true, bool clearNodes= true)
        {
            frontier.Clear();
            stateToNode.Clear();
            explored.Clear();

            if (clearNodes)
            {
                ClearNodes();

                createdNodes.Add(start);
            }

            frontier.Push(start);

            int iterations = 0;

            while ((frontier.Count > 0) && (iterations < maxIterations)) 
            {

                var node = frontier.Pop();

                if (node.IsGoal(goal))
                {
                    return node;
                }

                explored[node.GetState()] = node;

                foreach (var child in node.Expand())
                {

                    iterations++;

                    if (clearNodes)
                    {
                        createdNodes.Add(child);

                    }

                    if (earlyExit && child.IsGoal(goal))
                    {
                        return child;
                    }



                    var childCost = child.GetCost();
                    var state = child.GetState();

                    if (explored.ContainsKey(state)) { continue; }

                    INode<T> similarNode;
                    stateToNode.TryGetValue(state, out similarNode);

                    if (similarNode != null)
                    {
                        if (similarNode.GetCost() > childCost)
                        {
                            frontier.Remove(similarNode);
                        }
                        else { break; }
                    }

                    frontier.Push(child);
                    stateToNode[state] = child;





                }








            }



            return null;


        }






    }
}
