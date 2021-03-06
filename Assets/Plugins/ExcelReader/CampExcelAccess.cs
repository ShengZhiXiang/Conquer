﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;


public  class CampExcelAccess : ExcelAccessBase<CampModel> {


    public  override List<CampModel> CreateList()
    {
        string excelName = "Excels/Battle/CampInfo" + ".xlsx";
        string sheetName = "basic";
        List<CampModel> result = new List<CampModel>();
        DataRowCollection collection = ReadExcel(excelName, sheetName);

        for (int i = 1; i < collection.Count; i++)
        {
           
            CampModel campModel = new CampModel
            {
                campID = int.Parse(collection[i][0].ToString()),
                campName = collection[i][1].ToString(),
                campTilePath = collection[i][2].ToString(),
                baseUnitSpritePath = collection[i][3].ToString(),
                campClassName = collection[i][4].ToString(),
                cardStartIndex = int.Parse(collection[i][5].ToString()),
                cardEndIndex = int.Parse(collection[i][6].ToString()),
                cannonTilePath = collection[i][7].ToString()
            };

            result.Add(campModel);
        }
        return result;
    }

   
}
