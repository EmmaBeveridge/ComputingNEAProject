﻿using System;
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
        static string houseRectanglesFileName = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName + "/Data/HouseRectangles.xlsx";



        public static DataTable ReadExcelFile (string fileName)
        {
            WorkBook workBook = WorkBook.Load(fileName);
            WorkSheet workSheet = workBook.DefaultWorkSheet;
            return workSheet.ToDataTable(true);
            
        }


        public static DataTable ReadCoordinates(string sheetName)
        {
            WorkBook workbook = WorkBook.Load(houseRectanglesFileName);
            WorkSheet worksheet = workbook.GetWorkSheet(sheetName);
            return worksheet.ToDataTable(true);



        }


       




    }
}