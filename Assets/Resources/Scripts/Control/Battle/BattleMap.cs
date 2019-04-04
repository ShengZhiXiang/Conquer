using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;

public struct MapSize
{
    public int Height;
    public int Width;

    public int HeightOffset;
    public int WidthOffset;
    public MapSize(int height,int width)
    {
        Height = height;
        Width = width;

        HeightOffset = Height / 2;
        WidthOffset = Width / 2;
    }
}
public enum MapHeight
{
    SMALL = 4,
    MIDDLE = 6,
    LARGE = 8
}

public class BattleMap :MonoBehaviour,IPointerEnterHandler,IPointerClickHandler,IPointerExitHandler {
    public Tilemap campMap;
    public Tilemap terrianMap;
    public Tilemap capitalMap;
    public Tile[] terrianTiles;
   
    //地图规模   
    private MapSize _mapSize;
    public MapSize MapSize
    {
        get { return _mapSize; }
        set { _mapSize = value; }
    }

    //阵营字典
    //private Dictionary<int, Camp> _camps;
    //public Dictionary<int, Camp> Camps
    //{
    //    get { return _camps; }
    //    set { _camps = value; }
    //}

    //二维数组表示的地图
    private Land[,] _map;
    public Land[,] Map
    {
        get { return _map; }
        set { _map = value; }
    }
    public UnityAction mapEnterAction;
    public UnityAction<Land,PointerEventData> mapClickAction;
    public UnityAction mapExitAction;

    //临时地图规模-地形数量字典，到时候会从DataSet里拿
    private Dictionary<CustomTerrain, int> terrianDic = new Dictionary<CustomTerrain, int>();

    
    /// <summary>
    /// 设置一些初始化信息，在获得脚本引用后调用
    /// </summary>
    /// <param name="size"></param>
    /// <param name="campCount"></param>
    
    private void Awake()
    {
        Clear();
        MapSize = BattleManager.Instance.MapSize;
    }

    private void Start()
    {
        
    }
    public void Clear()
    {
        //if (Map == null)
        //    return;
        Vector3Int coordinate;
        int offset = 11;
        //用tileMap接口清除地图上的所有tile资源
        for (int i = 0; i < 22; i++)
        {
            for (int j = 0; j < 22; j++)
            {
                coordinate = new Vector3Int(j - offset, offset - i, 0);
                campMap.SetTile(coordinate, null);
                terrianMap.SetTile(coordinate, null);
            }
        }
    }
    /// <summary>
    /// 对外接口，重新生成地图
    /// </summary>
    public void ReMakeMap()
    {
        Clear();
        InitialMap();
        InitLandsInMap();
        FillingMapWithTerrain();
        FillingMapWithCamp();
    }
    /// <summary>
    /// 根据地图规模，阵营数量和地形字典初始化地图数据
    /// </summary>
    public void InitialMap()
    {
        
        Map = new Land[MapSize.Height, MapSize.Width];
        if (terrianDic != null)
        {
            terrianDic.Clear();
        }
        //地形表
        Dictionary<int, TerrainModel> terrainModelDic = GameDataSet.Instance.terrainModelDic;
        //地图大小-地形数量表
        Terrain_MapModel curTerrain_Map =  GameDataSet.Instance.terrain_MapModelDic[MapSize.Height];
        List<Terrain_MapModel.Pair> terrainID_AmountList = curTerrain_Map.terrainID_AmountList;
        foreach (Terrain_MapModel.Pair ID_Amount in terrainID_AmountList)
        {
            int terrainID = ID_Amount.key;
            TerrainModel model = terrainModelDic[terrainID];
            terrianDic.Add(new CustomTerrain(model), ID_Amount.value);
            Debug.Log(string.Format("{0}号地形，有{1}个",model.terrainName, ID_Amount.value));
        }      
    }

    /// <summary>
    /// 初始化地图
    /// 给Map数组赋值坐标
    /// </summary>
    public void InitLandsInMap()
    {
        Vector3Int coordinate;
        //行遍历
        for (int i = 0; i < Map.GetLength(0); i++)
        {
            //列遍历
            for (int j = 0; j < Map.GetLength(1); j++)
            {
                //用地图上的坐标初始化数组中的每一个元素。
                coordinate = Convert2MapCoordinate(i, j);
                _map[i, j] = new Land(coordinate);
            }
        }
    }
   
    /// <summary>
    /// 在地图上放随机地形
    /// </summary>
    public void FillingMapWithTerrain()
    {
        List<CustomTerrain> keys = new List<CustomTerrain>(terrianDic.Keys);
        //前几个找随机空地放地形
        for (int i = 0; i < keys.Count - 1; i++)
        {
            CustomTerrain curTerrain = keys[i];
            for (int j = 0; j < terrianDic[curTerrain]; j++)
            {
                int[] emptyGridXY = GetEmptyGird(1);
                int x = emptyGridXY[0];
                int y = emptyGridXY[1];

                Map[x, y].SetTerrianInfo(curTerrain);

                terrianMap.SetTile(Map[x, y].CoordinateInMap, curTerrain.tile);
            }
        }
        //地图上空的地方放最后一个地形
        CustomTerrain lastTerrain = keys[keys.Count - 1];
        for (int i = 0; i < Map.GetLength(0); i++)
        {
            for (int j = 0; j < Map.GetLength(1); j++)
            {
                if (!Map[i, j].isTerriannOccupied)
                {
                    Map[i, j].SetTerrianInfo(lastTerrain);

                    terrianMap.SetTile(Map[i, j].CoordinateInMap, lastTerrain.tile);
                }
            }
        }
    }
    /// <summary>
    /// 用阵营填充地图
    /// </summary>
    public void FillingMapWithCamp()
    {
        List<Land> curCampLands;
        Camp curCamp;
        //遍历所有阵营-1
        //应该改用foreach循环遍历
        foreach (KeyValuePair<int, Camp> CampItem in BattleManager.Instance.CampDic)
        {
            curCamp = CampItem.Value;
            curCampLands = curCamp.ownedLands;
            if (curCampLands.Count!=0)
                curCampLands.Clear();
            for (int i=0;i<curCamp.initialLands;i++)
            {
                //得到地图数组上一个空地块
                int[] mapIndx = GetEmptyGird(2);
                int x = mapIndx[0];
                int y = mapIndx[1];

                Map[x, y].InitialLandInfo(curCamp.campID);
                //用tilemap接口绘制该地块
                campMap.SetTile(Map[x, y].CoordinateInMap, curCamp.tile);
                //在每个阵营的地块坐标List中加入他们的坐标
                curCampLands.Add(Map[x, y]);
            }
            if (BattleManager.Instance.gameMode==GameMode.PVE&&curCamp!=BattleManager.Instance.MyCamp)
            {
                curCamp.capitalLand = curCamp.ownedLands[3];
                SetCapital(curCamp.ownedLands[3]);
                //Debug.Log(string.Format("第{0}号阵营设置了首都", curCamp.campID));
            }
            
        }
              
    }
    

    /// <summary>
    /// 设置首都
    /// </summary>

    public void SetCapital(Land land)
    {
        land.InitialLandInfo(land.CampID, true, true);
        //Debug.Log(string.Format("当前阵营ID:{0},坐标：{1}，作战单位：{2}", land.CampID, land.CoordinateInMap, land.battleUnit) );
    }






    #region 工具函数
    /// <summary>
    /// 用随机数找到一个地图上空的地块
    /// 1:随机地形分配，2：随机阵营分配
    /// </summary>
    /// <returns>返回地块的坐标</returns>
    public int[] GetEmptyGird(int option)
    {
        int RandomX;
        int RandomY;
        bool gridIsOccupied;
        do
        {
            //从地图坐标范围中取两次随机数，一个做x，一个做y，组成坐标。
            RandomX = UnityEngine.Random.Range(0, MapSize.Height);
            RandomY = UnityEngine.Random.Range(0, MapSize.Width);
            gridIsOccupied = option == 1 ?
                _map[RandomX, RandomY].isTerriannOccupied :
                _map[RandomX, RandomY].isCampOccupied;
        } while (gridIsOccupied);
        return new int[2] { RandomX, RandomY };

    }
    /// <summary>
    /// 从数组坐标到地图坐标转换
    /// </summary>
    public Vector3Int Convert2MapCoordinate(int x, int y)
    {
        //因为TileMap坐标原点在屏幕中心，所以需要减去偏移值
     
        return new Vector3Int(MapSize.HeightOffset-x,y-MapSize.WidthOffset, 0);
    }
    /// <summary>
    /// 从地图坐标转换到二维数组下标
    /// </summary>
    public int[] Convert2ArrayIndex(Vector3Int vector3Int)
    {
       
        return new int[2] { MapSize.HeightOffset-vector3Int.x, vector3Int.y+MapSize.WidthOffset };
    }

    public int[] Convert2ArrayIndex(Vector3 vector3)
    {
        
        return new int[2] { MapSize.HeightOffset - (int)vector3.x, (int)vector3.y + MapSize.WidthOffset };
    }

    public bool isCoordinateInMap(Vector3Int coordinate)
    {
        int[] array = Convert2ArrayIndex(coordinate);
        int x = array[0];
        int y = array[1];
        if (x >= 0 && x < Map.GetLength(0) && y >= 0 && y <Map.GetLength(1))
        {
            return true;
        }
        return false;
    }

    public Land Coordinate2Land(Vector3Int coordinate)
    {
        if (isCoordinateInMap(coordinate))
        {
            int[] index = Convert2ArrayIndex(coordinate);
            return Map[index[0],index[1]];
        }
        return null;
    }

    /// <summary>
    /// 鼠标坐标转化到TileMap坐标
    /// </summary>
    /// <param name="inputMousePos"></param>
    /// <returns></returns>
    public Vector3Int MousePos2MapCoordinate(Vector3 inputMousePos)
    {
        Vector3 mousePos;
        Vector3Int TileMapCoordinate;
        mousePos = Camera.main.ScreenToWorldPoint(inputMousePos);
        TileMapCoordinate = campMap.WorldToCell(mousePos);
        return TileMapCoordinate;
    }

    /// <summary>
    /// 鼠标坐标转化到地图下标
    /// </summary>
    /// <param name="inputMousePos"></param>
    /// <returns></returns>
    public int[] MousePos2MapIndex(Vector3 inputMousePos)
    {
        Vector3Int vec = MousePos2MapCoordinate(inputMousePos);
        return Convert2ArrayIndex(vec);
    }

    /// <summary>
    /// 获取当前鼠标选中的在Map下的Land
    /// </summary>
    /// <returns></returns>
    public Land GetCurMouseLand(Vector3 inputMousePos)
    {
        int[] index = MousePos2MapIndex(inputMousePos);
        int x = index[0];
        int y = index[1];
        if (x >= 0 && x < Map.GetLength(0) && y >= 0 && y < Map.GetLength(1))
        {
            return Map[x, y];
        }
        return null;
       
    }

   
    /// <summary>
    /// 地图中坐标转换到世界坐标
    /// </summary>
    /// <param name="mapCoordinate"></param>
    /// <returns></returns>
    public Vector3 MapCoordinate2World(Vector3Int mapCoordinate)
    {
        return campMap.CellToWorld(mapCoordinate);
    }


    /// <summary>
    /// 地图坐标转换到屏幕坐标
    /// </summary>
    /// <param name="mapCoordinate"></param>
    /// <returns></returns>
    public Vector3 MapCoordinate2ScreenPos(Vector3Int mapCoordinate)
    {
        Vector3 worldPos = MapCoordinate2World(mapCoordinate);
        return Camera.main.WorldToScreenPoint(worldPos);
    }
    /// <summary>
    /// 两个Land是否紧邻着
    /// </summary>
    /// <returns></returns>
    public bool IsTwoLandNeighbor(Vector3Int coordinateA, Vector3Int coordinateB)
    {
        if (new Vector3Int(coordinateA.x - coordinateB.x, coordinateA.y - coordinateB.y, 0).magnitude <= 1)
        {
            return true;
        }
        Vector3Int centerLand = coordinateA.x > coordinateB.x ? coordinateA : coordinateB;
        Vector3Int belowLand = coordinateA.x > coordinateB.x ? coordinateB : coordinateA;
        if (centerLand.x - belowLand.x == 1 && Mathf.Abs(centerLand.y - belowLand.y) <= 1f)
        {
            return true;
        }
        return false;
    }

    #endregion

    #region 鼠标事件

    public void OnPointerEnter(PointerEventData eventData)
    {
      //  Debug.Log("进入Map");
        if (mapEnterAction != null)
        {
              mapEnterAction();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
       
        if (mapClickAction != null)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
            int[] array = Convert2ArrayIndex(campMap.WorldToCell(worldPos));
            int x = array[0];
            int y = array[1];           
            mapClickAction(Map[x, y], eventData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
       // Debug.Log("离开Map");
        if (mapExitAction!=null)
        {
            mapExitAction();
        }
    }
    #endregion

}
