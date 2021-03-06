﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCardUI : UINode {
    [SerializeField]
    public Image BG;
    [SerializeField]
    public Text goldCost;
    [SerializeField]
    public Text cardName;

    public RectTransform rectTransform;

    public int cardId;
    public int arrayIndex;
    public bool isSelfCard;
    public BattleCardTriggerTime triggerTime;
    public CardEffectType cardEffectType;
    public Func<Land, int> CardFunc;

    public void InitCardInfo(CardModel cardModel,int arrayIndex,Func<Land,int> CardFunc)
    {
        this.cardId = cardModel.cardID;
        BG.sprite = Resources.Load<Sprite>(cardModel.spritePath);
        goldCost.text = cardModel.costGold.ToString();
        cardName.text = cardModel.cardName;
        isSelfCard = cardModel.isSelfCard == 1;
        this.arrayIndex = arrayIndex;
        triggerTime = (BattleCardTriggerTime)Enum.Parse(typeof(BattleCardTriggerTime), cardModel.cardTriggerTime);
        this.CardFunc = CardFunc;
        cardEffectType = (CardEffectType)Enum.Parse(typeof(CardEffectType), cardModel.cardEffectType);
    }

    private bool canClick ;
    public void SetCanClick(bool canClick)
    {
        this.canClick = canClick;
    }
    public override void Initial()
    {
        base.Initial();
        UGUIEventListener.Get(gameObject).onClick = delegate() 
        {
            if (canClick)
            {
                BattleCardManager.Instance.OnClickCard(arrayIndex);
            }          
        };

        UGUIEventListener.Get(gameObject).onEnter = delegate ()
        {
            BattleCardManager.Instance.OnMouseEnterCard(arrayIndex);
        };

        UGUIEventListener.Get(gameObject).onExit = delegate ()
        {
            BattleCardManager.Instance.OnMouseExitCard(arrayIndex);
        };
        
    }
    private void SetPosition(Vector2 pos)
    {
        rectTransform.anchoredPosition = pos;
    }

    public void PopSelf()
    {
        SetPosition(new Vector2(0f,30f));
    }
    public void BackToNormal()
    {
        SetPosition(Vector2.zero);
    }
  
}
