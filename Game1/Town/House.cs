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
using Game1.GOAP;

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



        public static Mesh navMesh;
        public static List<House> houses = new List<House>();


        public Vector3 townLocation;


        public List<Room> rooms;
        
        
        public float rotation;


        public Street street;

        public List<Vector3> groundCorners = new List<Vector3>();
        public List<Vector3> corners = new List<Vector3>();
        

        public string interiorWallsName;
        public string exteriorWallsName;
        public string roofName;
        public Model interiorWallsModel;
        public Model exteriorWallsModel;
        public Model roofModel;
        public List<Avatar> wallAvatars = new List<Avatar>();

        public Matrix houseToTownTransformation;
        public List<Plane> planes = new List<Plane>();

        public List<GOAPAction> GOAPActions = new List<GOAPAction>(); 


        public void GenerateAvatar()
        {

            if (side == "left")
            {
                rotation += 180;
            }

            Matrix rotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(rotation));


            Matrix translationMatrix = Matrix.CreateTranslation(townLocation);

            houseToTownTransformation = rotationMatrix * translationMatrix;

            wallAvatars.Add(new Avatar(exteriorWallsModel, townLocation));
            wallAvatars.Add(new Avatar(interiorWallsModel, townLocation));
            wallAvatars.Add(new Avatar(roofModel, townLocation));

            wallAvatars.ForEach(delegate (Avatar a) { a.worldMatrix = houseToTownTransformation; } );
            GeneratePlanes();
            


        }
        public void GeneratePlanes()
        {
            SetCorners();
            int cuboidCount = corners.Count / 8;
            
            for (int i = 0; i < cuboidCount; i++)
            {
                planes.Add(new Plane(corners[i * 8], corners[i * 8 + 1], corners[i * 8 + 2]));
                planes.Add(new Plane(corners[i * 8 + 4], corners[i * 8 + 5], corners[i * 8 + 6]));
                planes.Add(new Plane(corners[i * 8], corners[i * 8 + 4], corners[i * 8 + 7]));
                planes.Add(new Plane(corners[i * 8 + 1], corners[i * 8 + 5], corners[i * 8 + 6]));
                planes.Add(new Plane(corners[i * 8], corners[i * 8 + 4], corners[i * 8 + 5]));
                planes.Add(new Plane(corners[i * 8 + 3], corners[i * 8 + 6], corners[i * 8 + 7]));



            }



        }


        public void SetCorners()
        {
            //Regex regex = new Regex(@"T\d+\.R\.S\d\.(\d\.)*H\d+");
            Regex regex = new Regex(@"H\d+$");
            var match = regex.Match(id);
            DataTable coordTable = ExcelFileManager.ReadCoordinates(match.Value, house: true);

            foreach (DataRow row in coordTable.Rows)
            {
                groundCorners.Add(new Vector3(int.Parse(row["X"].ToString()), 0, int.Parse(row["Z"].ToString())));
            }

            coordTable = ExcelFileManager.ReadCoordinates("House", item: true);

            foreach (DataRow row in coordTable.Rows)
            {
                Vector3 houseCoord = new Vector3(int.Parse(row["X"].ToString()), int.Parse(row["Y"].ToString()), int.Parse(row["Z"].ToString()));

                Vector3 townCoord = (Matrix.CreateTranslation(houseCoord) * houseToTownTransformation).Translation;
                corners.Add(townCoord);
            }




        }

        public static House getHouseContainingPoint(Vector3 position)
        {
            foreach (House house in houses)
            {
                if (house.inHouse(position))
                {
                    return house;
                }

            }

            return null;
        }


        public bool inHouse(Vector3 position)
        {


            if (position.Y> corners[0].Y || position.Y< groundCorners[0].Y)
            {
                return false;
            }

            if (groundCorners.Contains(position))
            {
                return true;
            }


            int intersectionCount = 0;
            for (int i = 0; i < groundCorners.Count; i++)
            {
                Vector3 v1 = groundCorners[i];
                Vector3 v2 = groundCorners[(i + 1) % groundCorners.Count];

                LineSegment edge = new LineSegment(v1, v2);
                Vector3 intersection = new Vector3();
                if (edge.Intersects(position, position + Vector3.UnitX, out intersection, true, true))
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
                    roofName = "RoofBrown";
                    break;
                default:
                    interiorWallsName = "HouseInteriorWallsBrown";
                    exteriorWallsName = "HouseExteriorWallsBrown";
                    roofName = "RoofBrown";
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
