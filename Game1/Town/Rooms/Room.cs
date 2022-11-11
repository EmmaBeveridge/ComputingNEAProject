using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Game1.Town;
using Game1.GOAP;
namespace Game1.Rooms
{
    public class Room
    {
        public List<Item> items = new List<Item>();
        
        [JsonProperty ("id")]
        public string id;


        Vector3 relativeLocation;
        public Vector3 townLocation = new Vector3();


        [JsonProperty("relativeLocation")]
        public Neo4j.Driver.Point relativeLoc { set { relativeLocation = new Vector3((float) value.X, 0, (float) value.Y); } }

        


        [JsonProperty ("class")]
        public string roomClass;


        public House house;

        public List<GOAPAction> GOAPActions = new List<GOAPAction>();



        public Room( string argId, int[] argLocation, string argRoomClass)
        {
            
            id = argId;
            
            roomClass = argRoomClass;
        }


        public void SetLocation()
        {
            Matrix rotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(house.rotation));


            Matrix relativeTransformation = Matrix.CreateTranslation(new Vector3((float)relativeLocation.X, 0, (float)relativeLocation.Y)) * rotationMatrix;

            Matrix translationMatrix = Matrix.CreateTranslation(house.TownLocation);

            townLocation = (relativeTransformation * translationMatrix).Translation;


        }




    }
}
