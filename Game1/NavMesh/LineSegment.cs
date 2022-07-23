using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Game1.NavMesh
{
    public class LineSegment
    {
        public Vertex v1 { get; private set; }
        public Vertex v2 { get; private set; }

        public Vector3 direction;


        public float length;


        public LineSegment() { }


        public LineSegment(Vector3 argV1, Vector3 argV2)
        {
            v1 = new Vertex(argV1);
            v2 = argV2;
            length = (v1.position - v2.position).Length();
            direction = v2.position - v1.position;
        }


        public LineSegment (Vertex argV1, Vertex argV2)
        {
            v1 = argV1;
            v2 = argV2;
            length = (v1.position - v2.position).Length();
            direction = v2.position - v1.position;
        }




        public bool ContainsVertex(Vector3 vertex)
        {

            return (v1.position == vertex || v2.position == vertex);
        }



        public bool Intersects( Vector3 a, Vector3 b)
        {
            Vector3 intersection;
            return Intersects(a, b, out intersection);
        }



        public bool Intersects(Vector3 a, Vector3 b, out Vector3 intersection, bool vectorInfinite)//vectorInfinite refers to ab not linesegment  
        {
            intersection = Vector3.Zero;

            float lambda, mu;

            Vector3 posVector1 = a;
            Vector3 dirVector1 = b - a;

            Vector3 posVector2 = v1.position;
            Vector3 dirVector2 = v2.position - v1.position;




            if (Vector3.Cross(dirVector1, dirVector2).LengthSquared() == 0)
            {
                return false; //lines parallel
            }

            //https://harry7557558.github.io/Graphics/numerical/tools/Graphics_Gems_1,ed_A.Glassner.pdf pg 304


            lambda = Vector3.Dot(Vector3.Cross((posVector2 - posVector1), dirVector2), Vector3.Cross(dirVector1, dirVector2)) / (Vector3.Cross(dirVector1, dirVector2).LengthSquared());

            mu = Vector3.Dot(Vector3.Cross((posVector1 - posVector2), dirVector1), Vector3.Cross(dirVector2, dirVector1)) / (Vector3.Cross(dirVector2, dirVector1).LengthSquared());

            if (!vectorInfinite)
            {
                if (lambda > 1 || lambda < 0)
                {

                    return false;

                }
            }

            if (mu > 1 || mu < 0)
            {
                return false;
            }



            intersection = posVector1 + lambda * dirVector1;

            return true;




        }



        public bool Intersects (Vector3 a, Vector3 b, out Vector3 intersection)
        {
            return Crosses(new Vector2(a.X, a.Z), new Vector2(b.X, b.Z), out intersection);
        }








        public bool Crosses(Vector2 a, Vector2 b, out Vector3 intersection)
        {
            intersection = Vector3.Zero;

            Vector2 c = new Vector2(v1.position.X, v1.position.Z);
            Vector2 d = new Vector2(v2.position.X, v2.position.Z);
            

            float denominator = ((b.X - a.X) * (d.Y - c.Y)) - ((b.Y - a.Y) * (d.X - c.X));

            if (denominator == 0)
            {
                return false;
            }

            float numerator1 = ((a.Y - c.Y) * (d.X - c.X)) - ((a.X - c.X) * (d.Y - c.Y));
            float numerator2 = ((a.Y - c.Y) * (b.X - a.X)) - ((a.X - c.X) * (b.Y - a.Y));

            if (numerator1 == 0 || numerator2 == 0)
            {
                return false;
            }

            float r = EpsilonUnitInterval(numerator1 / denominator);
            float s = EpsilonUnitInterval(numerator2 / denominator);

            intersection = new Vector3(a.X + (b.X - a.X) * r, 0, a.Y + (b.Y - a.Y) * s);

            return (r > 0 && r < 1) && (s > 0 && s < 1);
        }

        const float EPSILON = 0.00001f;

        public static float EpsilonUnitInterval(float a)
        {
            if (NearlyEqual(a, 1f))
            {
                return 1f;
            }

            if (NearlyEqual(a, 0f))
            {
                return 0f;
            }

            return a;
        }

        public static bool NearlyEqual(float a, float b)
        {
            var Diff = Math.Abs(b - a);

            if (b == a)
            {
                return true;
            }
            else if (b == 0 || a == 0 || Diff < float.Epsilon)
            {
                return Diff < EPSILON;
            }
            else
            {
                return Diff / (b + a) < EPSILON;
            }
        }


        public bool Contains(Vector3 c)
        {
            var partsLen = (v1 - c).Length() + (c - v2).Length();
            return NearlyEqual(length, partsLen);
        }


        public static bool Contains (Vector3 a, Vector3 b, Vector3 c)
        {
            //returns if segment ab contains point c

            return AlmostEqual((a - c).Length() + (c - b).Length(), (a - b).Length());

        }

        public Vector3 GetClosestPointOnEdge(Vector3 p)
        {

            Vector3 vectorV1 = v1.position;
            Vector3 vectorV2 = v2.position;



            Vector3 direction = vectorV2 - vectorV1;



            //calculate lambda by rearranging dot product =0
            float lambda = (vectorV1.X * (p.X - vectorV1.X) + vectorV1.Y * (p.Y - vectorV1.Y) + vectorV1.Z * (p.Z - vectorV1.Z)) / (direction.X * vectorV1.X + direction.Y * vectorV1.Y + direction.Z * vectorV1.Z);
            if (lambda <= 0)
            {
                return vectorV1;
            }
            else if (lambda >= 1)
            {
                return vectorV2;
            }
            else
            {
                return vectorV1 + lambda * direction;
            }


        }
        

        public static bool AlmostEqual(float a, float b) //uses % diff to determine is nearly equal 
        {
            const float allowablePercentageDifference = 0.0001f;
            var difference = Math.Abs(b - a);
            
            if (b == a)
            {
                return true;
            }
            else if (a == 0 || b == 0 || difference < float.Epsilon)
            {
                return difference < allowablePercentageDifference;
            }
            else
            {
                return (difference / ((b + a) / 2) < allowablePercentageDifference);
            }




        }

        public bool Equals (Vector3 a, Vector3 b)
        {
            return ((a == v1.position && b == v2.position) || (a == v2.position && b == v1.position));
        }


        public static bool operator == (LineSegment t1, LineSegment t2)
        {
            if (object.ReferenceEquals(t1, null))
            {
                return object.ReferenceEquals(t2, null);
            }

            if (object.ReferenceEquals(t2, null))
            {
                return false;
            }


            return (((t1.v1 == t2.v1) && (t1.v2 == t2.v2)) ||
            ((t1.v1 == t2.v2) && (t1.v2 == t2.v1)));
        }

        public static bool operator != (LineSegment t1, LineSegment t2)
        {
            return !(t1 == t2);
        }


    }
}
