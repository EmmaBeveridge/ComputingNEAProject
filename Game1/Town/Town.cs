﻿using System;
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

        static public List<Vector3> corners = new List<Vector3> { new Vector3(maxX, 0, maxZ), new Vector3(maxX, 0, minZ), new Vector3(minX, 0, minZ), new Vector3(minX, 0, maxZ) };
        static public Mesh navMesh;
        public List<District> districts = new List<District>();
        public List<House> houses = new List<House>();
        public List<Building> buildings = new List<Building>();


        public List<GOAPAction> GOAPActions = new List<GOAPAction>();

    }
}
