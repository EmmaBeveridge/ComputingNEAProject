using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.Rooms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        [JsonProperty("leftSide")]
        public bool side;


        public Vector3 TownLocation;

        public float rotation;


        public Street street;


        public string modelName;

        public Model model;

        public Avatar avatar;

        public static List<Building> buildings = new List<Building>();

        public BoundingBox buildingBox;


        public Building()
        {
            modelName = "Station";
        }

        public void GenerateAvatar()
        {

            if (side)
            {
                rotation += 180;
            }

            Matrix rotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(rotation));


            Matrix translationMatrix = Matrix.CreateTranslation(TownLocation);

            Matrix worldMatrix = rotationMatrix * translationMatrix;

            avatar = new Avatar(model, TownLocation);
            avatar.worldMatrix = worldMatrix;


            buildingBox = avatar.UpdateBoundingBox();



        }




    }
}
