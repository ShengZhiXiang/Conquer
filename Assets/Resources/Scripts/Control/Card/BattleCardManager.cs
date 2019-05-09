using System;
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
    ATOMIC_BOMB,
    COMMUNIST,
    BLITZ,
    MILITARISM,
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
    private BattleCardUI _curSelectCard;
    public BattleCardUI CurSelectCard
    {
        get { return _curSelectCard;}
        set { _curSelectCard = value;}
    }
    public static readonly int BasicCardStartIndex = 3001;
    public static readonly int BasicCardEndIndex = 3005;
    public Dictionary<BattleCardFuncEnum, Func<Land, int>> CardEnum_FuncDic; 
    public void Inital()
    {
        CardEnum_FuncDic = new Dictionary<BattleCardFuncEnum, Func<Land, int>>();
        CardEnum_FuncDic.Add(BattleCardFuncEnum.AIR_RAID, AirRaid);
        CardEnum_FuncDic.Add(BattleCardFuncEnum.DEFENCE_ADD_ROLL, DefenceAddRoll);
        CardEnum_FuncDic.Add(BattleCardFuncEnum.DEFENCE_ADD_POINT, DefenceAddPoint);
        CardEnum_FuncDic.Add(BattleCardFuncEnum.ATTACK_ADD_ROLL, AttackAddRoll);
        CardEnum_FuncDic.Add(BattleCardFuncEnum.DEFENCE_LOSE_REDUCE_POPULATION, DefenceLoseReducePopulation);

        RegisteEvent();
    }

    private void RegisteEvent()
    {
        BattleManager.Instance.BATTLE_EVENT_EndTurn += HideBattleCards;
        BattleManager.Instance.BATTLE_EVENT_MyTurnStart += ShowBattleCards;
        BattleManager.Instance.BATTLE_EVENT_AITURNStart += ShowBattleCards;
    }


    
    #region 卡牌操作函数
    public void OnClickCard(int arrayIndex)
    {
        List<BattleCardUI> CurCampCards = BattleManager.Instance.CurCamp.BattleCardUIs;
        //如果当前没有选中的卡牌，则设置一下当前选中的卡牌
        if (CurSelectCard == null)
        {
            OnSelectCard(arrayIndex);
        }
        //否则如果点的是同一张卡牌，则视为取消选中
        else if (CurSelectCard.arrayIndex == arrayIndex)
        {
            CancelSelectCard();
        }
        //点的是另一张卡牌
        else
        {
            CancelSelectCard();
            OnSelectCard(arrayIndex);      
        }
        BattleManager.Instance.OnClickCard();
        

    }
    public void OnSelectCard(int arrayIndex)
    {
        List<BattleCardUI> CurCampCards = BattleManager.Instance.CurCamp.BattleCardUIs;
        CurCampCards[arrayIndex].PopSelf();
        CurSelectCard = CurCampCards[arrayIndex];
        BattleManager.Instance.SetCampLandsHighLight(CurSelectCard.isSelfCard);
    }

    public void CancelSelectCard()
    {
        List<BattleCardUI> CurCampCards = BattleManager.Instance.CurCamp.BattleCardUIs;
        if (CurSelectCard==null)
        {
            return;
        }
        CurCampCards[CurSelectCard.arrayIndex].BackToNormal();
        BattleManager.Instance.SetCampLandsHighLight(CurSelectCard.isSelfCard, false);
        CurSelectCard = null; 
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
        List<BattleCardUI> CurCampCards = BattleManager.Instance.CurCamp.BattleCardUIs;
        if (CurCampCards.Count<6)
        {
            BattleCardUI battleCardUI = InstantiateBattleCardUI();
            bool supplyCampCard = UnityEngine.Random.value > 0.5f;
            int arrayIndex = CurCampCards.Count;
            if (supplyCampCard)
            {
                //补给阵营专属牌
                Camp curCamp = BattleManager.Instance.CurCamp;          
                int startIndex = curCamp.cardStartIndex;
                int endindex = curCamp.cardEndIndex;    
                CardModel cardModel = GetRandomCardModelByIndex(startIndex, endindex);             
                BattleCardFuncEnum funcEnum = (BattleCardFuncEnum)Enum.Parse(typeof(BattleCardFuncEnum), cardModel.cardFuncEnum);
                battleCardUI.InitCardInfo(cardModel, arrayIndex, curCamp.CardEnum_FuncDic[funcEnum]);
            }
            else
            {
                //补给基础牌
                CardModel cardModel = GetRandomCardModelByIndex(BasicCardStartIndex, BasicCardEndIndex);
                BattleCardFuncEnum funcEnum = (BattleCardFuncEnum)Enum.Parse(typeof(BattleCardFuncEnum), cardModel.cardFuncEnum);
                battleCardUI.InitCardInfo(cardModel, arrayIndex, CardEnum_FuncDic[funcEnum]);
            }                    
            CurCampCards.Add(battleCardUI);           
        }
    }
    private BattleCardUI InstantiateBattleCardUI()
    {
        GameObject temp = Resources.Load<GameObject>("Prefabs/UI/BattleCard");
        GameObject cardListNode = GlobalUImanager.Instance.OpenUI(UIEnum.BattleMainPanel).GetComponent<BattleMainPanel>()._cardList;
        GameObject card = Instantiate(temp, cardListNode.transform);
        return card.GetComponent<BattleCardUI>();
    } 
    private CardModel GetRandomCardModelByIndex(int cardStartIndex,int cardEndIndex)
    {
        int cardId = UnityEngine.Random.Range(cardStartIndex, cardEndIndex + 1);
        CardModel cardModel = GameDataSet.Instance.CardModelDic[cardId];
        return cardModel;
    }

    public void DestroyCard()
    {
        List<BattleCardUI> CurCampCards = BattleManager.Instance.CurCamp.BattleCardUIs;
        Destroy(CurSelectCard.gameObject);
        CurCampCards.Remove(CurSelectCard);
        //重新赋值下顺序
        for (int i = 0 ; i < CurCampCards.Count ; i++)
        {
            CurCampCards[i].arrayIndex = i ;
        }
        bool isMycampCard = CurSelectCard.isSelfCard;
        CurSelectCard = null;

        BattleManager.Instance.SetCampLandsHighLight(isMycampCard, false);
    }

    private void HideBattleCards()
    {
        List<BattleCardUI> CurCampCards = BattleManager.Instance.CurCamp.BattleCardUIs;
        foreach (BattleCardUI battleCardUI in CurCampCards)
        {
            battleCardUI.gameObject.SetActive(false);
        } 
    }

    private void ShowBattleCards()
    {
        List<BattleCardUI> CurCampCards = BattleManager.Instance.CurCamp.BattleCardUIs;
        Debug.Log(BattleManager.Instance.CurCamp.ToString());
        Debug.Log(BattleManager.Instance.CurCamp.BattleCardUIs.Count.ToString());
        foreach (BattleCardUI battleCardUI in CurCampCards)
        {
            battleCardUI.gameObject.SetActive(true);
        }
    }

    #region 卡牌方法函数
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
        BattleManager.Instance.CurCamp.AddCampGold(curCamp.AttackConsumeGold * 2);
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
    #endregion

}
