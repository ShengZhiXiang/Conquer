using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAttackState : AttackStateBase {

    public CardAttackState(MyRoundState myRound) : base(myRound) { }

    public override void ClickAction(Land clickLand)
    {
        base.ClickAction(clickLand);
        BattleCardUI CurCard = BattleCardManager.Instance.CurSelectCard;
        //如果这块地上有牌了，就直接返回
        if (clickLand.HasCard() && !(CurCard.triggerTime == BattleCardTriggerTime.IMMEDIATELY))
        {
            MyRound.CurAttackState = MyRound.AttackStateDic[AttackStateEnum.NoneAttack];
            GlobalUImanager.Instance.OpenPopTip().GetComponent<PopTip>().SetContent("该地已经有卡牌了");
            return;
        }  
        bool curLandIsMyCampLand = clickLand.CampID == BattleManager.Instance.CurCamp.campID;
        //如果卡牌放的位置是对的，并且剩余资源足够使用卡牌则放牌，否则跳过
        if (CurCard != null && !(CurCard.isSelfCard ^ curLandIsMyCampLand))
        {
            if (clickLand.LandCanUseCard(CurCard))
            {
                clickLand.UseCard(CurCard);
                BattleCardManager.Instance.DestroyCard();
            }
            else
            {
                GlobalUImanager.Instance.OpenPopTip().GetComponent<PopTip>().SetContent("资源不足，不能布置卡牌！");
                BattleCardManager.Instance.CancelSelectCard();
            }       
        }
        else
        {
            BattleCardManager.Instance.CancelSelectCard();
            Debug.Log("收回卡牌！！！");
        }

        MyRound.CurAttackState = MyRound.AttackStateDic[AttackStateEnum.NoneAttack];
        

    }
    public override void OnClickCard()
    {
        base.OnClickCard();
        if (BattleCardManager.Instance.CurSelectCard == null)
        {
            MyRound.CurAttackState = MyRound.AttackStateDic[AttackStateEnum.NoneAttack];
        }
        
    }
    public override void UpdateFunc()
    {
        base.UpdateFunc();
    }
}
