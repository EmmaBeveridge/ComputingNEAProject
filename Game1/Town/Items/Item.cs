using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Game1.Rooms;
using Microsoft.Xna.Framework.Graphics;

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


        public Vector3 townLocation;

        public float rotation;


        public Room room;


        

        public Avatar avatar;
        public Model model;



       public void GenerateAvatar()
       {

            //Matrix rotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(rotation));

            //townLocation = room.house.townLocation + new Vector3(int.Parse(locationX), 0, int.Parse(locationZ));
            //Matrix translationMatrix = Matrix.CreateTranslation(townLocation);

            //avatar = new Avatar(model, townLocation);


            //avatar.worldMatrix =  rotationMatrix * translationMatrix;

           

            Matrix rotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(rotation));


            Matrix relativeTransformation = Matrix.CreateTranslation(new Vector3(int.Parse(locationX), 0, int.Parse(locationZ))) * rotationMatrix;
            
            townLocation = room.house.townLocation ;
            Matrix translationMatrix = Matrix.CreateTranslation(townLocation);

            avatar = new Avatar(model, townLocation);
            
            


            avatar.worldMatrix = relativeTransformation * translationMatrix;
            
            





        }

        


        public Item(string argId, int[] argLocation, string argItemClass)
        {

            id = argId;
            itemClass = argItemClass;

            

        }


       





    }
}
