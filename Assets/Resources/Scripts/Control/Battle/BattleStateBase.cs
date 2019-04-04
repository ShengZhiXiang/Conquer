using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BattleStateBase  {

    public virtual void RefreshUI() { }
    public virtual void RegisteMouseEvent(ref UnityAction mapEnterAction,ref UnityAction<Land, PointerEventData> mapClickAction,ref UnityAction mapExitAction) { }
    public virtual void OnUpdateFunc() { }

    /// <summary>
    /// 需要注册鼠标事件的EnterState方法
    /// </summary>
    /// <param name="mapEnterAction"></param>
    /// <param name="mapClickAction"></param>
    /// <param name="mapExitAction"></param>
    public  void EnterStateWithMouse(ref UnityAction mapEnterAction, ref UnityAction<Land, PointerEventData> mapClickAction, ref UnityAction mapExitAction)
    {
        RefreshUI();
        RegisteMouseEvent(ref mapEnterAction,ref mapClickAction,ref mapExitAction);
    }  
    
    /// <summary>
    /// 不需要注册鼠标事件的EnterState方法
    /// </summary>
    public  void EnterState()
    {
        RefreshUI();
    }

    /// <summary>
    /// 退出当前状态时把鼠标事件清空
    /// </summary>
    /// <param name="mapEnterAction"></param>
    /// <param name="mapClickAction"></param>
    /// <param name="mapExitAction"></param>
    public virtual void ExitState(ref UnityAction mapEnterAction, ref UnityAction<Land, PointerEventData> mapClickAction, ref UnityAction mapExitAction)
    {
        mapEnterAction = null;
        mapClickAction = null;
        mapExitAction = null;
    }

   
    

}
