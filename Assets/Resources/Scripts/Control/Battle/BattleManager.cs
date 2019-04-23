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
    MyRound,
    AI
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
        BattleStateDictionary.Add(BattleStateEnum.AI,new AIState());

        CurBattleState = BattleStateDictionary[BattleStateEnum.None];
        CurBattleState.EnterStateWithoutMouse();
        //事件注册回调
        BATTLE_EVENT_BattleStart += SetCurCampCanAttack;
        BATTLE_EVENT_BattleStart += InitalNumOfRounds;
        BATTLE_EVENT_FinishAttackOneLand += OnFinshAttackOneLand;
        BATTLE_EVENT_MyTurnStart += SetCurCampCanAttack;
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
    private MapHeight _mapHeight;
    public MapHeight MapHeight
    {
        get { return _mapHeight; }
        set { _mapHeight = value; }
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
    //当前回合数
    private int _curNumOfRounds;
    public int CurNumOfRounds
    {
        get { return _curNumOfRounds; }
        set { _curNumOfRounds = value; }
    }
    #region 战斗中事件
    /// <summary>
    /// 战斗开始事件
    /// </summary>
    public Action BATTLE_EVENT_BattleStart;

    /// <summary>
    /// 进攻一个地块结束的事件
    /// </summary>
    public Action BATTLE_EVENT_FinishAttackOneLand;

    /// <summary>
    /// 轰炸一个地块结束的事件
    /// </summary>
    public Action BATTLE_EVENT_BOMB_ANOTHER_LAND;

    /// <summary>
    /// 购买一个高炮的事件
    /// </summary>
    public Action BATTLE_EVENT_PURCHASE_CANNON;

    /// <summary>
    /// 使用一张卡牌的事件
    /// </summary>
    public Action BATTLE_EVENT_USE_CARD;
    /// <summary>
    /// 我的回合开始事件
    /// </summary>
    public Action BATTLE_EVENT_MyTurnStart;

    /// <summary>
    /// AI回合开始
    /// </summary>
    public Action BATTLE_EVENT_AITURNStart;
    /// <summary>
    /// 回合结束的回调
    /// </summary>
    public Action BATTLE_EVENT_EndTurn;

    /// <summary>
    /// 重新选择地图事件
    /// </summary>
    public Action BATTLE_EVENT_ReSelectMap;
    #endregion
    

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
    public void GenerateMapInScene(MapHeight mapHeight, int campCount)
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

    public void SetMapInfo(MapHeight mapHeight, int campCount)
    {
        MapHeight = mapHeight;
        int mapWidth = (int)mapHeight * 16 / 9;
        GlobalUImanager.Instance.SetBaseUnitSupplyTips((int)mapHeight+1);
        MapSize = new MapSize((int)mapHeight, mapWidth);
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
        Sprite baseUnitSprite;
        string campName;
        for (int i = 0; i < CampCount - 1; i++)
        {
            campTile = Resources.Load<Tile>(tempDic[campIDs[i]].tilePath);
            baseUnitSprite = Resources.Load<Sprite>(tempDic[campIDs[i]].baseUnitSpritePath);
            campName = tempDic[campIDs[i]].campName;
            CampDic.Add(campIDs[i], new Camp(campIDs[i],campName, landCountPerCamp, campTile, baseUnitSprite));
        }
        int leftPlots = allLands - (landCountPerCamp * (CampCount - 1));
        campTile = Resources.Load<Tile>(tempDic[campIDs[CampCount - 1]].tilePath);
        baseUnitSprite = Resources.Load<Sprite>(tempDic[campIDs[CampCount - 1]].baseUnitSpritePath);
        campName = tempDic[campIDs[CampCount - 1]].campName;
        CampDic.Add(campIDs[CampCount - 1], new Camp(campIDs[CampCount - 1],campName, leftPlots, campTile,baseUnitSprite));


        //暂时设置我的阵营为第一个阵营
        MyCamp = CampDic[campIDs[0]];
        //设置当前阵营
        CurCamp = MyCamp;
    }
     

   public  IEnumerator  ReGenerateMap()
    {
        BattleReloadMapUI battleReloadMapUI = GlobalUImanager.Instance.BattleReloadMap.GetComponent<BattleReloadMapUI>();
        battleReloadMapUI.PlayAnimation();
        float time = battleReloadMapUI.GetAnimationTime();
        yield return new WaitForSeconds(time);
        foreach (Camp camp in CampDic.Values)
        {
            camp.OnRegenerateMap();
        }
        BattleMap.ReMakeMap();
        if (BATTLE_EVENT_ReSelectMap != null)
        {
            BATTLE_EVENT_ReSelectMap();
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


    #region 事件注册方法
    private void SetCurCampCanAttack()
    {
        //为了在UI上更新一些东西，比如不能进攻的时候把结束按钮高亮
        bool campCanMove = false;
        foreach(Land land in CurCamp.ownedLands)
        {
            if (land.ArmyCanMove())
            {
                campCanMove = true;
                break;
            }
        }
        //如果当前剩余的钱够一次进攻，并且至少有一个领地的兵可以动，那么可以进攻
        CanAttack = CurCamp.CampCanMove() && campCanMove;
    }
    private void InitalNumOfRounds()
    {
        CurNumOfRounds = 1;
    }

    private void OnFinshAttackOneLand()
    {
        //进攻消耗
        CurCamp.AttackLandConsumeGold();
        SetCurCampCanAttack(); 
        //todo
    }

    private void MyTurnStartAction()
    {
        //卡牌分配
        //todo
        Debug.Log("我的回合开始惹！");
    }
    #endregion
    public void TwoLandFight(Land attackLand, Land defenceLand)
    {
        StartCoroutine(CorotineTwoLandFight(attackLand, defenceLand));
    }

    BattleDicePanel _battleDicePanel;
    int _attackPoint;
    int _defencePoint;
    private  IEnumerator CorotineTwoLandFight(Land attackLand, Land defenceLand)
    {

        DicePanelParam attackLandParam = GetDicePanelParamByLand(attackLand,true);
        DicePanelParam defenceLandParam = GetDicePanelParamByLand(defenceLand, false);
        ShowDiceRollPanel(attackLandParam, defenceLandParam);
        //等界面关闭立即执行地块逻辑
        yield return new WaitWhile(()=> { return _battleDicePanel.isActiveAndEnabled; });
        //守方胜
        if (_attackPoint <= _defencePoint)
        {
            DefenceVictory(attackLand);
        }
        else
        {
            AttackVictory(attackLand,defenceLand);
        }
        //一次进攻结束的回调
        if (BATTLE_EVENT_FinishAttackOneLand != null)
        {
            BATTLE_EVENT_FinishAttackOneLand();
        }
    }

    private DicePanelParam GetDicePanelParamByLand(Land land,bool isAttack)
    {
        Camp camp = CampDic[land.CampID];
        int basicDiceRolls = land.GetBasicDiceRolls();
        //地块上的卡牌获得的额外投掷次数
        BattleCardTriggerTime diceRoll = isAttack ? BattleCardTriggerTime.ATTACK_DICE_ROLL : BattleCardTriggerTime.DEFENCE_DICE_ROLL;
        int cardExtraDiceRolls = land.GetExtraIncreaseByCard(diceRoll);
        //阵营获得的额外投掷次数（坦克，飞机等+1）
        int campExtraDiceRolls = camp.GetExtraDiceRollsAttackByLand(land,isAttack);
        int finalDiceRolls = basicDiceRolls + cardExtraDiceRolls + campExtraDiceRolls;
        //根据最终投掷次数模拟的结果
        int basicDiceRandomPoint = GetDiceRandomResult(finalDiceRolls);
        Debug.Log(camp.name+"阵营摇色子点数是"+ basicDiceRandomPoint);
        //地块上的卡牌的额外点数加成
        BattleCardTriggerTime endPoint = isAttack ? BattleCardTriggerTime.ATTACK_END_POINT : BattleCardTriggerTime.DEFENCE_END_POINT;
        int cardExtraEndPoint = land.GetExtraIncreaseByCard(endPoint);
        Debug.Log(camp.name + "阵营地块卡牌点数是" + cardExtraEndPoint);
        //阵营卡牌当回合持续buff点数加成
        int campCardExtraEndPoint = camp.hasCardBuff ? camp.CampBuffCardEffect.attackExtraEndPoint : 0;
        Debug.Log(camp.name + "阵营阵营buff点数是" + campCardExtraEndPoint);
        int finalDicePoint = basicDiceRandomPoint + cardExtraEndPoint + campCardExtraEndPoint;
        if (isAttack)
        {
            _attackPoint = finalDicePoint;
        }
        else
        {
            _defencePoint = finalDicePoint;
        }
        return new DicePanelParam(camp.name, null, finalDiceRolls, finalDicePoint);
    }
    private void ShowDiceRollPanel(DicePanelParam attackLandParam, DicePanelParam defenceLandParam)
    {
        //弹出骰子面板
        if (_battleDicePanel == null)
        {
            _battleDicePanel = GlobalUImanager.Instance.BattleDicePanel.GetComponent<BattleDicePanel>();
        }
        _battleDicePanel.RefreshAttackCampInfo(attackLandParam);
        _battleDicePanel.RefreshDefenceCampInfo(defenceLandParam);
        _battleDicePanel.ShowSelf();
    }

    private void DefenceVictory(Land attackLand)
    {      
        //给进攻失败这块地重新赋值,我炮没了
        attackLand.SetLandInfo(attackLand.CampID, 1, new Cannon());
        //替换成没有炮的tile
        //todo  
        //进攻失败，卡牌有啥表示没
        attackLand.AfterFightCardEffect(BattleCardTriggerTime.ATTACK_LOSE);
    }

    private void AttackVictory(Land attackLand,Land defenceLand)
    {
        int winLandLeftUnit = 0;
        //显示上重新绘制防守地块上的Tile
        BattleMap.ResetCampTile(defenceLand.CoordinateInMap, CampDic[attackLand.CampID].tile);
        //从防守地块列表中删除该地块
        CampDic[defenceLand.CampID].ownedLands.Remove(defenceLand);
        Cannon oldCannon = attackLand.cannon;
        //给攻下的这块地重新赋值,土遁：高炮转移术！
        winLandLeftUnit = attackLand.BattleUnit - 1;
        defenceLand.SetLandInfo(attackLand.CampID, winLandLeftUnit, oldCannon);
        //防守失败，卡牌有啥表示没？
        defenceLand.AfterFightCardEffect(BattleCardTriggerTime.DEFENCE_LOSE);
        //进攻成功，阵营有buff加成否？
        Camp camp = CampDic[attackLand.CampID];
        if (camp.hasCardBuff && camp.CampBuffCardEffect.VictoryCB != null)
        {
            camp.CampBuffCardEffect.VictoryCB(defenceLand);
        }
        //进攻地块留守一个单位
        attackLand.SetLandInfo(attackLand.CampID, 1, new Cannon());
        //替换成没有炮的tile
        //todo
        //给进攻地块列表加上攻下的这块地
        CampDic[attackLand.CampID].ownedLands.Add(defenceLand);
    }
    /// <summary>
    /// 模拟骰子随机结果
    /// </summary>
    /// <param name="diceRolls">掷骰子次数</param>
    /// <returns></returns>
    private int GetDiceRandomResult(int diceRolls)
    {
        int result = 0;
        int temp = 0;
        for (int i = 0 ; i < diceRolls; i++)
        {
            temp = UnityEngine.Random.Range(1, 7);
            //Debug.Log("骰子随机结果是"+ temp);
            result += temp;
        }
        return result;
    }

    /// <summary>
    /// 轰炸其他地块
    /// </summary>
    /// <param name="attackLand"></param>
    /// <param name="defenceLand"></param>
    public void BombAnotherLand(Land attackLand, Land defenceLand)
    {
        Camp attackCamp = CampDic[attackLand.CampID];
        Camp defenceCamp = CampDic[defenceLand.CampID];
        int bombPoint = attackCamp.CannonBombPoint;
        int leftUnit = defenceLand.BattleUnit > bombPoint ? (defenceLand.BattleUnit - bombPoint) : 1 ; 
        defenceLand.SetLandInfo(defenceLand.CampID, leftUnit);
        attackLand.cannon.isInCool = true;
        attackLand.cannon.lastFire = CurNumOfRounds;

        if (BATTLE_EVENT_BOMB_ANOTHER_LAND!=null)
        {
            BATTLE_EVENT_BOMB_ANOTHER_LAND();
        }
    }

    private int curCampIndex = 0;
    List<int> campIDs;
    /// <summary>
    /// 回合结束
    /// </summary>
    public void BattleTurnEnd()
    {
        OnTurnEnd();

        //当前回合数+1
        CurNumOfRounds++;

        //修改当前阵营为下一阵营
        curCampIndex = (curCampIndex + 1) % CampDic.Count;
        campIDs = new List<int>(CampDic.Keys);
        CurCamp = CampDic[campIDs[curCampIndex]];


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
        //    CurBattleState = BattleStateDictionary[BattleStateEnum.AI];
        //    CurBattleState.EnterState();
        //}

        if (BATTLE_EVENT_EndTurn != null)
        {
            BATTLE_EVENT_EndTurn();
        }
    }

    private void OnTurnEnd()
    {
      
        //补给当前阵营各地块
        foreach (Land land in CurCamp.ownedLands)
        {      
            land.EndTurnSupply();         
        }
        List<GameObject> BaseUnitSupplyTips = GlobalUImanager.Instance.BaseUnitSupplyTips;
        int index = 0;
        foreach (GameObject BaseUnitSupplyTip in BaseUnitSupplyTips)
        {
            BattleBaseUnitSupplyTip battleBaseUnitSupplyTip = BaseUnitSupplyTip.GetComponent<BattleBaseUnitSupplyTip>();
            if (index < CurCamp.ownedLands.Count)
            {
                battleBaseUnitSupplyTip.SetImage(CurCamp.baseUnitSprite);
                battleBaseUnitSupplyTip.SetPosition(CurCamp.ownedLands[index].CoordinateInMap);
                index++;
            }
            else
            {
                break;
            }
        }
        //遍历该阵营下所有地块，结算
        Debug.Log("当前结算阵营" + CurCamp.name);
        Cannon cannon;
        foreach (Land land in CurCamp.ownedLands)
        {
            //当前阵营的炮，重置冷却
            cannon = land.cannon;
            if (cannon.isOwned && cannon.isInCool)
            {
                land.cannon.isInCool = !((CurNumOfRounds - cannon.lastFire) >= CampDic.Count);
            }
            //进攻效果的卡牌都去掉
            if (land.BattleCard!=null)
            {
                BattleCardTriggerTime landCardTrigger = land.BattleCard.triggerTime;
                if (landCardTrigger.Equals(BattleCardTriggerTime.ATTACK_DICE_ROLL)|| landCardTrigger.Equals(BattleCardTriggerTime.ATTACK_DICE_ROLL))
                {
                    land.BattleCard = null;
                       
                }
            }
        }
        //当前阵营的卡牌buff取消
        CurCamp.ResetCardEffect();
        SupplyCard();


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

    #region 战斗卡牌函数
    public void OnClickCard(int cardID)
    {
        if (CurBattleState is MyRoundState)
        {
            MyRoundState myRound = CurBattleState as MyRoundState;
            myRound.CurAttackState.OnClickCard(cardID); 
        }
       
    }

    /// <summary>
    /// 补给卡牌
    /// </summary>
    public void SupplyCard()
    {    
        if (CurCamp == MyCamp)
        {
            BattleCardManager.Instance.SupplyCard();
        }
    }
    #endregion

}
