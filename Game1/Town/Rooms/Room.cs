using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Game1.Town;
namespace Game1.Rooms
{
    public class Room
    {
        public List<Item> items = new List<Item>();
        
        [JsonProperty ("id")]
        public string id;


        [JsonProperty ("relativeLocationX")]
        string locationX;

        [JsonProperty("relativeLocationZ")]
        public string locationZ;



        [JsonProperty ("class")]
        public string roomClass;


        public House house;


        public Room( string argId, int[] argLocation, string argRoomClass)
        {
            
            id = argId;
            
            roomClass = argRoomClass;
        }







    }
}
