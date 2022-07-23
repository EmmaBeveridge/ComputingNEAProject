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

        [JsonProperty("townLocationX")]
        public string townLocationX { set { townLocation.X = int.Parse(value); } }

        [JsonProperty("townLocationZ")]
        public string townLocationZ { set { townLocation.Z = int.Parse(value); } }



        public Vector3 townLocation;

        public string districtClass;

        public string mapName;

        public List<Street> streets;

        public Model mapModel;

        public Avatar avatar;


        public void GenerateAvatar()
        {

           
            avatar = new Avatar(mapModel, townLocation);


           



        }

        public District() { }



    }
}
