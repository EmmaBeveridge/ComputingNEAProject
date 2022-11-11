using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.Rooms;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Game1.Town

{
    public class Building
    {
        [JsonProperty("id")]
        public string id;

        [JsonProperty("townLocation")]
        public Neo4j.Driver.Point townLoc { set { TownLocation.X = (float)value.X; TownLocation.Z = (float)value.Y; } }

        [JsonProperty("class")]
        public string buildingClass;


        public Vector3 TownLocation;

        public float rotation;


        public Street street;


        public Building()
        {

        }

        



    }
}
