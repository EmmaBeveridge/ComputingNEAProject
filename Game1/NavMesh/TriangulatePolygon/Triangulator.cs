using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.IO;
using System.Data;
using Game1.DataClasses;

namespace Game1.NavMesh.TriangulatePolygon
{
    class Triangulator
    {
        static string PolygonVerticesFileName = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName + "/Data/Polygon data.xlsx";
        static public List<Vector2> shapeVertices = new List<Vector2>();
        static public List<Vector2> holeVertices = new List<Vector2>();

        static public List<Vertex> resultVertices;
        static public int[] indices;


        static readonly CircularLinkedListIndexed<Vertex> polygonVertices = new CircularLinkedListIndexed<Vertex>();
        static readonly CircularLinkedListIndexed<Vertex> earVertices = new CircularLinkedListIndexed<Vertex>();
        static readonly CircularList<Vertex> convexVertices = new CircularList<Vertex>();
        static readonly CircularList<Vertex> reflexVertices = new CircularList<Vertex>();

        const int yValue = 0;





        public static void BuildTriangles()
        {

            GetVerticesFromFile();
            Vector2[] newVertices = CutHoleInShape(shapeVertices.ToArray(), holeVertices.ToArray());

            Triangulate(newVertices);
           
        }


        private static void GetVerticesFromFile()
        {
            DataTable sheet = ExcelFileManager.ReadExcelFile(PolygonVerticesFileName);
            foreach (DataRow row in sheet.Rows)
            {
                if (row["HoleBool"].ToString() == "0")
                {
                    shapeVertices.Add(new Vector2(int.Parse(row["X"].ToString()), int.Parse(row["Z"].ToString())));

                }
                else
                {
                    holeVertices.Add(new Vector2(int.Parse(row["X"].ToString()), int.Parse(row["Z"].ToString())));
                }

            }
            //Console.WriteLine("Shape vertices");
            //foreach (var item in shapeVertices)
            //{
            //    Console.WriteLine(item.ToString());
            //}

            //Console.WriteLine("Hole vertices");

            //foreach (var item in holeVertices)
            //{
            //    Console.WriteLine(item.ToString());
            //}
        }


        public static void Triangulate(Vector2[] inputVertices)//Vector2 used where Y = Z
        {



            List<Triangle> triangles = new List<Triangle>();
            //uses anticlockwise winding order

            Vector2[] outputVertices = (Vector2[])inputVertices.Clone();

            polygonVertices.Clear();
            earVertices.Clear();
            convexVertices.Clear();
            reflexVertices.Clear();


            for (int i = 0; i < outputVertices.Length; i++)
            {
                polygonVertices.AddLast(new Vertex(new Vector3(outputVertices[i].X, yValue, outputVertices[i].Y), i));
            }
            
            resultVertices = polygonVertices.ToList();


            FindConvexAndReflexVertices();
            FindEarVertices();

            while (polygonVertices.Count > 3 && earVertices.Count > 0)
            {
                ClipNextEar(triangles);
            }


            if (polygonVertices.Count == 3)
            {
                triangles.Add(new Triangle(polygonVertices[0].Value, polygonVertices[1].Value, polygonVertices[2].Value));
            }

            indices = new int[triangles.Count * 3];
            for (int i = 0; i < triangles.Count; i++)
            {

                indices[(i * 3)] = triangles[i].A.index;
                indices[(i * 3) + 1] = triangles[i].B.index;
                indices[(i * 3) + 2] = triangles[i].C.index;

            }

            
        }


        public static Vector2[] CutHoleInShape(Vector2[] shapeVertices, Vector2[] holeVertices)
        {
            //shape vertices wound counterclockwise, hole vertices wound clockwise

            polygonVertices.Clear();
            earVertices.Clear();
            convexVertices.Clear();
            reflexVertices.Clear();

            for (int i = 0; i < shapeVertices.Length; i++)
            {

                polygonVertices.AddLast(new Vertex(new Vector3(shapeVertices[i].X, yValue, shapeVertices[i].Y), i));

            }


            CircularList<Vertex> holePolygon = new CircularList<Vertex>();

            for (int i = 0; i < holeVertices.Length; i++)
            {
                holePolygon.Add(new Vertex(new Vector3(holeVertices[i].X, yValue, holeVertices[i].Y), i));

            }

            FindConvexAndReflexVertices();
            FindEarVertices();


            Vertex holeVertexMaxX = holePolygon[0];
            foreach (Vertex vertex in holePolygon)
            {
                if (vertex.position.X> holeVertexMaxX.position.X)
                {
                    holeVertexMaxX = vertex;
                }
            }


            List<LineSegment> segmentsToTest = new List<LineSegment>();

            //builds list of segments where at least one vertex had an x value greater than biggest x value of hole vertices and where one vertex has greater z value and one a smaller z value than hole vertex

            for (int i = 0; i < polygonVertices.Count; i++)
            {

                Vertex a = polygonVertices[i].Value;
                Vertex b = polygonVertices[i + 1].Value;

                if ((a.position.X > holeVertexMaxX.position.X || b.position.X > holeVertexMaxX.position.X) && ((a.position.Z >= holeVertexMaxX.position.Z && b.position.Z <= holeVertexMaxX.position.Z) || (a.position.Z <= holeVertexMaxX.position.Z && b.position.Z >= holeVertexMaxX.position.Z))) 
                {
                    segmentsToTest.Add(new LineSegment(a, b));

                }

            }



            float? shortestDistance = null;
            LineSegment closestSegment = new LineSegment();

            foreach (LineSegment segment in segmentsToTest)
            {
                Vector3 intersection;
                bool intersects = segment.Intersects(holeVertexMaxX.position, holeVertexMaxX.position+Vector3.UnitX, out intersection, true); //ERROR: SENDING DIRECTION VECTOR UNITX, NOT POS VECTOR -- MAKE ANOTHER INTERSECTION METHOD??
                if (intersects)
                {
                    float distance = (intersection - holeVertexMaxX.position).Length();

                    if (shortestDistance == null || distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        closestSegment = segment;
                    }
                }
            }

            if (shortestDistance == null)
            {
                return shapeVertices;
            }

            Vector3 I = holeVertexMaxX.position + Vector3.UnitX * shortestDistance.Value;
            Vertex P = (closestSegment.v1.position.X> closestSegment.v2.position.X) ? closestSegment.v1: closestSegment.v2;



            //https://www.geometrictools.com/Documentation/TriangulationByEarClipping.pdf pg8

            Triangle mip = new Triangle(holeVertexMaxX, new Vertex(I, 1), new Vertex(P.position, P.index));

            List<Vertex> reflexVerticesInmip = new List<Vertex>();

            foreach (Vertex vertex  in reflexVertices)
            {
                if (mip.Encloses(vertex.position))
                {

                    reflexVerticesInmip.Add(vertex);


                }


            }

            if (reflexVerticesInmip.Count > 0)
            {
                float smallestDotProduct = -1f; //smallest dot product => smallest angle
                foreach (Vertex vertex in reflexVerticesInmip)
                {
                    Vector3 d = Vector3.Normalize(vertex.position - holeVertexMaxX.position);
                    float dotProduct = Vector3.Dot(Vector3.UnitX, d);

                    if (dotProduct < smallestDotProduct)
                    {
                        smallestDotProduct = dotProduct;
                        P = vertex;
                    }



                }



            }


            int startHoleIndex = holePolygon.IndexOf(holeVertexMaxX);
            int injectPoint = polygonVertices.IndexOf(P);

            for (int i = startHoleIndex; i < startHoleIndex+holePolygon.Count; i++)
            {
                polygonVertices.AddAfter(polygonVertices[injectPoint++], holePolygon[i]);
            }

            polygonVertices.AddAfter(polygonVertices[injectPoint], P);

            Vector2[] newShapeVertices = new Vector2[polygonVertices.Count];

            for (int i = 0; i < polygonVertices.Count; i++)
            {

                newShapeVertices[i] = new Vector2(polygonVertices[i].Value.position.X, polygonVertices[i].Value.position.Z);


            }


            return newShapeVertices;


        }

        private static void ClipNextEar(ICollection<Triangle> triangles)
        {
            Vertex ear = earVertices[0].Value;
            Vertex previous = polygonVertices[polygonVertices.IndexOf(ear) - 1].Value;
            Vertex next = polygonVertices[polygonVertices.IndexOf(ear) + 1].Value;

            triangles.Add(new Triangle(ear, next, previous));

            earVertices.RemoveAt(0);
            polygonVertices.RemoveAt(polygonVertices.IndexOf(ear));

            ValidateAdjacentVertex(previous);
            ValidateAdjacentVertex(next);


        }


        private static void ValidateAdjacentVertex(Vertex vertex)
        {
            if (reflexVertices.Contains(vertex))
            {
                if (IsConvex(vertex))
                {
                    reflexVertices.Remove(vertex);
                    convexVertices.Add(vertex);
                }
            }

            if (convexVertices.Contains(vertex))
            {
                bool wasEar = earVertices.Contains(vertex);
                bool isEar = IsEar(vertex);

                if (wasEar && !isEar) 
                {
                    earVertices.Remove(vertex);

                }
                else if (!wasEar && isEar) 
                {
                    earVertices.AddFirst(vertex);
                }


            }


        }




        public static void FindConvexAndReflexVertices()
        {
            foreach (Vertex vertex in polygonVertices)
            {
                if (IsConvex(vertex))
                {
                    convexVertices.Add(vertex);
                    Console.WriteLine($"{vertex.position.ToString()}: convex");
                    

                }
                else
                {
                    reflexVertices.Add(vertex);
                    Console.WriteLine($"{vertex.position.ToString()}: reflex");

                }
            }
        }


        private static bool IsConvex(Vertex vertex)
        {
            Vertex previous = polygonVertices[polygonVertices.IndexOf(vertex) - 1].Value;
            Vertex next = polygonVertices[polygonVertices.IndexOf(vertex) + 1].Value;

            Vector2 vertexPos = new Vector2(vertex.position.X, vertex.position.Z);
            Vector2 prevPos = new Vector2(previous.position.X, previous.position.Z);
            Vector2 nextPos = new Vector2(next.position.X, next.position.Z);

            Vector2 d1 = Vector2.Normalize(vertexPos - prevPos);
            Vector2 d2 = Vector2.Normalize(nextPos - vertexPos);
            Vector2 n2 = new Vector2(-d2.Y, d2.X);

            return (Vector2.Dot(d1, n2) > 0f);




        }

        private static bool IsReflex (Vertex vertex)
        {
            return !IsConvex(vertex);
        }


        public static void FindEarVertices() 
        {
            foreach (Vertex vertex in convexVertices)
            {
                if (IsEar(vertex))
                {
                    earVertices.AddLast(vertex);
                }

            }
        }


        private static bool IsEar(Vertex vertex)
        {
            Vertex previous = polygonVertices[polygonVertices.IndexOf(vertex) - 1].Value;
            Vertex next = polygonVertices[polygonVertices.IndexOf(vertex) + 1].Value;

            foreach (Vertex reflexVertex in reflexVertices)
            {
                if (reflexVertex.Equals(previous) || reflexVertex.Equals(vertex) || reflexVertex.Equals(next))
                {
                    continue;
                }

                if (Triangle.Encloses(previous, vertex, next, reflexVertex))
                {
                    return false;
                }


            }

            return true;



        }





        




    }
}
