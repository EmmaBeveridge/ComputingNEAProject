using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.Town.Districts;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using Game1.NavMesh;

namespace Game1.Town
{
    public class Town
    {

        [JsonProperty("id")]
        public string id;

        static int maxX = 100000;
        static int maxZ = 100000;
        static int minX = -100000;
        static int minZ = -100000;

        static public List<Vector3> corners = new List<Vector3> { new Vector3(maxX, 0, maxZ), new Vector3(maxX, 0, minZ), new Vector3(minX, 0, minZ), new Vector3(minX, 0, maxZ) };
        static public Mesh navMesh;
        public List<District> districts = new List<District>();
        public List<House> houses = new List<House>();

    }
}
