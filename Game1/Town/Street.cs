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


        Vector3 direction = new Vector3();

        [JsonProperty("direction")]
        public Neo4j.Driver.Point dir { set { direction.X = (float)value.X; direction.Z = (float)value.Y; } }


        Vector3 TownLocation;

        [JsonProperty("townLocation")]
        public Neo4j.Driver.Point townLoc { set { TownLocation.X = (float)value.X; TownLocation.Z = (float)value.Y; } }


        [JsonProperty("rotation")]
        public float rotation;
        
        [JsonProperty("length")]
        public float length;


        public List<Street> children = new List<Street>();
        public Street parent;
        public List<House> houses;
        public List<Building> buildings;



    }
}
