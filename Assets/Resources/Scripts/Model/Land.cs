using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ArmyType
{
    SingleUnit,
    TankWithUnit,
    AirPlaneWithUnit
}
public struct Cannon
{
    //是否拥有
    public bool isOwned;
    //是否处于冷却状态
    public bool isInCool;
    //上一次开火的回合
    public int lastFire;
}
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
    //当前的卡牌
    private BattleCard _battleCard;
    public BattleCard BattleCard
    {
        get { return _battleCard; }
        set { _battleCard = value; }
    }
    
    //有几个战斗单位
    private int _battleUnit;
    public int BattleUnit
    {
        get { return _battleUnit; }
        set
        {
            _battleUnit = value;
            if (_battleUnit > 0 && _battleUnit < 4)
            {
                ArmyType = ArmyType.SingleUnit;
                //设置该地的tile为普通士兵
                //todo
            } else if (_battleUnit >= 4 && _battleUnit < 9)
            {
                ArmyType = ArmyType.TankWithUnit;
                //设置该地的tile为坦克
                //todo
            } else if (_battleUnit >= 9)
            {
                ArmyType = ArmyType.AirPlaneWithUnit;
                //设置该地的tile为飞机
                //todo
            }

        }
    }

    //该地的军队类型
    private ArmyType _armyType;
    public ArmyType ArmyType
    {
        get { return _armyType; }
        private set { _armyType = value; }
    }
   //该地的炮
    public Cannon cannon;
    //剩余几个人口
    public int leftPopulation;
    //初始金币
    public int initalGold;
    //每回合增长金币
    public int increaseGold;
    //每回合增长的人口
    public int increasePopulation;
    private readonly int maxPopulation = 9;
    private readonly int maxBattleUnit = 9;
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

    public Action OnLandSet2Capital;


    #region UI
    private BattleBaseUnitSupplyTip _battleBaseUnitSupplyTip;
    public BattleBaseUnitSupplyTip BattleBaseUnitSupplyTip
    {
        get
        {
            if (_battleBaseUnitSupplyTip == null)
            {
                GameObject temp = Resources.Load<GameObject>("Prefabs/UI/BattleBaseUnitSupplyTip");
                GameObject go = UnityEngine.Object.Instantiate(temp, GlobalUImanager.Instance.UI_Land_SupplyTips);
                go.SetActive(false);
                _battleBaseUnitSupplyTip = go.GetComponent<BattleBaseUnitSupplyTip>();
                _battleBaseUnitSupplyTip.SetPosition(CoordinateInMap);
            }
            return _battleBaseUnitSupplyTip;
        }
    }

    private LandHighLightSide _landHighLightSide;
    public LandHighLightSide LandHighLightSide
    {
        get
        {
            if (_landHighLightSide == null)
            {
                GameObject temp = Resources.Load<GameObject>("Prefabs/UI/LandHighLightSide");
                GameObject go = UnityEngine.Object.Instantiate(temp, GlobalUImanager.Instance.Ui_Land_HighLights);
                go.SetActive(false);
                _landHighLightSide = go.GetComponent<LandHighLightSide>();
                _landHighLightSide.SetPosition(CoordinateInMap);
            }
            return _landHighLightSide;
        }
    }


    #endregion
    public Land(Vector3Int coordinateInMap)
    {
        CoordinateInMap = coordinateInMap;
    }
    public void InitialLandInfo(int campID,  bool isCampOccupied = true)
    {
        CampID = campID;
        BattleUnit = UnityEngine.Random.Range(1, 3);
        //地形资源
        leftPopulation =  CurTerrian!=null ? CurTerrian.initialPopulation : 5;
        initalGold = CurTerrian != null ? CurTerrian.initalGold : 2;
        increasePopulation = CurTerrian != null ? CurTerrian.growPopulation : 1;
        increaseGold = CurTerrian != null ? CurTerrian.growGold : 1;
        this.isCampOccupied = isCampOccupied;
        cannon = new Cannon();
    }


    public void SetCapital()
    {
        //动画效果
        //todo
        IsCapital = true;
        BattleUnit = UnityEngine.Random.Range(3, 5);
        //首都地形的初始各种资源都加一点
        leftPopulation += 2;
        increasePopulation += 1;
        increaseGold += 1;
        if (OnLandSet2Capital!=null)
        {
            OnLandSet2Capital();
        }
    }

    public void SetLandInfo(int campID,int battleUnit,Cannon cannon)
    {
        CampID = campID;
        BattleUnit = battleUnit;
        this.cannon = cannon;
    }
    public void SetLandInfo(int campID, int battleUnit)
    {
        CampID = campID;
        BattleUnit = battleUnit;
    }
    public void SetTerrianInfo(CustomTerrain terriann, bool isterrianOccupied = true)
    {
        CurTerrian = terriann;
        isTerriannOccupied = isterrianOccupied;
    }

    public bool ArmyCanMove()
    {
        return BattleUnit > 1;
    }

    public void EndTurnSupply()
    {
        //金币补给
        BattleManager.Instance.CurCamp.AddCampGold(increaseGold)  ;
        //人口补给
        leftPopulation += increasePopulation;
        //随机给兵,每回合随机补0~3个兵
        int randomSupplyUnit = UnityEngine.Random.Range(0, 4);
        //如果该地的剩余人口大于随机数，那么补随机数的兵，并扣除对应剩余人口
        if (leftPopulation >= randomSupplyUnit)
        {
            BattleUnit += randomSupplyUnit;
            leftPopulation -= randomSupplyUnit;
        }
        //否则把该地补满(剩余人口不够随机出来的，补充最大人数)
        else
        {
            BattleUnit += leftPopulation;
            leftPopulation = 0;
        }

        //最大军队数限制
        if (BattleUnit > maxBattleUnit)
        {
            leftPopulation += BattleUnit - maxBattleUnit;
            BattleUnit = maxBattleUnit;
        }
        //人口上限
        if (leftPopulation > maxPopulation)
        {
            leftPopulation = maxPopulation;
        }
        BattleBaseUnitSupplyTip.SetImage(BattleManager.Instance.CurCamp.baseUnitSprite);
        BattleBaseUnitSupplyTip.ShowSelf();
    }

    /// <summary>
    /// 获取最基本的骰子投掷次数
    /// 每个阵营都应该相同，基于BattleUnit
    /// </summary>
    /// <returns></returns>
    public int GetBasicDiceRolls()
    {
        return BattleUnit;
    }
    #region 卡牌相关代码
    /// <summary>
    /// 摇骰子根据卡牌获得的额外收益
    /// </summary>
    /// <param name="triggerTime"></param>
    /// <returns></returns>
    public int GetExtraIncreaseByCard(BattleCardTriggerTime triggerTime)
    {
        if (BattleCard==null||BattleCard.triggerTime!=triggerTime)
        {
            return 0;
        }
        int result  = BattleCard.CardFunc(this);
        BattleCard = null;
        return result;
    }


    /// <summary>
    /// 进攻或防守完的效果
    /// </summary>
    /// <param name="triggerTime"></param>
    public void AfterFightCardEffect(BattleCardTriggerTime triggerTime)
    {
        if (BattleCard == null || BattleCard.triggerTime != triggerTime)
        {
            return ;
        }

        BattleCard.CardFunc(this);
        BattleCard = null;
    }

    public bool UseCardConsume(BattleCard battleCard)
    {

        if (battleCard.goldCost <= BattleManager.Instance.CurCamp.OwnGold && leftPopulation >= battleCard.populationCost)
        {
            BattleManager.Instance.CurCamp.ReduceCampGold(battleCard.goldCost);
            leftPopulation -= battleCard.populationCost;
            if (BattleManager.Instance.BATTLE_EVENT_USE_CARD!=null)
            {
                BattleManager.Instance.BATTLE_EVENT_USE_CARD();
            }
            return true;
        }
        GlobalUImanager.Instance.OpenPopTip().GetComponent<PopTip>().SetContent("资源不足，不能布置卡牌！");
        return false;


    }
    #endregion
}
