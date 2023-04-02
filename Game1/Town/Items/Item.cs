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
using Game1.DataClasses;
using System.Data;
using Game1.NavMesh;

using Game1.GOAP;
using Game1.Town;
using Game1.UI;

namespace Game1
{
    public class Item
    {
        
        [JsonProperty("id")]
        public string id;


        //[JsonProperty("relativeLocationX")]
        //public string locationX;

        //[JsonProperty("relativeLocationZ")]
        //public string locationZ;

        [JsonProperty("relativeLocation")]
        public Neo4j.Driver.Point relativeLoc;


        [JsonProperty("class")]
        public string itemClass;

        [JsonProperty("inventory")]
        public List<string> inventory;

        /// <summary>
        /// List of plane objects forming cuboid around item. Used to detect intersection of mouse ray and item. 
        /// </summary>
        public List<Plane> planes = new List<Plane>();
        public List<Vector3> corners = new List<Vector3>();
        //public Dictionary<string, float> keyCoordinates = new Dictionary<string, float>() 
        //{ { "maxX", float.MinValue }, { "minX", float.MaxValue}, { "maxY", float.MinValue}, {"minY", float.MaxValue }, { "maxZ", float.MinValue}, { "minZ", float.MaxValue} };
        public string modelName;
        public Vector3 townLocation;

        public Vector3 houseTownLocation;
        public House house;

        public float rotation;


        public Room room;

        /// <summary>
        ///  List of text displayed on action selection buttons e.g. sleep 
        /// </summary>
        public Dictionary<string, GOAPAction> actionLabels = new Dictionary<string, GOAPAction>();

        /// <summary>
        ///  List of action buttons displayed when user is selecting an action using that item.
        /// </summary>
        public List<Button> actionButtons = new List<Button>();

        /// <summary>
        /// List of GOAP actions for this item. 
        /// </summary>
        public List<GOAPAction> GOAPActions = new List<GOAPAction>();


        public bool IsAvailable = true;


        //public People interactingWithPerson = null;
        //public bool actionComplete = false;
        //public double actionTimeElapsed = 0;


        public Avatar avatar;
        public Model model;
        Matrix itemToTownTransformation;


        /// <summary>
        /// Calculates transformation matrices and creates avatar. 
        /// </summary>
        public void GenerateAvatar()
        {

            //Matrix rotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(rotation));

            //townLocation = room.house.townLocation + new Vector3(int.Parse(locationX), 0, int.Parse(locationZ));
            //Matrix translationMatrix = Matrix.CreateTranslation(townLocation);

            //avatar = new Avatar(model, townLocation);


            //avatar.worldMatrix =  rotationMatrix * translationMatrix;

           

            Matrix rotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(rotation));


            Matrix relativeTransformation = Matrix.CreateTranslation(new Vector3((float)relativeLoc.X, 0, (float)relativeLoc.Y)) * rotationMatrix;



            house = room.house;
            houseTownLocation = house.TownLocation ;
            


            Matrix translationMatrix = Matrix.CreateTranslation(houseTownLocation);

            avatar = new Avatar(model, houseTownLocation);


            itemToTownTransformation = relativeTransformation * translationMatrix;

            townLocation = itemToTownTransformation.Translation;
            avatar.worldMatrix = itemToTownTransformation;

            GeneratePlanes();





        }

        /// <summary>
        ///  Calls SetCorners method. Creates planes of cuboid encasing item using corners of item and adds to planes list. 
        /// </summary>
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

        /// <summary>
        /// : Reads in local house coordinates for item. Applies houseToTownTransformation matrix to coordinate and obtains translation vector from this transformation and adds to corners list. 
        /// </summary>
        public virtual void SetCorners()
        {
           
            
            DataTable coordTable = ExcelFileManager.ReadCoordinates(itemClass, item: true);

            foreach (DataRow row in coordTable.Rows)
            {
                Vector3 itemCoords = new Vector3(int.Parse(row["X"].ToString()), int.Parse(row["Y"].ToString()), int.Parse(row["Z"].ToString()));

                Vector3 townCoords = (Matrix.CreateTranslation(itemCoords) * room.house.houseToTownTransformation).Translation;

                //if (townCoords.X> keyCoordinates["maxX"]) { keyCoordinates["maxX"] = townCoords.X; }
                //if (townCoords.Y> keyCoordinates["maxY"]) { keyCoordinates["maxY"] = townCoords.Y; }
                //if (townCoords.Z> keyCoordinates["maxZ"]) { keyCoordinates["maxZ"] = townCoords.Z; }
                //if (townCoords.X< keyCoordinates["minX"]) { keyCoordinates["minX"] = townCoords.X; }
                //if (townCoords.Y< keyCoordinates["minY"]) { keyCoordinates["minY"] = townCoords.Y; }
                //if (townCoords.Z< keyCoordinates["minZ"]) { keyCoordinates["minZ"] = townCoords.Z; }




                corners.Add( townCoords);
            }
           

          
        }


        public Item(string argId, int[] argLocation, string argItemClass)
        {

            id = argId;
            itemClass = argItemClass;

            

        }

        /// <summary>
        /// Determines if item cuboid contains a specified coordinate. Does so by checking if a vector extending infinitely in the positive x direction from the point has an odd number of intersections with the edges of the item rectangle on ground plane. If there are an odd number of intersections, then the point must be within rectangle/item. If there are an even or 0 intersections, then point cannot be within rectangle/item. 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool ContainsPoint(Vector3 position)
        {

            if (position.Y> corners[0].Y || position.Y<corners[4].Y)
            {
                return false;
            }

            List<Vector3> groundCorners = new List<Vector3>();
            for (int i = 0; i < corners.Count/8; i++)
            {
                Vector3[] cuboidCorners = new Vector3[] { corners[i*8+4], corners[i*8 + 5], corners[i*8 + 6], corners[i*8 + 7] };
                groundCorners.AddRange(cuboidCorners);
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


        /// <summary>
        /// Generates action selection buttons and sets position to allow buttons to be displayed in a circle around the screen. 
        /// </summary>
        /// <param name="graphicsDeviceManager"></param>
        public void BuildButtons(GraphicsDeviceManager graphicsDeviceManager)
        {
            for (int i = 0; i < actionLabels.Count; i++)
            {
                float angle = (MathHelper.TwoPi / actionLabels.Count) * i;
                float x =(float) Math.Cos(angle) * Button.circleRadius + graphicsDeviceManager.GraphicsDevice.Viewport.Width /2 - Button.defaultTexture.Width/2;
                float y =(float) Math.Sin(angle) * Button.circleRadius + graphicsDeviceManager.GraphicsDevice.Viewport.Height / 2 - Button.defaultTexture.Height/2;

                if (angle < 0) { angle += MathHelper.TwoPi; }
                if ( angle>= MathHelper.TwoPi) { angle -= MathHelper.TwoPi; }


                actionButtons.Add(new Button(actionLabels.Keys.ElementAt(i), actionLabels.Values.ElementAt(i), new Vector2(x, y)));




            }
        }




        /// <summary>
        /// Uses UIHandler to clear the current buttons displayed and adds all buttons advertising valid actions for the user to display list in UIHandler. 
        /// </summary>
        /// <param name="game"></param>
        public void DisplayActions(Game1 game)
        {
            game.UIHandler.ClearButtons();
            game.UIHandler.AddRangeButtons(actionButtons);
        }



        /// <summary>
        ///  Virtual method implemented in children classes. Method used to define the GOAP actions advertised by the item. 
        /// </summary>
        public virtual void DefineActions() { }

        //public virtual Action<GameTime> BeginAction(string actionName, People person) { return null; }





    }
}
