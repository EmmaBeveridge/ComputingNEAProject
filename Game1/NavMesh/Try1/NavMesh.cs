using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.NavMesh
{
    class NavMesh
    {
        public List<NavMeshGeometry> geometry;
        public List<Pylon> pylons;
        public List<FloodFillNode> fillNodes;
        public List<NavMeshVertex> vertices;
        public List<NavMeshVertex> verticesCopy;
        public int vertexCount;
        public List<NavMeshVertex> edges;
        public List<NavMeshVertex> realEdges;
        public List<Portal> portals;
        public List<Portal> edgeList;
        public List<NavMeshVertex> verticesAddedByPortalCreation;
        public List<Vector3> centrePointsList;

        public NavMesh()
        {
            pylons = new List<Pylon>();
            fillNodes = new List<FloodFillNode>();
            vertices = new List<NavMeshVertex>();
            verticesCopy = new List<NavMeshVertex>();
            edges = new List<NavMeshVertex>();
            realEdges = new List<NavMeshVertex>();
            portals = new List<Portal>();
            edgeList = new List<Portal>();
            verticesAddedByPortalCreation = new List<NavMeshVertex>();
            centrePointsList = new List<Vector3>(); 

        }



        public void BuildNavMeshGeometry(List<List<Vector2>> argGeometry)
        {

            geometry = new List<NavMeshGeometry>();
            foreach (List<Vector2> shapePosScale in argGeometry)
            {

                geometry.Add(new NavMeshGeometry(new Vector3(shapePosScale[0].X, 0, shapePosScale[0].Y), new Vector3(shapePosScale[1].X, 0, shapePosScale[1].Y)));

            }


        }

        public void BeginFilling()
        {
            if (pylons.Count > 0)
            {
                fillNodes.Clear();

                Queue<FloodFillNode> fillNodesQueue = new Queue<FloodFillNode>();
                FloodFillNode fillNode = new FloodFillNode();
                fillNode.position = pylons[0].position;

                fillNodesQueue.Enqueue(fillNode);
                fillNodes.Add(fillNode);
                int count = 0;
                while (fillNodesQueue.Count > 0)
                {
                    ExpandFiller(fillNodesQueue.Dequeue(), fillNodesQueue);
                    count++;
                }


            }


        }




        public NavMeshVertex FindOuterEdgeVertex (List<NavMeshVertex> vertexList)
        {
            NavMeshVertex vertex = null;
            if (vertexList.Count > 0)
            {
                vertex = vertexList[0];
                foreach(NavMeshVertex compVertex in vertexList)
                {
                    if (vertex.position.X < compVertex.position.X)
                    {
                        vertex = compVertex;
                    }
                }
            }

            return vertex;

        }


        public NavMeshVertex FindVertexAtPosition(Vector3 position)
        {
            List<NavMeshVertex> matchVertices = vertices.Where(v => Vector3.DistanceSquared(position, v.position) < 0.1f).ToList();
            
            if (matchVertices.Count > 0)
            {
                return matchVertices.First();

            }
            else
            {
                return null;
            }
            
            
        }


        public List<NavMeshVertex> FindAllVerticesAtPosition(Vector3 position)
        {
            List<NavMeshVertex> matchVertices = vertices.Where(v => Vector3.DistanceSquared(position, v.position) < 0.1f).ToList();
            return matchVertices;

        }

        public void CreateEdges()
        {
            realEdges.Clear();
            verticesCopy.Clear();
            verticesCopy.AddRange(vertices);
            vertexCount = 0;

            Vector3 vector1 = Vector3.Zero;
            Vector3 vector2 = Vector3.Zero;
            float dotProduct = 0;


            while (verticesCopy.Count > 0)
            {
                NavMeshVertex start = verticesCopy[0];
                realEdges.Add(start);

                NavMeshVertex next = start;
                NavMeshVertex previous = null;
                NavMeshVertex previousPrevious = null;

                while (next != null)
                {
                    verticesCopy.Remove(next);
                    vertexCount++;
                    previousPrevious = previous;

                    if (next != null && previous != null)
                    {
                        previous.nextVertex = next;

                    }

                    previous = next;
                    next = null;

                    foreach (NavMeshVertex vertex in previous.linkedVertices)
                    {
                        if (verticesCopy.Contains(vertex))
                        {
                            next = vertex;
                            next.previousVertex = previous;
                        }
                    }

                }


                previous.nextVertex = start;
                start.previousVertex = previous;

            }

        }



      





    }
}
