using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.NavMesh;
using Microsoft.Xna.Framework;

namespace Game1.AStar
{
    public class Path
    {
        public Mesh mesh;
        public float maxScale, minScale;

        NodeCollection nodes;
        AStar<Vertex> pathFinder;
        
        
        public List<LineSegment> obstructingEdges;

        AStar<Vertex> PathFinder
        {
            get { return pathFinder ?? (pathFinder = new AStar<Vertex>(nodes.GetLinks)); }
        }

        public Path()
        {

        }

        public Path(Mesh argMesh)
        {
            mesh = argMesh;
            Initialise();
        }

        private bool LineOfSightCheck(Vector3 a, Vector3 b)
        {
            var centre = (a + b) / 2;

            if (!mesh.Contains(centre))
            {
                return false;
            }

            foreach (LineSegment edge in obstructingEdges)
            {

                if(edge.v1.position.X == 200 && edge.v2.position.X == 350)
                {

                }

                if (edge.Equals(a, b))
                {
                    return true;

                }

                if (edge.Intersects(a, b))
                {
                    return false;
                }



            }

            return true;

        }


        private bool InLineOfSight(Vector3 a, Vector3 b)
        {
            if (!LineOfSightCheck(a, b))
            {
                return false;
            }

            foreach (var vertex in mesh.vertices)
            {
                if (vertex.position != a && vertex.position !=b && LineSegment.Contains(a, b, vertex.position))// if on line segment but don't define segment 
                {
                    if (!LineOfSightCheck( vertex.position, a) || !LineOfSightCheck(vertex.position, b))
                    {
                        return false;
                    }

                }

            }

            return true;

        }

        protected void Initialise()
        {
            obstructingEdges = new List<LineSegment>();
            List<LineSegment> sharedEdges = new List<LineSegment>();

            foreach (var outerTriangle in mesh.triangles)
            {
                foreach (var outerEdge in outerTriangle.edges)
                {
                    if (!obstructingEdges.Any(e => e == outerEdge))
                    {
                        obstructingEdges.Add(outerEdge);
                    }
                    
                }
                
                foreach (var innerTriangle in mesh.triangles) 
                {
                    if (innerTriangle != outerTriangle)
                    {
                        var sharedEdge = outerTriangle.GetSharedEdge(innerTriangle);
                        if (sharedEdge != null && !sharedEdges.Any(e => e == sharedEdge)) 
                        {
                            sharedEdges.Add(sharedEdge);
                        }


                    }
                }

            }

            obstructingEdges.RemoveAll(o => sharedEdges.Any(s => s == o));

            //foreach (LineSegment edge in obstructingEdges)
            //{
            //    //Console.WriteLine(edge.v1.position.ToString() + "    " + edge.v2.position.ToString() );
            //}


            nodes = new NodeCollection();

            foreach (var point in mesh.vertices)
            {
                int triangleCount = mesh.triangles.Where(t => t.HasVertex(point.position)).Count();
                int edgeCount = sharedEdges.Where(e => e.ContainsVertex(point.position)).Count();

                if (triangleCount != edgeCount)
                {
                    nodes.Add(point);
                }

            }

            nodes.CalculateStaticLinks(InLineOfSight);
        }


        public bool LineObstructed(Vector3 from, Vector3 to, out Vector3 position)
        {
            position = Vector3.Zero;
            if (mesh == null||from == to)
            {
                return false;
            }

            List<Vector3> points = new List<Vector3>();
            Vector3 intersection;

            foreach (LineSegment edge in obstructingEdges)
            {
                if (edge.Equals(from, to))
                {
                    return false;
                }

                if (edge.Intersects(from, to, out intersection)) 
                {
                    points.Add(intersection);
                }

            }

            if (points.Count == 0)
            {
                return false;
            }

            int smallestIndex = -1;
            float shortestDistance = float.MaxValue;

            for (int i = 0; i < points.Count; i++)
            {
                float tempDistance = (points[i] - from).LengthSquared();
                if (tempDistance < shortestDistance)
                {
                    shortestDistance = tempDistance;
                    smallestIndex = i;
                }


            }

            position = points[smallestIndex];
            return true;

        }

        public void FindPath(Vector3 from, Vector3 to, ref List<Vector3> result)
        {
            result.Clear();

            if (mesh == null)
            {
                result.Add(from);
                result.Add(to);
                return;
            }


            //foreach (var triangle in mesh.triangles)
            //{
            //    Console.WriteLine($"A:{triangle.A.position.ToString()}\nB:{triangle.B.position.ToString()}\nC:{triangle.C.position.ToString()}\n\n");
            //}

            //for (int i = 0; i < mesh.triangles.Length; i++)
            //{
            //    Console.WriteLine($"\"tri{i}\":{{\"A\":[{mesh.triangles[i].A.position.X} , {mesh.triangles[i].A.position.Z}] , \"B\":[{mesh.triangles[i].B.position.X} , {mesh.triangles[i].B.position.Z}] ,\"C\":[{mesh.triangles[i].C.position.X} , {mesh.triangles[i].C.position.Z}] }},");
            //}


            Triangle triangleStart = mesh.GetClosestTriangle(from);
            Triangle triangleEnd = mesh.GetClosestTriangle(to);

            var firstPoint = triangleStart.GetClosestPoint(from);
            var lastPoint = triangleEnd.GetClosestPoint(to);

            

            if (firstPoint == lastPoint)
            {
                result.Add(firstPoint);
                return;
            }

            if (triangleStart == triangleEnd || InLineOfSight(firstPoint, lastPoint))
            {
                result.Add(firstPoint);
                result.Add(lastPoint);
                return;
            }

            
            List<Vertex> searchResult = new List<Vertex>();


            nodes.CalculateDynamicLinks(firstPoint, lastPoint, InLineOfSight);

            PathFinder.FindPath(firstPoint, lastPoint, ref searchResult, Vertex.Heuristic);

            if (searchResult.Count == 0)
            {
                return;
            }

            Console.WriteLine("SearchResult" + searchResult.Count);

            foreach (Vertex vertex in searchResult)
            {
                result.Add(vertex.position);
                Triangle tri = mesh.GetTriangleAt(vertex.position);
                Console.WriteLine(tri.ToString());
            }

            Console.WriteLine("Result length" + result.Count);

        }


        public Triangle GetClosestTriangle (Vector3 position)
        {
            if (mesh == null)
            {
                return null;
            }
            return mesh.GetClosestTriangle(position);

        }

        public Vector3 GetClosestPoint(Vector3 position)
        {
            if (mesh == null)
            {
                return Vector3.Zero;
            }

            return mesh.GetClosestTriangle(position).GetClosestPoint(position);
        }









    }
}
