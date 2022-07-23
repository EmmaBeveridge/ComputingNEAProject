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



        public bool HasEdge(LineSegment edge)
        {
            return AB == edge || BC == edge || CA == edge;
        }


        public bool HasVertex(Vector3 point)
        {
            return A.position == point || B.position == point || C.position == point;
        }
       




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


        public bool Encloses(Vector3 point)
        {
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

        public static float Determinant(Vector3 a, Vector3 b)
        {
            return a.X * b.Z - a.Z * b.X;
        }



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














    }
}
