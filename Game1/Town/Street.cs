using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Game1.Town
{
    public class Street
    {
        [JsonProperty("id")]
        public string id;

        [JsonProperty("directionX")]
        public string directionX;

        [JsonProperty("directionZ")]
        public string directionZ;

        [JsonProperty("rotation")]
        public float rotation;
        
        [JsonProperty("length")]
        public string length;


        public List<Street> children;
        public Street parent;
        public List<House> houses;
        public List<Building> buildings;



    }
}
