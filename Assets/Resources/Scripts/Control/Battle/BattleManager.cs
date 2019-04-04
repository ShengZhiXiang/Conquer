using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
public enum BattleStateEnum
{
    None,
    SelectCaptital,
    MyRound
}

public enum GameMode
{
    PVE,
    PVP
}

public class BattleManager : Singleton<BattleManager> {


    /// <summary>
    /// 对战地图
    /// </summary>
    private BattleMap _battleMap;
    public BattleMap BattleMap
    {
        get { return _battleMap; }
        set { _battleMap = value; }
    }

    /// <summary>
    /// 阵营信息字典
    /// </summary>
    private Dictionary<int, Camp> _campDic;
    public Dictionary<int, Camp> CampDic
    {
        get { return _campDic; }
        set { _campDic = value; }
    }

    public Dictionary<BattleStateEnum, BattleStateBase> BattleStateDictionary;
    private void Awake()
    {
        BattleStateDictionary = new Dictionary<BattleStateEnum, BattleStateBase>();
        BattleStateDictionary.Add(BattleStateEnum.None, new BattleStateBase());
        BattleStateDictionary.Add(BattleStateEnum.SelectCaptital, new SelectCapitalState());
        BattleStateDictionary.Add(BattleStateEnum.MyRound, new MyRoundState());

        CurBattleState = BattleStateDictionary[BattleStateEnum.None];
        CurBattleState.EnterState();

        BattleStart += SetCurCampCanAttack;
        BattleFinishAttackOneLand += OnFinshAttackOneLand;
    }
    //我的阵营
    private Camp _myCamp;
    public Camp MyCamp
    {
        get { return _myCamp; }
        set { _myCamp = value; }
    }
    //当前阵营
    private Camp _curCamp;
    public Camp CurCamp
    {
        get { return _curCamp; }
        set {  _curCamp = value;   }
    }

    //地图大小
    private MapSize _mapSize;
    public MapSize MapSize
    {
        get { return _mapSize; }
        set {
                if (!_mapSize.Equals(value))
                {
                     _mapSize = value;
                }
            }
    }
    //阵营数量
    private int _campCount;
    public int CampCount
    {
        get { return _campCount; }
        set
        {
            if (_campCount != value)
            {
                _campCount = value;
            }
        }
    }

    //当前状态的引用
    private BattleStateBase _curBattleState;
    public BattleStateBase CurBattleState
    {
        get { return _curBattleState; }
        set {
            if (_curBattleState != value)
            {
                if (_curBattleState!=null)
                {
                    _curBattleState.ExitState(ref BattleMap.mapEnterAction, ref BattleMap.mapClickAction, ref BattleMap.mapExitAction);
                }               
                _curBattleState = value;
            }
        }
    }
    #region 战斗中事件
    /// <summary>
    /// 战斗开始事件
    /// </summary>
    public Action BattleStart;

    /// <summary>
    /// 进攻一个地块结束的回调
    /// </summary>
    public Action BattleFinishAttackOneLand;


    /// <summary>
    /// 我的回合结束的回调
    /// </summary>
    public Action BattleEndMyTurn;

    /// <summary>
    /// 重新选择地图事件
    /// </summary>
    public Action BattleReSelectMap;
    #endregion
    /// <summary>
    /// 临时的进攻剩余步数，以后替换成粮食和金币
    /// </summary>
    private int _attackLeftStep = 100;

    private bool _canAttack;
    /// <summary>
    /// 是否可以进攻
    /// </summary>
    public bool CanAttack
    {
        get { return _canAttack; }
        set
        {
            if (_canAttack!=value)
            {
                _canAttack = value;
            }
        }
    }

    public GameMode gameMode = GameMode.PVE;

    /// <summary>
    /// 生成对战地图，应该只调用一次
    /// </summary>
    /// <param name="mapSize"></param>
    /// <param name="campCount"></param>
    public void GenerateMapInScene(int mapHeight, int campCount)
    {
        SetMapInfo(mapHeight, campCount);
        SetCamps();

        GameObject battleMap = Resources.Load("Prefabs/GameMap") as GameObject;
        GameObject MapInstance = Instantiate(battleMap);
        BattleMap = MapInstance.GetComponent<BattleMap>();


        InitalMap();
        //进入选首都阶段状态
        CurBattleState = BattleStateDictionary[BattleStateEnum.SelectCaptital];
        CurBattleState.EnterStateWithMouse(ref BattleMap.mapEnterAction, ref BattleMap.mapClickAction, ref BattleMap.mapExitAction);

    }

    public void SetMapInfo(int mapHeight, int campCount)
    {
        int mapWidth = mapHeight * 16 / 9;
      
        MapSize = new MapSize(mapHeight, mapWidth);
        Debug.Log(string.Format("MapSize的高度是{0},宽度是{1},高度一半是{2},宽度一半是{3}",MapSize.Height,MapSize.Width,MapSize.HeightOffset,MapSize.WidthOffset));
        CampCount = campCount;
    }
    /// <summary>
    /// 设置对战信息
    /// </summary>
    public void SetCamps()
    {
        Dictionary<int, CampModel> tempDic = GameDataSet.Instance.CampModelDic;
        CampDic = new Dictionary<int, Camp>();
        int allLands = MapSize.Height*MapSize.Width;
        int landCountPerCamp = allLands / CampCount;
        //这里是按顺序从CampAsset里拿资源，后期应该改成自己选阵营     
        List<int> campIDs = new List<int>(tempDic.Keys);
        Tile campTile;
        string campName;
        for (int i = 0; i < CampCount - 1; i++)
        {
            campTile = Resources.Load<Tile>(tempDic[campIDs[i]].tilePath);
            campName = tempDic[campIDs[i]].campName;
            CampDic.Add(campIDs[i], new Camp(campIDs[i],campName, landCountPerCamp, campTile));
        }
        int leftPlots = allLands - (landCountPerCamp * (CampCount - 1));
        campTile = Resources.Load<Tile>(tempDic[campIDs[CampCount - 1]].tilePath);
        campName = tempDic[campIDs[CampCount - 1]].campName;
        CampDic.Add(campIDs[CampCount - 1], new Camp(campIDs[CampCount - 1],campName, leftPlots, campTile));


        //暂时设置我的阵营为第一个阵营
        MyCamp = CampDic[campIDs[0]];
        //设置当前阵营
        CurCamp = MyCamp;
    }
   

    
   
    

   public  IEnumerator  ReGenerateMap()
    {
        LoadingUI loadingUI = GlobalUImanager.Instance.LoadingUI.GetComponent<LoadingUI>();
        loadingUI.PlayAnimation();
        float time = loadingUI.GetAnimationTime();
        yield return new WaitForSeconds(time);
        BattleMap.ReMakeMap();
        if (BattleReSelectMap!=null)
        {
            BattleReSelectMap();
        }
    }

    private void InitalMap()
    {
        BattleMap.ReMakeMap();
    }

    private void Update()
    {
        CurBattleState.OnUpdateFunc();
    }

 

    private void SetCurCampCanAttack()
    {
        bool campCanMove = false;
        foreach(Land land in CurCamp.ownedLands)
        {
            if (land.ArmyCanMove())
            {
                campCanMove = true;
                break;
            }
        }
        //如果当前进攻步数大于0，并且至少有一个领地的兵可以动，那么可以进攻
        CanAttack = _attackLeftStep > 0 && campCanMove;
    }

    public void OnFinshAttackOneLand()
    {
        //减去进攻步数
        _attackLeftStep--;
        SetCurCampCanAttack(); 
        //todo
    }

    
    public void TwoLandFight(Land attackLand, Land defenceLand)
    {
        int winLandLeftUnit = 0;

        //守方胜
        if (attackLand.battleUnit <= defenceLand.battleUnit)
        {
  
            //给进攻失败这块地重新赋值
            attackLand.SetLandInfo(attackLand.CampID,1);

        }
        //攻方胜
        else
        {
            //显示上重新绘制防守地块上的Tile
            BattleMap.campMap.SetTile(defenceLand.CoordinateInMap, CampDic[attackLand.CampID].tile);
            //从防守地块列表中删除该地块
            CampDic[defenceLand.CampID].ownedLands.Remove(defenceLand);
            //给攻下的这块地重新赋值
            winLandLeftUnit = attackLand.battleUnit - 1;
            defenceLand.SetLandInfo(attackLand.CampID, winLandLeftUnit);
            //进攻地块留守一个单位
            attackLand.SetLandInfo(attackLand.CampID,1);
            //给进攻地块列表加上攻下的这块地
            CampDic[attackLand.CampID].ownedLands.Add(defenceLand);
        }

        if (BattleFinishAttackOneLand!=null)
        {
            BattleFinishAttackOneLand();
        }
    }

    private int curCampIndex = 0;
    /// <summary>
    /// 我的回合结束
    /// </summary>
    public void MyTurnEnd()
    {
        //补给各地块
        //todo
        curCampIndex = (curCampIndex + 1) % CampCount;
        List<int> campIDs = new List<int>(GameDataSet.Instance.CampModelDic.Keys);
        CurCamp = CampDic[campIDs[curCampIndex]];
        if (BattleEndMyTurn!=null)
        {
            BattleEndMyTurn();
        }
        //目前模拟本地人人对战
        //把if括号加上就是人机回合
        //if (CurCamp == MyCamp)
        //{
            CurBattleState = BattleStateDictionary[BattleStateEnum.MyRound];
            CurBattleState.EnterStateWithMouse(ref BattleMap.mapEnterAction, ref BattleMap.mapClickAction, ref BattleMap.mapExitAction);
        //}
        //else
        //{
        //    //转到下一回合
        //    CurBattleState = BattleStateDictionary[BattleStateEnum.None];
        //    CurBattleState.EnterState();
        //}
        
    }

    /// <summary>
    /// 两个Land是否紧邻着
    /// </summary>
    /// <returns></returns>
    public bool IsTwoLandNeighbor(Vector3Int coordinateA, Vector3Int coordinateB)
    {
        return BattleMap.IsTwoLandNeighbor(coordinateA, coordinateB);
    }

    public bool isCoordinateInMap(Vector3Int coordinate)
    {      
        return BattleMap.isCoordinateInMap(coordinate);
    }
   
   
}
