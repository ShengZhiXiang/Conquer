﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataSet : Singleton<GameDataSet> {

    public  Dictionary<int, CampModel> CampModelDic = new Dictionary<int, CampModel>();
    public Dictionary<int, TerrainModel> terrainModelDic = new Dictionary<int, TerrainModel>();
    public  Dictionary<int, Terrain_MapModel> terrain_MapModelDic = new Dictionary<int, Terrain_MapModel>();
    public Dictionary<int, CardModel> CardModelDic = new Dictionary<int, CardModel>();
    public Dictionary<string, int> CampName_IDDic = new Dictionary<string, int>();
    public void Awake()
    {
        base.Initial();
        ModelHolder modelHolder = Resources.Load<ModelHolder>("ExcelAsset/ModelHolder");
        //阵营
        foreach (CampModel campModel in modelHolder.campModels)
        {
            CampModelDic.Add(campModel.campID, campModel);
            CampName_IDDic.Add(campModel.campName,campModel.campID);
        }
        //卡牌
        foreach (CardModel cardModel in modelHolder.cardModels)
        {
            CardModelDic.Add(cardModel.cardID,cardModel);
        }
        //地形表
        foreach (TerrainModel terrainModel in modelHolder.terrainModels)
        {
            terrainModelDic.Add(terrainModel.terrainID, terrainModel);
        }
        //地形_地图表
        foreach (Terrain_MapModel terrain_mapModel in modelHolder.terrain_MapModels)
        {
            terrain_MapModelDic.Add(terrain_mapModel.size, terrain_mapModel);
        }

    }

    /// <summary>
    /// 空方法，只为了唤醒GameDataSet
    /// </summary>
    public void Empty()
    {
        Debug.Log("GameDataSet已启动");
    }
}
