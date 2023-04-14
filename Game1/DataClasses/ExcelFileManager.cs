using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronXL;

namespace Game1.DataClasses
{
    class ExcelFileManager
    {
        //static string houseRectanglesFileName = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName + "/Data/HouseRectangles.xlsx";
        static string houseRectanglesFileName = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName + "/Data/TownRectangles.xlsx";
        static string itemCoordinatesFileName = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName + "/Data/ItemCoordinates.xlsx";


        /// <summary>
        /// Reads data from Excel file into a DataTable object. 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static DataTable ReadExcelFile (string fileName)
        {
            fileName = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName + "/Data/" + fileName;
            WorkBook workBook = WorkBook.Load(fileName);
            WorkSheet workSheet = workBook.DefaultWorkSheet;
            return workSheet.ToDataTable(true);
            
        }

        /// <summary>
        /// Reads coordinates for house or items depending on parameters using the static fields storing the relevant file names. 
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="town"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static DataTable ReadCoordinates(string sheetName, bool town = false, bool item = false )
        {
            string fileName = null;
            if (town)
            {
                fileName = houseRectanglesFileName;
            }
            else if (item)
            {
                fileName = itemCoordinatesFileName;
            }

            WorkBook workbook = WorkBook.Load(fileName);
            WorkSheet worksheet = workbook.GetWorkSheet(sheetName);
            return worksheet.ToDataTable(true);



        }


       




    }
}
