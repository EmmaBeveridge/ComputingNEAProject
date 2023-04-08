using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Game1.DataClasses;
using Game1.GOAP;
using Game1.Rooms;
using Game1.UI;
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

        /// <summary>
        /// true if on left side of street
        /// </summary>
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

        public List<Vector3> groundCorners = new List<Vector3>();


        public Dictionary<string, GOAPAction> actionLabels = new Dictionary<string, GOAPAction>();
        public List<Button> actionButtons = new List<Button>();
        public List<GOAPAction> GOAPActions = new List<GOAPAction>();

        public static float StreetOffset = 100;



        public List<string> CareerNames = new List<string>();
        public string EmployedAtConditionString = null; //string used when defining conditions for GOAP action to indicate employed at building

        public Building()
        {
            modelName = "Office";
        }

        /// <summary>
        /// BuildButtons() : Generates action selection buttons and sets position to allow buttons to be displayed in a circle around the screen. 
        /// </summary>
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

            //SetCorners();


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
                float x = (float)Math.Cos(angle) * Button.circleRadius + graphicsDeviceManager.GraphicsDevice.Viewport.Width / 2 - Button.defaultTexture.Width / 2;
                float y = (float)Math.Sin(angle) * Button.circleRadius + graphicsDeviceManager.GraphicsDevice.Viewport.Height / 2 - Button.defaultTexture.Height / 2;

                if (angle < 0) { angle += MathHelper.TwoPi; }
                if (angle >= MathHelper.TwoPi) { angle -= MathHelper.TwoPi; }


                actionButtons.Add(new Button(actionLabels.Keys.ElementAt(i), actionLabels.Values.ElementAt(i), new Vector2(x, y)));




            }
        }




        /// <summary>
        /// Uses UIHandler to clear the current buttons displayed and adds all buttons advertising valid actions for the user to display list in UIHandler.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="player"></param>
        public void DisplayActions(Game1 game, Player player)
        {
            game.UIHandler.ClearButtons();
            game.UIHandler.AddRangeButtons(GetValidButtons(player));
        }


        /// <summary>
        ///  Determines which of the buildings actions are currently valid for the user (e.g. should not display option for user to quit job/ attend work at a building at which they are not employed) and returns list of action buttons describing only valid actions. 
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        protected List<Button> GetValidButtons(Player person)
        {
            List<Button> validButtons = new List<Button>();

            if (person.Career != null && CareerNames.Contains(person.Career.CareerName)) //employed in career at building
            {
                validButtons.AddRange(actionButtons.FindAll(b => b.buttonAction.GetStateOfPrecondition(EmployedAtConditionString) == true));
            } 
            else { validButtons.AddRange(actionButtons.FindAll(b => b.buttonAction.GetStateOfPrecondition(EmployedAtConditionString) == false)); }
            return validButtons;
        }


        #region TESTING FAILED
        //public void SetCorners()
        //{
        //    //Regex regex = new Regex(@"T\d+\.R\.S\d\.(\d\.)*H\d+");
        //    Regex regex = new Regex(@"B\d+$");
        //    var match = regex.Match(id);
        //    DataTable coordTable = ExcelFileManager.ReadCoordinates(match.Value, town: true);

        //    foreach (DataRow row in coordTable.Rows)
        //    {
        //        groundCorners.Add(new Vector3(int.Parse(row["X"].ToString()), 0, int.Parse(row["Z"].ToString())));
        //    }



        //}

        #endregion

        public virtual void DefineActions() { }





    }
}
