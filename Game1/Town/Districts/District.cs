using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;

namespace Game1.Town.Districts
{
    public class District
    {

        [JsonProperty("id")]
        public string id;

        [JsonProperty("townLocation")]
        public Neo4j.Driver.Point townLoc { set { TownLocation.X = (float)value.X; TownLocation.Z = (float)value.Y; } }


        public Vector3 TownLocation;

        public string districtClass;

        public string mapName;

        public List<Street> streets;

        public Model mapModel;

        public Avatar avatar;


        public void GenerateAvatar()
        {

           
            avatar = new Avatar(mapModel, TownLocation);


           



        }

        public District() { }



    }
}
