using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AIState : BattleStateBase {

    public override void EnterState()
    {
        base.EnterState();
        if (BattleManager.Instance.BATTLE_EVENT_AITURNStart!=null)
        {
            BattleManager.Instance.BATTLE_EVENT_AITURNStart();
        }
        
    }

    public override void ExitState(ref UnityAction mapEnterAction, ref UnityAction<Land, PointerEventData> mapClickAction, ref UnityAction mapExitAction)
    {
        base.ExitState(ref mapEnterAction, ref mapClickAction, ref mapExitAction);
    }

    public override void OnUpdateFunc()
    {
        base.OnUpdateFunc();
    }

    public override void RefreshUI()
    {
        base.RefreshUI();
    }
}
