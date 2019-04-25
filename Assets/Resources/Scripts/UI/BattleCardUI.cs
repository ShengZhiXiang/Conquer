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

    public void InitCardInfo(BattleCard battleCard ,int arrayIndex)
    {
        this.cardId = battleCard.ID;
        BG.sprite = battleCard.sprite;
        this.goldCost.text = battleCard.goldCost.ToString();
        this.cardName.text = battleCard.cardName;
        this.arrayIndex = arrayIndex;
    }

    public override void Initial()
    {
        base.Initial();
        UGUIEventListener.Get(gameObject).onClick = delegate() 
        {
            BattleCardManager.Instance.OnClickCard(arrayIndex);
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
