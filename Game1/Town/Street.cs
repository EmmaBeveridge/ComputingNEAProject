using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Game1.Town
{
    public class Street
    {
        [JsonProperty("id")]
        public string id;

        //[JsonProperty("directionX")]
        //public string directionX;

        //[JsonProperty("directionZ")]
        //public string directionZ;


        public Vector3 End = new Vector3();

        [JsonProperty("end")]
        public Neo4j.Driver.Point endPoint { set { End.X = (float)value.X; End.Z = (float)value.Y; } }


        public Vector3 Start = new Vector3();

        [JsonProperty("start")]
        public Neo4j.Driver.Point startPoint { set { Start.X = (float)value.X; Start.Z = (float)value.Y; } }

        /// <summary>
        /// Angle of the street to the horizontal
        /// </summary>
        [JsonProperty("rotation")]
        public float rotation;


        /// <summary>
        /// List of adjoining children streets 
        /// </summary>
        public List<Street> children = new List<Street>();
        /// <summary>
        /// Adjoining parent street. 
        /// </summary>
        public Street parent;
        public List<House> houses;
        public List<Building> buildings;


        /// <summary>
        /// Finds perpendicular vector clockwise(left) from street vector
        /// </summary>
        /// <returns>perpendicular clockwise/left  vector</returns>
        public Vector3 FindPerpendicularVector()
        {
            //https://gamedev.stackexchange.com/questions/70075/how-can-i-find-the-perpendicular-to-a-2d-vector

            Vector3 direction = End - Start;

            Vector3 normal = new Vector3(direction.Z, 0, -direction.X);
            normal.Normalize();
            return normal ;
        }

        /// <summary>
        ///  Returns coordinates of closest point on street to point supplied as parameter. Works by projecting vector from start to point onto street direction vector, the length of this projection indicates the point on the street that is closest to the specified point (as projected as right angle to street direction vector). Finds length of projection by dividing the dot product of the vector from street start to point and the street direction vector by the magnitude of the street direction vector. If this projection has length less than 0, closest point is start point of street; if length of the projection is greater than 1, the closest point is the end point of the street. Otherwise, the closest point is the start point plus the direction vector of the street multiplied by the length of the projection. 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector3 FindClosestPointOnStreet(Vector3 point)
        {
            //uses projection formula explained here : https://www.cuemath.com/geometry/projection-vector/

            Vector2 point2D = new Vector2(point.X, point.Z);
            Vector2 streetVector = new Vector2(End.X - Start.X, End.Z - Start.Z);
            Vector2 startToPoint = new Vector2(point2D.X - Start.X, point2D.Y - Start.Z);
            float magnitudeStreet = streetVector.LengthSquared();
            float dot = Vector2.Dot(streetVector, startToPoint);
            float projectionLength = dot / magnitudeStreet;

            if (projectionLength< 0)
            {
                return Start;
            }
            else if (projectionLength > 1)
            {
                return End;
            }
            else
            {
                return Start + (End - Start) * projectionLength;
            }







        }





    }
}
