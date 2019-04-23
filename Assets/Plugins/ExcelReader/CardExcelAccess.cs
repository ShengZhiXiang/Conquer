using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class CardExcelAccess : ExcelAccessBase<CardModel> {

    public override List<CardModel> CreateList()
    {
        string excelName = "Excels/Battle/CardInfo" + ".xlsx";
        string sheetName = "basic";
        List<CardModel> result = new List<CardModel>();
        DataRowCollection collection = ReadExcel(excelName, sheetName);
        for (int i = 1; i < collection.Count; i++)
        {
            CardModel campModel = new CardModel
            {
                cardID = int.Parse(collection[i][0].ToString()),
                cardName = collection[i][1].ToString(),
                spritePath = collection[i][2].ToString(),
                isCampCard = int.Parse(collection[i][3].ToString()) == 1 ? true : false,
                belongCampID = int.Parse(collection[i][4].ToString()),
                cardFuncEnum = collection[i][5].ToString(),
                funcDescription = collection[i][6].ToString(),
                costGold = int.Parse(collection[i][7].ToString()),
                costPopulation = int.Parse(collection[i][8].ToString()),
                cardTriggerTime = collection[i][9].ToString(),
                isSelfCard = int.Parse(collection[i][10].ToString()),
            };

            result.Add(campModel);
        }
        return result;

    }
}
