using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct CampBuffCardEffect
{
    //进攻时额外增加的最终点数
    public int attackExtraEndPoint;
    //轰炸操作减少的消耗金币
    public int BombConsumeReduce;
    //普通进攻减少的消耗金币
    public int AttackConsumeReduce;
    //攻下一个地块胜利之后的回调
    public Action<Land> VictoryCB;

    public void Reset()
    {
        attackExtraEndPoint = 0;
        BombConsumeReduce = 0;
        AttackConsumeReduce = 0;
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
    //当前阵营拥有的总金币
    private int _ownedGold;
    public int OwnGold
    {
        get { return _ownedGold; }
        private set { _ownedGold = value; }
    }
    //该阵营军队进攻一次所需要的金币
    public int attackConsumeGold = 2;
    //该阵营买高炮所需要的金币
    public int buyCannonConsumeGold = 5;
    //该阵营用高炮轰炸一次需要的金币
    public int cannonBombConsumeGold = 2;
    //是否有卡牌效果的buff
    public bool hasCardBuff;
    //卡牌效果额外buff
    public CampBuffCardEffect CampBuffCardEffect = new CampBuffCardEffect();
    //该阵营使用的tile
    public Tile tile;
    //该阵营的基本士兵图片
    public Sprite baseUnitSprite;
    public Camp(int campID,string name, int initialLands, Tile tile,Sprite baseUnitSprite)
    {
        this.campID = campID;
        this.name = name;
        this.tile = tile;
        this.initialLands = initialLands;
        this.baseUnitSprite = baseUnitSprite;
        ownedLands = new List<Land>();
    }

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
        int realConsumeGold = hasCardBuff ? attackConsumeGold - CampBuffCardEffect.AttackConsumeReduce : attackConsumeGold;
        return OwnGold >= realConsumeGold;
    }
    public bool PurchaseCannon()
    {
        if (OwnGold > buyCannonConsumeGold)
        {
            ReduceCampGold(buyCannonConsumeGold);
            return true;
        }
        return false;      
    }
    public void AttackLandConsumeGold()
    {
        int realConsumeGold = hasCardBuff ? attackConsumeGold - CampBuffCardEffect.AttackConsumeReduce : attackConsumeGold;
        ReduceCampGold(realConsumeGold);
    }
    public bool CannonAttack()
    {
        int realConsumeGold = hasCardBuff ? cannonBombConsumeGold - CampBuffCardEffect.BombConsumeReduce : cannonBombConsumeGold;
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
    //高炮轰炸一次炸掉的单位数
    public int CannonBombPoint = 3;
    //以后用实现虚方法的方式实现根据阵营不同，掷骰子增益不同的效果
    /// <summary>
    /// 根据Land获取额外掷骰子的次数
    /// </summary>
    /// <param name="land"></param>
    /// <returns></returns>
    public int GetExtraDiceRollsAttackByLand(Land land,bool isAttack)
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

    

    
}
