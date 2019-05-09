using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public enum CAMP_NAME
{
    USA,
    GERMANY,
    JAPAN,
    SOVIET,
}

public struct CampBuffCardEffect
{
    //进攻时额外增加的最终点数
    public int attackExtraEndPoint;
    //轰炸操作减少的消耗金币
    public int BombConsumeReduce;
    //普通进攻减少的消耗金币
    public int AttackConsumeReduce;
    //回合结束时额外补给的兵力
    public int ExtraSupplyUnit;
    //攻下一个地块胜利之后的回调
    public Action<Land> VictoryCB;

    public void Reset()
    {
        attackExtraEndPoint = 0;
        BombConsumeReduce = 0;
        AttackConsumeReduce = 0;
        ExtraSupplyUnit = 0;
        VictoryCB = null;
    }
}
public class Camp  {
    
    //阵营ID
    public int campID;
    //阵营名字
    public string name;
    //阵营拥有的地块List
    public List<Land> ownedLands;
    //初始分到的地块数
    public int initialLands;
    public int OwnGold { get; private set; }
    //是否有卡牌效果的buff
    public bool hasCardBuff;
    //卡牌效果额外buff
    public CampBuffCardEffect CampBuffCardEffect = new CampBuffCardEffect();
    //该阵营使用的tile
    public Tile tile;
    //该阵营的基本士兵图片
    public Sprite baseUnitSprite;
    //玩家名字
    public string PlayerName;

    public int cardStartIndex;
    public int cardEndIndex;
    //阵营特殊卡牌枚举-方法表
    public Dictionary<BattleCardFuncEnum, Func<Land, int>> CardEnum_FuncDic;
    
    /// <summary>
    /// 该阵营在对战中拥有的卡牌列表
    /// </summary>
    private List<BattleCardUI> _battleCardUIs;
    public List<BattleCardUI> BattleCardUIs
    {
        get { return _battleCardUIs; }
        set { _battleCardUIs = value; }
    }
    public Camp(int campID, string name, int initialLands, Tile tile, Sprite baseUnitSprite ,string PlayerName,int cardStartIndex,int cardEndIndex)
    {
        this.campID = campID;
        this.name = name;
        this.tile = tile;
        this.initialLands = initialLands;
        this.baseUnitSprite = baseUnitSprite;
        this.PlayerName = PlayerName;
        this.cardStartIndex = cardStartIndex;
        this.cardEndIndex = cardEndIndex;
        ownedLands = new List<Land>();
        CardEnum_FuncDic = new Dictionary<BattleCardFuncEnum, Func<Land, int>>();
        BattleCardUIs = new List<BattleCardUI>();
    }
    #region 子类阵营需要重写的数值
    //该阵营军队进攻一次所需要的金币
    public virtual int AttackConsumeGold
    {
        get { return 2; }
    }
    //该阵营买高炮所需要的金币
    public virtual int BuyCannonCoumeGold
    {
        get { return 5; }
    }
    //该阵营用高炮轰炸一次需要的金币
    public virtual int CannonAttackConsumGold
    {
        get { return 3; }
    }
    //高炮轰炸一次炸掉的单位数
    public virtual int CannonBombPoint
    {
        get { return UnityEngine.Random.Range(2, 5); }
    }
    #endregion
    #region 公用方法
    public void OnRegenerateMap()
    {
        OwnGold = 0;
    }
    public void AddCampGold(int addGold)
    {
        OwnGold += addGold;
    }
    public void ReduceCampGold(int reduceGold)
    {
        OwnGold -= reduceGold;
    }

    public bool CampCanMove()
    {
        int realConsumeGold = hasCardBuff ? AttackConsumeGold - CampBuffCardEffect.AttackConsumeReduce : AttackConsumeGold;
        return OwnGold >= realConsumeGold;
    }
    public bool PurchaseCannon()
    {
        if (OwnGold > BuyCannonCoumeGold)
        {
            ReduceCampGold(BuyCannonCoumeGold);
            return true;
        }
        return false;      
    }
    public void AttackLandConsumeGold()
    {
        int realConsumeGold = hasCardBuff ? AttackConsumeGold - CampBuffCardEffect.AttackConsumeReduce : AttackConsumeGold;
        ReduceCampGold(realConsumeGold);
    }
    public bool CannonAttack()
    {
        int realConsumeGold = hasCardBuff ? CannonAttackConsumGold - CampBuffCardEffect.BombConsumeReduce : CannonAttackConsumGold;
        if (OwnGold > realConsumeGold)
        {
            ReduceCampGold(realConsumeGold);
            return true;
        }
        return false;
    }
    /// <summary>
    /// 每个回合结束时把阵营持续进攻buff去掉
    /// </summary>
    public void ResetCardEffect()
    {
        hasCardBuff = false;
        CampBuffCardEffect.Reset();
    }

    #endregion
    #region 子类阵营需要重写的虚方法
    /// <summary>
    /// 根据Land获取额外掷骰子的次数
    /// </summary>
    /// <param name="land"></param>
    /// <returns></returns>
    public virtual int GetExtraDiceRollsAttackByLand(Land land,bool isAttack)
    {
        int result = 0;
        if (isAttack)
        {
            if (land.ArmyType == ArmyType.TankWithUnit)
            {
                result += 1;
            }
            else if (land.ArmyType == ArmyType.AirPlaneWithUnit)
            {
                result += 2;
            }
        }
        else
        {
            result = 0;
        }     
        
        return result ;
    }

    #endregion


}
