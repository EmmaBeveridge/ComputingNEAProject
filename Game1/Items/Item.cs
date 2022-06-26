using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Game1
{
    public class Item
    {
        
        [JsonProperty("id")]
        public string id;


        [JsonProperty("relativeLocationX")]
        public string locationX;

        [JsonProperty("relativeLocationZ")]
        public string locationZ;



        [JsonProperty("class")]
        public string itemClass;


      
        public string modelName;


        public Vector3 location;

        


        public Item(string argId, int[] argLocation, string argItemClass)
        {

            id = argId;
            itemClass = argItemClass;

            

        }


        public void SetLocation()
        {
            //Regex getLocationRegex = new Regex(@"\[(?<locationX>\d+)\s*,\s*(?<locationZ>\d+)\]");
            //Match locationMatch = getLocationRegex.Match(locationX);
            //location = new Vector3(int.Parse(locationMatch.Groups["locationX"].Value), 0, int.Parse(locationMatch.Groups["locationZ"].Value));

        }





    }
}
