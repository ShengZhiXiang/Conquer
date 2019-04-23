using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelHolder : ScriptableObject {
    public List<CampModel> campModels;
    public List<TerrainModel> terrainModels;
    public List<Terrain_MapModel> terrain_MapModels;
    public List<CardModel> cardModels;

    public void GenerateAllLists()
    {
        campModels = new  CampExcelAccess().CreateList();
        terrainModels = new TerrainExcelAccess().CreateList();
        terrain_MapModels = new Terrain_MapExcelAccess().CreateList();
        cardModels = new CardExcelAccess().CreateList();
    }
	
}
