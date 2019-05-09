using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAttackState : AttackStateBase {

    public CardAttackState(MyRoundState myRound) : base(myRound) { }

    public override void ClickAction(Land clickLand)
    {
        base.ClickAction(clickLand);

        //如果这块地上有牌了，就直接返回
        if (clickLand.BattleCardUI!=null)
        {
            MyRound.CurAttackState = MyRound.AttackStateDic[AttackStateEnum.NoneAttack];
            Debug.Log("这块地上只能有一张牌");
            return;
        }
        BattleCardUI CurCard =  BattleCardManager.Instance.CurSelectCard;
        bool curLandIsMyCampLand = clickLand.CampID == BattleManager.Instance.CurCamp.campID;
        //如果卡牌放的位置是对的，并且剩余资源足够使用卡牌则放牌，否则跳过
        if (CurCard != null && !(CurCard.isSelfCard ^ curLandIsMyCampLand) && clickLand.UseCardConsume(CurCard))
        {
            if (CurCard.triggerTime.Equals(BattleCardTriggerTime.IMMEDIATELY))
            {
                CurCard.CardFunc(clickLand);
            }
            else
            {
                clickLand.BattleCardUI = CurCard;
            }
            BattleCardManager.Instance.DestroyCard();
            Debug.Log("使用成功");
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
        MyRound.CurAttackState = MyRound.AttackStateDic[AttackStateEnum.NoneAttack];
    }
    public override void UpdateFunc()
    {
        base.UpdateFunc();
    }
}
