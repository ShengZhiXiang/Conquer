using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;

public class TerrainExcelAccess : ExcelAccessBase<TerrainModel> {
    public override List<TerrainModel> CreateList()
    {
        string excelName = "Excels/Terrain/TerrainInfo" + ".xlsx";
        string sheetName = "basic";
        List<TerrainModel> result = new List<TerrainModel>();
        DataRowCollection collection = ReadExcel(excelName, sheetName);

        for (int i = 1; i < collection.Count; i++)
        {
            TerrainModel campModel = new TerrainModel
            {
                terrainID = int.Parse(collection[i][0].ToString()),
                terrainName = collection[i][1].ToString(),
                initialPopulation = int.Parse(collection[i][2].ToString()),
                initalGold = int.Parse(collection[i][3].ToString()),
                growPopulation = int.Parse(collection[i][4].ToString()),
                growGold = int.Parse(collection[i][5].ToString()),
                tilePath = collection[i][6].ToString(),
                description = collection[i][7].ToString()
            };

            result.Add(campModel);
        }
        return result;
    }

   
}
