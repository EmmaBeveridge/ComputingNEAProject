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


        [JsonProperty("rotation")]
        public float rotation;
        
        

        public List<Street> children = new List<Street>();
        public Street parent;
        public List<House> houses;
        public List<Building> buildings;




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
