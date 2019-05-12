using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AIState : BattleStateBase {
    List<Land> _orderedCurCampLands = new List<Land>();
    private static readonly float POP_CARD_TIME = 1.0f;
    private static readonly float PLACE_CARD_TIME = 1.0f;
    private static readonly float OPERATE_CANNON_TIME = 1.5f;
    List<int> toDeleteCardIndexList = new List<int>();
    public override void EnterState()
    {
        base.EnterState();
        if (BattleManager.Instance.BATTLE_EVENT_AITURNStart!=null)
        {
            BattleManager.Instance.BATTLE_EVENT_AITURNStart();
        }
        OrderCampLands();
        BattleManager.Instance.StartBattleCoroutine(CampUseCard());
        BattleManager.Instance.StartBattleCoroutine(LandCannonOperate());


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

    IEnumerator  CampUseCard()
    {
        if (toDeleteCardIndexList.Count > 0)
        {
            toDeleteCardIndexList.Clear();
        }
        Camp CurCamp = BattleManager.Instance.CurCamp;
        foreach (BattleCardUI battleCardUI in CurCamp.BattleCardUIs)
        {
            //卡牌弹起
            battleCardUI.PopSelf();
            yield return new WaitForSeconds(POP_CARD_TIME);
            Land selectLand =  GetCardPlaceLandByType(battleCardUI);
            if (selectLand == null)
            {
                battleCardUI.BackToNormal();
            }
            else
            {
                selectLand.LandHighLightSide.ShowSelf(HighLightType.Single);
                selectLand.UseCard(battleCardUI);
                yield return new WaitForSeconds(PLACE_CARD_TIME);
                BattleCardManager.Instance.DestroyCardInAIState(battleCardUI,selectLand);
                toDeleteCardIndexList.Add(battleCardUI.arrayIndex);
            }
           
        }
        BattleCardManager.Instance.ResetCardInAIState(toDeleteCardIndexList);

    }

    /// <summary>
    /// 根据卡牌效果，返回一个应该放置的地块
    /// </summary>
    /// <param name="battleCardUI"></param>
    /// <returns></returns>
    private Land GetCardPlaceLandByType(BattleCardUI battleCardUI)
    {
        Land result = null;
        Camp curCamp = BattleManager.Instance.CurCamp;
        switch (battleCardUI.cardEffectType)
        {
            case CardEffectType.CampBuff:
                foreach (Land land in _orderedCurCampLands)
                {
                    if (land.LandCanUseCard(battleCardUI))
                    {
                        result = land;
                        break;
                    }
                }
                break;
            case CardEffectType.BombOtherCamp:
                foreach (Camp camp in BattleManager.Instance.CampDic.Values)
                {
                    if (camp != curCamp)
                    {
                        foreach (Land land in camp.ownedLands)
                        {
                            //遍历其他阵营地块，找出兵力最强的地
                            if (land.LandCanUseCard(battleCardUI) && (result == null || land.BattleUnit > result.BattleUnit) )
                            {
                                result = land;
                            }
                        }
                    }
                }        
                break;
            case CardEffectType.AttackOwnStrongLand:
                foreach (Land land in _orderedCurCampLands)
                {
                    if (land.LandCanUseCard(battleCardUI))
                    {
                        result = land;
                        break;
                    }
                }
                break;
            case CardEffectType.DefenceOwnWeakLand:
                for (int i = _orderedCurCampLands.Count - 1 ; i > 0 ; i--)
                {
                    if (_orderedCurCampLands[i].LandCanUseCard(battleCardUI))
                    {
                        result = _orderedCurCampLands[i];
                        break;
                    }
                }
                break;
            default:
                break;

        }
        return result;
    }
    IEnumerator LandCannonOperate()
    {
        List<Land> enemyLands;
       
        foreach (Land land in _orderedCurCampLands)
        {
            enemyLands = GetNeighborEnemyLandList(land);
            if (enemyLands.Count > 0)
            {
                //无炮买炮
                if (!land.cannon.isOwned)
                {
                    if (BattleManager.Instance.CurCamp.PurchaseCannon())
                    {
                        land.cannon.isOwned = true;
                        BattleManager.Instance.SetLandCannonTile(land);
                        yield return new WaitForSeconds(OPERATE_CANNON_TIME);
                        //买完直接轰炸
                        Land toBombLand = GetCannonBombLand(enemyLands);
                        //BombOtherLand(land, toBombLand);
                        if (toBombLand.BattleUnit >= 1 && BattleManager.Instance.CurCamp.CanCannonAttack())
                        {
                            toBombLand.LandHighLightSide.ShowSelf(HighLightType.Mutiple);
                            land.LandHighLightSide.ShowSelf(HighLightType.Mutiple);
                            yield return new WaitForSeconds(OPERATE_CANNON_TIME);
                            BattleManager.Instance.BombAnotherLand(land, toBombLand);
                            toBombLand.LandHighLightSide.ShowSelf(HighLightType.Mutiple, false);
                            land.LandHighLightSide.ShowSelf(HighLightType.Mutiple, false);
                        }
                    }                   
                }
                else if(!land.cannon.isInCool)//有炮轰炸
                {
                    Land toBombLand = GetCannonBombLand(enemyLands);
                   BombOtherLand(land, toBombLand);
                    if (toBombLand.BattleUnit >= 1 && BattleManager.Instance.CurCamp.CanCannonAttack())
                    {
                        toBombLand.LandHighLightSide.ShowSelf(HighLightType.Mutiple);
                        land.LandHighLightSide.ShowSelf(HighLightType.Mutiple);
                        yield return new WaitForSeconds(OPERATE_CANNON_TIME);
                        BattleManager.Instance.BombAnotherLand(land, toBombLand);
                        toBombLand.LandHighLightSide.ShowSelf(HighLightType.Mutiple, false);
                        land.LandHighLightSide.ShowSelf(HighLightType.Mutiple, false);
                    }
                }

            }
           
        }
        
    }

    /// <summary>
    /// 根据周围敌方格子，返回一个应该轰炸的地块
    /// </summary>
    /// <param name="enemyLands"></param>
    /// <returns></returns>
    private Land GetCannonBombLand(List<Land> enemyLands)
    {
        Land result = null;
        foreach (Land land in enemyLands)
        {
            if (result == null || result.BattleUnit < land.BattleUnit)
            {
                result = land;
            }
        }
        return result;
    }
    private void  BombOtherLand(Land attackLand,Land defenceLand)
    {
        //如果要轰炸的地块只有一个单位就不炸它了
        
       
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
