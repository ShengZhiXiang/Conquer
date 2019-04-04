using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;

public class Terrain_MapExcelAccess : ExcelAccessBase<Terrain_MapModel> {

    public override List<Terrain_MapModel> CreateList()
    {
        string excelName = "Excels/Terrain/Terrain_MapInfo" + ".xlsx";
        string sheetName = "basic";
        List<Terrain_MapModel> result = new List<Terrain_MapModel>();
        DataRowCollection collection = ReadExcel(excelName, sheetName);

        for (int i = 2; i < collection.Count; i++)
        {
            Terrain_MapModel campModel = new Terrain_MapModel
            {
                size = int.Parse(collection[i][0].ToString()),
                terrainID_AmountList = new List<Terrain_MapModel.Pair>
                {
                    new Terrain_MapModel.Pair
                    (int.Parse(collection[1][1].ToString()), int.Parse(collection[i][1].ToString())),
                    new Terrain_MapModel.Pair
                    (int.Parse(collection[1][2].ToString()), int.Parse(collection[i][2].ToString())),
                    new Terrain_MapModel.Pair
                    (int.Parse(collection[1][3].ToString()), int.Parse(collection[i][3].ToString())),
                    new Terrain_MapModel.Pair
                    (int.Parse(collection[1][4].ToString()), int.Parse(collection[i][4].ToString())),
                }
            };

            result.Add(campModel);
        }
        return result;
    }

  
}
