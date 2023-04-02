using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.Town.Districts;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using Game1.NavMesh;
using Game1.GOAP;


namespace Game1.Town
{
    public class Town
    {

        [JsonProperty("id")]
        public string id;

        static int maxX = 4000;
        static int maxZ = 4000;
        static int minX = -4000;
        static int minZ = -4000;

        /// <summary>
        /// List of vectors containing the maximum and minimum bounds of the world. Bounds used in navmesh generation. 
        /// </summary>
        static public List<Vector3> corners = new List<Vector3> { new Vector3(maxX, 0, maxZ), new Vector3(maxX, 0, minZ), new Vector3(minX, 0, minZ), new Vector3(minX, 0, maxZ) };

        /// <summary>
        /// Static property on Town class storing the navmesh for the town defining walkable areas outside the house. 
        /// </summary>
        static public Mesh navMesh;
        /// <summary>
        /// List of districts within the town
        /// </summary>
        public List<District> districts = new List<District>();
        /// <summary>
        /// List of houses within the town.
        /// </summary>
        public List<House> houses = new List<House>();
        /// <summary>
        /// List of buildings within the town.
        /// </summary>
        public List<Building> buildings = new List<Building>();

        /// <summary>
        /// Contains list of all available actions in the town and their GOAP properties. 
        /// </summary>
        public List<GOAPAction> GOAPActions = new List<GOAPAction>();

        /// <summary>
        /// Constructor for new town object 
        /// </summary>
        public Town() { }

    }
}
