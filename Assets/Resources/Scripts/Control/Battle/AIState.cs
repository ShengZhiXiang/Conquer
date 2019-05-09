using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AIState : BattleStateBase {
    List<Land> _orderedCurCampLands = new List<Land>();
    public override void EnterState()
    {
        base.EnterState();
        if (BattleManager.Instance.BATTLE_EVENT_AITURNStart!=null)
        {
            BattleManager.Instance.BATTLE_EVENT_AITURNStart();
        }
        OrderCampLands();

        
    }
    private void OrderCampLands()
    {
        _orderedCurCampLands = BattleManager.Instance.CurCamp.ownedLands;
        Land weakLand;
        //冒泡排序把最大的放到前面
        for (int i = 0 ; i < _orderedCurCampLands.Count - 1 ; i++)
        {
            for (int j = i + 1 ; j < _orderedCurCampLands.Count ; j++)
            {
                
                if (_orderedCurCampLands[j].BattleUnit > _orderedCurCampLands[i].BattleUnit)
                {
                    //换位置
                    weakLand = _orderedCurCampLands[i];
                    _orderedCurCampLands[i] = _orderedCurCampLands[j];
                    _orderedCurCampLands[j] = weakLand;
                }
            }
        }

        foreach(Land land in _orderedCurCampLands)
        {
            Debug.Log("地块坐标是"+land.CoordinateInMap+"军队数量是"+land.BattleUnit);
        }

    }

    private void CampUseCard()
    {
        foreach (BattleCardUI battleCardUI in BattleManager.Instance.CurCamp.BattleCardUIs)
        {
            switch (battleCardUI.triggerTime)
            {
                case BattleCardTriggerTime.IMMEDIATELY:
                    if (battleCardUI.isSelfCard)
                    {
                        //随便找个地方放
                    }
                    else
                    {
                        //找敌军放
                    }
                    break;
                case BattleCardTriggerTime.DEFENCE_DICE_ROLL:
                case BattleCardTriggerTime.DEFENCE_END_POINT:
                case BattleCardTriggerTime.DEFENCE_LOSE:
                    //在自己防守薄弱的地方放
                    break;
                case BattleCardTriggerTime.ATTACK_DICE_ROLL:
                case BattleCardTriggerTime.ATTACK_END_POINT:
                case BattleCardTriggerTime.ATTACK_LOSE:
                    //在自己进攻牛逼的地方放
                    break;
                default:
                    break;
            }
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
