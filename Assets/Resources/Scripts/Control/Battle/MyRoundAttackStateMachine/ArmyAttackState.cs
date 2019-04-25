using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyAttackState : AttackStateBase {
    public ArmyAttackState(MyRoundState myRound) : base(myRound)
    {

    }
    public override void ClickAction(Land clickLand)
    {
        base.ClickAction(clickLand);
        if (BattleManager.Instance.IsTwoLandNeighbor(clickLand.CoordinateInMap, attackLand.CoordinateInMap) &&
            clickLand.CampID != attackLand.CampID &&
            BattleManager.Instance.CanAttack)
        {
            BattleManager.Instance.TwoLandFight(attackLand, clickLand);          
        }
        CancelHighlight();
        //Debug.Log("军队进攻状态下的取消操作");
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
