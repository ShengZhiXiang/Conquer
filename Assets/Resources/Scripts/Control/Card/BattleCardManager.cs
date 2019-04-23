﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BattleCardFuncEnum
{
    BOMB,
    AIR_RAID,
    DEFENCE_ADD_ROLL,
    DEFENCE_ADD_POINT,
    ATTACK_ADD_ROLL,
    ATTACK_ADD_POINT,
    DEFENCE_LOSE_REDUCE_POPULATION,
    ATTACK_LOSE_ADD_GOLD,
    ATTACK_CONSUME_REDUCE,
    ATTACK_CAMP_EXTRA_POINT,
    NATIONALISM,
}

public enum BattleCardTriggerTime
{
    IMMEDIATELY,
    DEFENCE_DICE_ROLL, 
    DEFENCE_END_POINT,
    ATTACK_DICE_ROLL,
    ATTACK_END_POINT,
    DEFENCE_LOSE,
    ATTACK_LOSE,
}

public class BattleCardManager : Singleton<BattleCardManager> {
    private BattleCard _curSelectCard;
    public BattleCard CurSelectCard
    {
        get { return _curSelectCard;}
        set { _curSelectCard = value;}
    }

    private Dictionary<int,BattleCardUI> _battleCardUIs;
    public Dictionary<int, BattleCardUI> BattleCardUIs
    {
        get { return _battleCardUIs; }
        set { _battleCardUIs = value; }
    }
    public Dictionary<int, BattleCard> battleCardDic;
    public Dictionary<BattleCardFuncEnum, Func<Land, int>> CardEnum_FuncDIc;
    public void Inital()
    {
        CardEnum_FuncDIc = new Dictionary<BattleCardFuncEnum, Func<Land, int>>();
        CardEnum_FuncDIc.Add(BattleCardFuncEnum.AIR_RAID, AirRaid);
        CardEnum_FuncDIc.Add(BattleCardFuncEnum.DEFENCE_ADD_ROLL, DefenceAddRoll);
        CardEnum_FuncDIc.Add(BattleCardFuncEnum.DEFENCE_ADD_POINT, DefenceAddPoint);
        CardEnum_FuncDIc.Add(BattleCardFuncEnum.ATTACK_ADD_ROLL, AttackAddRoll);
        CardEnum_FuncDIc.Add(BattleCardFuncEnum.DEFENCE_LOSE_REDUCE_POPULATION, DefenceLoseReducePopulation);
        //CardEnum_FuncDIc.Add(BattleCardFuncEnum.ATTACK_LOSE_ADD_GOLD, AttackLoseAddGold);
        //CardEnum_FuncDIc.Add(BattleCardFuncEnum.ATTACK_CONSUME_REDUCE,AttackConsumeReduce);
        //CardEnum_FuncDIc.Add(BattleCardFuncEnum.ATTACK_CAMP_EXTRA_POINT ,AttackFinalExtraPoint);
        //CardEnum_FuncDIc.Add(BattleCardFuncEnum.NATIONALISM,NationNalism);
        InitBattleCardDic();
    }

    private void InitBattleCardDic()
    {
          battleCardDic = new Dictionary<int, BattleCard>();
          BattleCardUIs = new Dictionary<int, BattleCardUI>();

        Sprite sprite;
        foreach (CardModel cardModel in GameDataSet.Instance.CardModelDic.Values)
        {
            BattleCardFuncEnum funcEnum = (BattleCardFuncEnum)Enum.Parse(typeof(BattleCardFuncEnum), cardModel.cardFuncEnum);
            bool isSelfCard = cardModel.isSelfCard == 1 ? true : false;
            BattleCardTriggerTime triggerTime = (BattleCardTriggerTime)Enum.Parse(typeof(BattleCardTriggerTime), cardModel.cardTriggerTime);
            sprite = Resources.Load<Sprite>(cardModel.spritePath);
            BattleCard battleCard = new BattleCard(cardModel.cardID,cardModel.cardName,sprite,funcEnum, triggerTime, isSelfCard, cardModel.costGold, cardModel.costPopulation);
            battleCard.CardFunc = CardEnum_FuncDIc[battleCard.funcEnum];
            battleCardDic.Add(battleCard.ID, battleCard);
        }

        //CurSelectCard = battleCardDic[3005];

    }
    #region 卡牌操作函数
    public void OnClickCard(int cardID)
    {
        //如果当前没有选中的卡牌，则设置一下当前选中的卡牌
        if (CurSelectCard == null)
        {
            CurSelectCard = battleCardDic[cardID];
            //弹出卡牌     
            BattleCardUIs[cardID].PopSelf();
        }
        //否则如果点的是同一张卡牌，则视为取消选中
        else if (CurSelectCard.ID == cardID)
        {
            BattleCardUIs[cardID].BackToNormal();
            CurSelectCard = null;
        }
        //点的是另一张卡牌
        else
        {
            BattleCardUIs[CurSelectCard.ID].BackToNormal();
            BattleCardUIs[cardID].PopSelf();
            CurSelectCard = battleCardDic[cardID];
        }
        BattleManager.Instance.OnClickCard(cardID);

    }
    public void OnMouseEnterCard(int cardID)
    {

    }

    public void OnMouseExitCard(int cardID)
    {

    }
    #endregion
    public void SupplyCard()
    {
        if (BattleCardUIs.Count<6)
        {
            GameObject temp = Resources.Load<GameObject>("Prefabs/UI/BattleCard");
            GameObject cardListNode = GlobalUImanager.Instance.OpenUI(UIEnum.BattleMainPanel).GetComponent<BattleMainPanel>()._cardList;
            GameObject card = Instantiate(temp, cardListNode.transform);
            BattleCardUI battleCardUI = card.GetComponent<BattleCardUI>();
            int index = UnityEngine.Random.Range(3001, 3006);
            BattleCard battleCard = battleCardDic[index];
            battleCardUI.InitCardInfo(battleCard);
            BattleCardUIs.Add(battleCard.ID, battleCardUI);
        }
    }
    public void DestroyCard()
    {
       
        Destroy(BattleCardUIs[CurSelectCard.ID].gameObject);
        BattleCardUIs.Remove(CurSelectCard.ID);
        CurSelectCard = null; 
    }
    private int AirRaid(Land land)
    {
        land.BattleUnit = 1;
        return 0;
    }

    private int DefenceAddRoll(Land land)
    {
        return 3;
    }

    private int DefenceAddPoint(Land land)
    {
        return 6;
    }

    private int AttackAddRoll(Land land)
    {
        return 4;
    }
    private int DefenceLoseReducePopulation(Land land)
    {
        land.BattleUnit -= 3;
        //不能全给轰死嚎
        if (land.BattleUnit <= 0)
        {
            land.BattleUnit = 1;
        }
        return 0;
    }

    private int AttackLoseAddGold(Land land)
    {
        Camp curCamp = BattleManager.Instance.CurCamp;
        BattleManager.Instance.CurCamp.AddCampGold(curCamp.attackConsumeGold * 2);
        return 0;
    }

    private int AttackConsumeReduce(Land land)
    {
        BattleManager.Instance.CurCamp.hasCardBuff = true;
        BattleManager.Instance.CurCamp.CampBuffCardEffect.AttackConsumeReduce += 1;
        return 0;
    }
    private int AttackFinalExtraPoint(Land land)
    {
        BattleManager.Instance.CurCamp.hasCardBuff = true;
        BattleManager.Instance.CurCamp.CampBuffCardEffect.attackExtraEndPoint += 2;
        return 0;
    }
    private  int NationNalism(Land land)
    {
        BattleManager.Instance.CurCamp.hasCardBuff = true;
        BattleManager.Instance.CurCamp.CampBuffCardEffect.VictoryCB += delegate(Land takeLand) 
        {
            takeLand.leftPopulation += 1;
        };
        return 0;
    }

}