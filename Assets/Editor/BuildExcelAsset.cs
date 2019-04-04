using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 所有生成Excel表的asset文件的编辑器方法都写到这里
/// </summary>
public class BuildExcelAsset : Editor {

    static ModelHolder modelHolder =  CreateInstance<ModelHolder>();
    static readonly string path = "Assets/Resources/ExcelAsset/ModelHolder.asset";
    [MenuItem("Tool/BuildExcelAsset/Camp")]
    public static void BuildCampAsset()
    {
        modelHolder.campModels = new CampExcelAccess().CreateList();
        SaveAsset();
    }
    [MenuItem("Tool/BuildExcelAsset/Terrain")]
    public static void BuildTerrainAsset()
    {

        modelHolder.terrainModels = new TerrainExcelAccess().CreateList();

        SaveAsset();
    }
    [MenuItem("Tool/BuildExcelAsset/Terrain_Map")]
    public static void BuildTerrain_MapAsset()
    {

        //if (!AssetDatabase.Contains(modelHolder))
        //{
        //    modelHolder = CreateInstance<ModelHolder>();
        //}
        modelHolder.terrain_MapModels = new Terrain_MapExcelAccess().CreateList();

        SaveAsset();
    }

    static void SaveAsset()
    {
        if (!AssetDatabase.Contains(modelHolder))
        {
            AssetDatabase.CreateAsset(modelHolder, path);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

}
