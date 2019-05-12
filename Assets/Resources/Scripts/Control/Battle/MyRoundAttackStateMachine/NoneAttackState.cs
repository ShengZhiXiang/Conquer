using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoneAttackState : AttackStateBase
{
    public NoneAttackState(MyRoundState myRound):base(myRound)
    {

    }
#region 重写方法
    public override void ClickAction(Land clickLand)
    {
        base.ClickAction(clickLand);
        if (!BattleManager.Instance.CanAttack)
        {
            return;
        }
        //空闲状态时只能操作自己阵营的地块
        if (clickLand.CampID == BattleManager.Instance.CurCamp.campID && !LandOperateMenu.isActiveAndEnabled)
        {
            Debug.Log("点击了自己的地块");
            //当前状态的进攻地块就是点击的地块
            attackLand = clickLand;
            //打开右键菜单
            SetLandOperateMenu();
        }
        else
        {
            Debug.Log("空闲状态下的取消操作");
            MyRound.Cancel();
        }     
    }
    private Vector3 mousePosition;
    private Land MouseHoverLand;
    public override void UpdateFunc()
    {
        base.UpdateFunc();
        if (BattleManager.Instance.CanAttack && !LandOperateMenu.isActiveAndEnabled)
        {
            mousePosition = Input.mousePosition;
            MouseHoverLand = BattleManager.Instance.BattleMap.GetCurMouseLand(mousePosition);
            //如果指向的是己方阵营，并且当前地块可选择，显示高亮框
            if (CurMouseLandCanSelect(MouseHoverLand))
            {
                MouseHoverLand.LandHighLightSide.ShowSelf(HighLightType.Single);
                //GlobalUImanager.Instance.SingleLandHighLight.GetComponent<LandHighLightSide>().SetPosition(MouseHoverLand.CoordinateInMap);
            }
            else
            {
               // GlobalUImanager.Instance.SingleLandHighLight.GetComponent<LandHighLightSide>().ShowSelf(false);
                MouseHoverLand.LandHighLightSide.ShowSelf(HighLightType.Single,false);
            }
        }
        else
        {
            GlobalUImanager.Instance.SingleLandHighLight.GetComponent<LandHighLightSide>().ShowSelf(HighLightType.Single,false);
        }
    }
    public override void OnClickCard()
    {
        base.OnClickCard();
        HideLandOpearteMenu();
        MyRound.CurAttackState = MyRound.AttackStateDic[AttackStateEnum.CardAttack];

    }
    #endregion
    #region 自身函数
    /// <summary>
    /// 弹出并设置地块操作菜单
    /// </summary>
    private void SetLandOperateMenu()
    {
        //先清空按钮点击事件
        LandOperateMenu.attackBtn.onClick.RemoveAllListeners();
        LandOperateMenu.bombBtn.onClick.RemoveAllListeners();
        //弹出菜单选项
        LandOperateMenu.ShowSelfOnLandPosition(attackLand.CoordinateInMap);
        LandOperateMenu.attackBtn.onClick.AddListener(
            delegate ()
            {
                if (CurMouseLandCanAttack(attackLand))
                {                    
                    //进入军队进攻状态
                    MyRound.CurAttackState = MyRound.AttackStateDic[AttackStateEnum.ArmyAttack];
                    MyRound.CurAttackState.attackLand = attackLand;                 
                    //周围敌方格子有高亮框
                    MyRound.HighLightNeighborEnemyLands(attackLand);                    
                }
                else
                {
                    //提示不能进攻
                    GlobalUImanager.Instance.OpenPopTip().GetComponent<PopTip>().SetContent("军队无法前进！");
                }
                HideLandOpearteMenu();
            });
        //如果该地没有大炮，则该操作为买炮
        if (!attackLand.cannon.isOwned)
        {
           LandOperateMenu.SetBombTextContent("购买高炮");
           LandOperateMenu.bombBtn.onClick.AddListener(
              delegate ()
             {
                 if (BattleManager.Instance.CurCamp.PurchaseCannon())
                 {
                     attackLand.cannon.isOwned = true;
                     //设置贴图为有炮的
                     //todo
                     BattleManager.Instance.SetLandCannonTile(attackLand);
                     if (BattleManager.Instance.BATTLE_EVENT_PURCHASE_CANNON!=null)
                     {
                         BattleManager.Instance.BATTLE_EVENT_PURCHASE_CANNON();
                     }
                 }
                 else
                 {
                     GlobalUImanager.Instance.OpenPopTip().GetComponent<PopTip>().SetContent("你没钱奥！");
                 }          
               HideLandOpearteMenu();
              });
        }
        else//否则是轰炸其他地块
        {
            LandOperateMenu.SetBombTextContent("轰炸");
            LandOperateMenu.bombBtn.onClick.AddListener(
            delegate ()
            {
                //如果没冷却就轰炸
                if (!attackLand.cannon.isInCool)
                {
                    //进入轰炸进攻状态
                    MyRound.CurAttackState = MyRound.AttackStateDic[AttackStateEnum.BombAttack];
                    MyRound.CurAttackState.attackLand = attackLand;
                    //周围敌方格子有高亮框
                    MyRound.HighLightNeighborEnemyLands(attackLand);                   
                }
                else//否则提示处于冷却中
                {
                     GlobalUImanager.Instance.OpenPopTip().GetComponent<PopTip>().SetContent("高炮处于冷却中！");
                }
                HideLandOpearteMenu();
            });
        }          
    }
    #endregion
}
