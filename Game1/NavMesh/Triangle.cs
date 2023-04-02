using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Game1.NavMesh
{
    public class Triangle
    {
        public int id { get; private set; }
        public Vertex A { get; private set; }
        public Vertex B { get; private set; }
        public Vertex C { get; private set; }

        public LineSegment AB { get; private set; }
        public LineSegment BC { get; private set; }
        public LineSegment CA { get; private set;  }

        public LineSegment[] edges { get; private set; }


        float s1, s2, s3, s4, denominator;
        float triangleArea;
        Vector3 boundsLT, boundsRB;



        public Triangle()
        {

        }

        //public Triangle(Vertex argA, Vertex argB, Vertex argC, int argID = -1)
        //{
        //    A = argA;
        //    B = argB;
        //    C = argC;
        //    id = argID;

        //    Cache();

        //}

        public Triangle(Vertex argA, Vertex argB, Vertex argC, int argID = -1)
        {
            A = argA;
            B = argB;
            C = argC;
            id = argID;

            Cache();

        }


        /// <summary>
        ///  Returns if the triangle has an edge that can be considered equal to the line segment supplied as a parameter
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public bool HasEdge(LineSegment edge)
        {
            return AB == edge || BC == edge || CA == edge;
        }

        /// <summary>
        /// Returns if the triangle has a vertex at a position equal to the supplied vector
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool HasVertex(Vector3 point)
        {
            return A.position == point || B.position == point || C.position == point;
        }




        /// <summary>
        ///  Returns edge common to triangle object and triangle supplied as parameter 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public LineSegment GetSharedEdge(Triangle other)
        {
            if (HasEdge(other.AB))
            {
                return other.AB;
            }
            else if (HasEdge(other.BC))
            {
                return other.BC;
            }
            else if (HasEdge(other.CA))
            {
                return other.CA;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///  Calculates mathematical attributes of triangle and creates and assigns line segment objects for edges.
        /// </summary>
        void Cache()
        {
            AB = new LineSegment(A, B);
            BC = new LineSegment(B, C);
            CA = new LineSegment(C, A);

            s1 = (B.position.Y - C.position.Y);
            s2 = (A.position.X - C.position.X);
            s3 = (C.position.X - B.position.X);
            s4 = (C.position.Y - A.position.Y);


            denominator = (s1 * s2 + s3 * (A.position.Y - C.position.Y));

            float angle = (float)Math.Acos(Vector3.Dot(CA.direction, -BC.direction) / (CA.length * BC.length));
            // negative BC as vectors point out from point c

            triangleArea = 0.5f * CA.length * BC.length * (float) Math.Sin(angle);

            edges = new LineSegment[] { AB, BC, CA };

            boundsLT.X = Math.Min(Math.Min(A.position.X, B.position.X), C.position.X);
            boundsLT.Z = Math.Min(Math.Min(A.position.Z, B.position.Z), C.position.Z);
            boundsRB.X = Math.Max(Math.Max(A.position.X, B.position.X), C.position.X);
            boundsRB.Z = Math.Max(Math.Max(A.position.Z, B.position.Z), C.position.Z);




        }

        /// <summary>
        /// Static method, calculating area enclosed by triangle formed by three vector points supplied as parameters.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static float Area(Vector3 a, Vector3 b, Vector3 c)
        {


            Vector3 CA = a - c;
            Vector3 BC = c - b;

            float angle = (float)Math.Acos(Vector3.Dot(CA, -BC) / (CA.Length() * BC.Length()));


            float triangleArea = 0.5f * CA.Length() * BC.Length() * (float)Math.Sin(angle);

            return triangleArea;



        }


        //public T GetVertexData(Vector3 p)
        //{
        //    if (!Encloses(p))
        //    {
        //        throw new InvalidOperationException("Point not within triangle");

        //    }



        //    var a = Area(B.position, p, C.position);
        //    var b = Area(C.position, p, A.position);
        //    var c = Area(A.position, p, B.position);

        //    return A.data.Multiply(a / triangleArea).Add(B.data.Multiply(b / triangleArea)).Add(C.data.Multiply(c / triangleArea));


        //}



        public static bool Encloses(Vertex a, Vertex b, Vertex c, Vertex point)
        {
            Triangle testTriangle = new Triangle(a, b, c);
            return testTriangle.Encloses(point.position);
        }




        //public bool Encloses(Vector3 point)
        //{
        //    if (point.X < boundsLT.X || point.Z < boundsLT.Z || point.X > boundsRB.X || point.Z > boundsRB.Z)
        //    {
        //        return false;
        //    }

        //    if (HasVertex(point))
        //    {
        //        return true;
        //    }

        //    if (InConvexHull(point))
        //    {
        //        return true;
        //    }

        //    return false;


        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Encloses(Vector3 point)
        {
            //https://blackpawn.com/texts/pointinpoly/default.html????

            if (point.X < boundsLT.X || point.Z < boundsLT.Z || point.X > boundsRB.X || point.Z > boundsRB.Z)
            {
                return false;
            }

            // Point lies on a vertex
            if (HasVertex(point))
            {
                return true;
            }


            float Denominator = CalculateDenominator();

            // Triangle having zero area -> two points equal
            if (Denominator == 0)
            {
                return AB.Contains(point) || BC.Contains(point);
            }

            var a = LineSegment.EpsilonUnitInterval((s1 * (point.X - C.position.X) + s3 * (point.Z - C.position.Z)) / Denominator);
            var b = LineSegment.EpsilonUnitInterval((s4 * (point.X - C.position.X) + s2 * (point.Z - C.position.Z)) / Denominator);
            var c = LineSegment.EpsilonUnitInterval(1 - a - b);

            return (0 <= a && a <= 1 && 0 <= b && b <= 1 && 0 <= c && c <= 1);


            //List<Vector3> corners = new List<Vector3>();
            //corners.Add(A.position);
            //corners.Add(B.position);
            //corners.Add(C.position);

            //int intersectionCount = 0;
            //for (int i = 0; i < corners.Count; i++)
            //{
            //    Vector3 v1 = corners[i];
            //    Vector3 v2 = corners[(i + 1) % corners.Count];

            //    LineSegment edge = new LineSegment(v1, v2);
            //    Vector3 intersection = new Vector3();
            //    if (edge.Intersects(point, point + Vector3.UnitX, out intersection, true))
            //    {
            //        intersectionCount++;
            //    }


            //}

            //if (intersectionCount % 2 == 0) { return false; }
            //return true;










        }







        public float CalculateDenominator()
        {
            s1 = B.position.Z - C.position.Z;
            s2 = A.position.X - C.position.X;
            s3 = C.position.X - B.position.X;
            s4 = C.position.Z - A.position.Z;
            float denominator = s1 * s2 + s3 * (A.position.Z - C.position.Z);
            return denominator;

        }

        /// <summary>
        ///  Returns if the point is in the convex hull of the triangle by determining if the point can be expressed as C + alpha*CB + beta*CA where alpha and beta are positive values which sum to one or less 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool InConvexHull(Vector3 point)
        {


            //can express point = C + alpha* CB + beta*CA?

            Vector3 vecCB = -BC.direction;
            Vector3 vecCA = CA.direction;

            float alpha = (Determinant(point, vecCA) - Determinant(C.position, vecCA)) / Determinant(vecCB, vecCA);
            float beta = -(Determinant(point, vecCB) - Determinant(C.position, vecCB)) / Determinant(vecCB, vecCA);

            if (alpha>0 && beta>0 && alpha + beta <= 1)
            {
                return true;

            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// static method returns value of the determinant of the augmented matrix formed by the 2 vector parameters. 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Determinant(Vector3 a, Vector3 b)
        {
            return a.X * b.Z - a.Z * b.X;
        }


        /// <summary>
        /// Returns point in triangle that is closes to the specified point. If the point is contained within the triangle, the point itself is returned. If the point is not contained in the triangle, the edges of the triangle are iterated over and for each the closest point on the edge is found using GetClosestPointOnEdge method of line segment objects – the closest of these is returned as closest point in triangle. 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector3 GetClosestPoint(Vector3 point)
        {
            float shortestDistance = float.PositiveInfinity;
            Vector3 closestPoint = Vector3.Zero;



            if (Encloses(point))
            {
                return point;
            }


            foreach (LineSegment edge in edges)
            {
                Vector3 tempClosest = edge.GetClosestPointOnEdge(point);
                float tempShortest = Math.Abs((tempClosest - point).Length());

                if (tempShortest < shortestDistance)
                {
                    shortestDistance = tempShortest;
                    closestPoint = tempClosest;
                }


            }

            return closestPoint;





        }



        public override string ToString()
        {
            return $"A: {A.position.ToString()} B:{B.position.ToString()} C:{C.position.ToString()}";
        }










    }
}
