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

        [JsonProperty("townLocationX")]
        public string townLocationX { set { townLocation.X = int.Parse(value); } }

        [JsonProperty("townLocationZ")]
        public string townLocationZ { set { townLocation.Z = int.Parse(value); } }

        [JsonProperty("class")]
        public string buildingClass;


        public Vector3 townLocation;

        public float rotation;


        public Street street;


        public Building()
        {

        }

        



    }
}
