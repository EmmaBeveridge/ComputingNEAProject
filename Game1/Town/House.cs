using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.Rooms;
using Game1.DataClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Game1.NavMesh;

namespace Game1.Town

{
    public class House
    {
        [JsonProperty("id")]
        public string id;

        [JsonProperty("colourScheme")]
        public string colourScheme;

        [JsonProperty("townLocationX")]
        public string townLocationX { set { townLocation.X = int.Parse(value); } }

        [JsonProperty("townLocationZ")]
        public string townLocationZ { set { townLocation.Z = int.Parse(value); } }

        [JsonProperty("side")]
        public string side;


        public Vector3 townLocation;


        public List<Room> rooms;
        
        
        public float rotation;


        public Street street;

        public List<Vector3> corners = new List<Vector3>();
        public static Mesh navMesh;


        public string interiorWallsName;
        public string exteriorWallsName;

        public Model interiorWallsModel;
        public Model exteriorWallsModel;

        public List<Avatar> wallAvatars = new List<Avatar>();


        public void GenerateAvatar()
        {

            if (side == "left")
            {
                rotation += 180;
            }

            Matrix rotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(rotation));


            Matrix translationMatrix = Matrix.CreateTranslation(townLocation);

            wallAvatars.Add(new Avatar(exteriorWallsModel, townLocation));
            wallAvatars.Add(new Avatar(interiorWallsModel, townLocation));

            wallAvatars.ForEach(delegate (Avatar a) { a.worldMatrix = rotationMatrix * translationMatrix; } );

            


        }



        public void SetCorners()
        {
            //Regex regex = new Regex(@"T\d+\.R\.S\d\.(\d\.)*H\d+");
            Regex regex = new Regex(@"H\d+$");
            var match = regex.Match(id);
            DataTable coordTable = ExcelFileManager.ReadCoordinates(match.Value);

            foreach (DataRow row in coordTable.Rows)
            {
                corners.Add(new Vector3(int.Parse(row["X"].ToString()), 0, int.Parse(row["Z"].ToString())));
            }
            {

            }
            
        }

        public bool inHouse(Vector3 position)
        {
            
            int intersectionCount = 0;
            for (int i = 0; i < corners.Count; i++)
            {
                Vector3 v1 = corners[i];
                Vector3 v2 = corners[i + 1 % corners.Count];

                LineSegment edge = new LineSegment(v1, v2);
                Vector3 intersection = new Vector3();
                if (edge.Intersects(position, position + Vector3.UnitX, out intersection, true))
                {
                    intersectionCount++;
                }


            }

            if (intersectionCount % 2 == 0) { return false; }
            return true;
            
        }



        public void setWallsFromColourScheme()
        {
            switch (colourScheme)
            {
                case "brown":
                    interiorWallsName = "HouseInteriorWallsBrown";
                    exteriorWallsName = "HouseExteriorWallsBrown";
                    break;
                default:
                    interiorWallsName = "HouseInteriorWallsBrown";
                    exteriorWallsName = "HouseExteriorWallsBrown";
                    break;

            }
        }



        


        public House()
        {

        }



        public House(List<Room> argRooms, float argRotation)
        {
            rooms = argRooms;
            rotation = argRotation;


        }




    }
}
