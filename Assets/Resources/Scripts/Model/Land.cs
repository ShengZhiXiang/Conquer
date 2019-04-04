using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地块类
/// </summary>
public class Land  {
   
    //当前地块的阵营ID
    private int _campID;
    public int CampID
    {
        get { return _campID; }
        set { _campID = value; }
    }
    //当前地块的地形
    private CustomTerrain _curTerrain;
    public CustomTerrain CurTerrian
    {
        get { return _curTerrain; }
        set { _curTerrain = value; }
    }
    //有几个战斗单位
    public int battleUnit;
    //是否是首都
    private bool _isCapital;
    public bool IsCapital
    {
        get { return _isCapital;  }
        set
        {
            if (_isCapital!=value)
            {
                _isCapital = value;
                //if (OnLandSet2Capital!=null)
                //{
                //    OnLandSet2Capital(CoordinateInMap);
                //}
            }
        }
    }
    //是否有阵营占据
    public bool isCampOccupied = false;
    //是否有地形占据
    public bool isTerriannOccupied = false;
    //在TileMap下的坐标
    private Vector3Int _coordinateInMap;
    public Vector3Int CoordinateInMap
    {
        get { return _coordinateInMap; }
        set { _coordinateInMap = value; }
    }

    public Action<Vector3Int> OnLandSet2Capital;

    public Land(Vector3Int coordinateInMap)
    {
        CoordinateInMap = coordinateInMap;
    }
    public void InitialLandInfo(int campID, bool isCapital = false, bool isCampOccupied = true)
    {
        CampID = campID;
        this.IsCapital = isCapital;
        battleUnit = isCapital ? 12 : UnityEngine.Random.Range(1, 3);
        this.isCampOccupied = isCampOccupied;
    }

    public void SetLandInfo(int campID,int battleUnit)
    {
        CampID = campID;
        this.battleUnit = battleUnit;
    }
    public void SetTerrianInfo(CustomTerrain terriann, bool isterrianOccupied = true)
    {
        CurTerrian = terriann;
        isTerriannOccupied = isterrianOccupied;
    }

    public bool ArmyCanMove()
    {
        return battleUnit > 1;
    }

  
}
