using Game1.DataClasses;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Game1.Items
{
    class Chair:Item
    {

        

        public Chair(string argId, int[] argLocation, string argRoomClass) : base(argId, argLocation, argRoomClass)
        {
            modelName = "Chair";

        }


        public override void SetCorners()
        {
            Regex regex = new Regex(@"I\d+$");
            var match = regex.Match(id);

            string name = itemClass + match.Value;

            DataTable coordTable = ExcelFileManager.ReadCoordinates(name , item: true);

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




                corners.Add(townCoords);
            }



        }

    }
}
