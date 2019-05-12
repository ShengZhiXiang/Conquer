using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAttackState : AttackStateBase {
    public BombAttackState(MyRoundState myround) : base(myround) { }
    public override void ClickAction(Land clickLand)
    {
        base.ClickAction(clickLand);
        if (BattleManager.Instance.IsTwoLandNeighbor(clickLand.CoordinateInMap, attackLand.CoordinateInMap) &&
            clickLand.CampID != attackLand.CampID)
        {
            if (BattleManager.Instance.CurCamp.CanCannonAttack())
            {
                BattleManager.Instance.BombAnotherLand(attackLand, clickLand);
            }
            else
            {
                GlobalUImanager.Instance.OpenPopTip().GetComponent<PopTip>().SetContent("没钱轰他娘的！");
            }
           
        }
        CancelHighlight();
        //Debug.Log("轰炸状态下的取消操作");
        MyRound.CurAttackState = MyRound.AttackStateDic[AttackStateEnum.NoneAttack];
    }
    public override void UpdateFunc()
    {
        base.UpdateFunc();
    }

    public override void OnClickCard()
    {
        base.OnClickCard();
        CancelHighlight();
        MyRound.CurAttackState = MyRound.AttackStateDic[AttackStateEnum.CardAttack];
    }

}
