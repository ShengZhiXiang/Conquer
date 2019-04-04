using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SelectCapitalState : BattleStateBase {

    

    public bool _showTip;
    public LandTip tip;
    private bool _lockUpdateMouse;
    public override void RegisteMouseEvent(ref UnityAction mapEnterAction,ref UnityAction<Land, PointerEventData> mapClickAction, ref UnityAction mapExitAction)
    {
        base.RegisteMouseEvent(ref mapEnterAction, ref mapClickAction, ref mapExitAction);
        mapEnterAction = delegate ()
        {
            tip = GlobalUImanager.Instance.LandTip.GetComponent<LandTip>();
            _showTip = true;
        };


        mapClickAction = delegate (Land clickLand, PointerEventData eventData)
        {
            Camp myCamp = BattleManager.Instance.MyCamp;
            if (eventData.button == PointerEventData.InputButton.Left && clickLand.CampID == myCamp.campID)
            {
                _lockUpdateMouse = true;
                tip.ShowSelf(false);
                GameObject MsgBoxObj = GlobalUImanager.Instance.OpenMsgBox(MsgBoxEnum.MsgBox2Btns);
                MsgBoxObj.GetComponent<MsgBox2Btns>().SetMsgBox("确定选择该地为首都吗？",
                    () =>
                    {
                        _lockUpdateMouse = false;
                        myCamp.capitalLand = clickLand;
                        BattleManager.Instance.BattleMap.SetCapital(clickLand);
                        GlobalUImanager.Instance.CloseMsgBox(MsgBoxObj);

                        //发出战斗开始的事件
                        if (BattleManager.Instance.BattleStart!=null)
                        {
                            BattleManager.Instance.BattleStart();
                        }
                        //进入我的回合状态
                        BattleMap battleMap = BattleManager.Instance.BattleMap;
                        BattleManager.Instance.CurBattleState = BattleManager.Instance.BattleStateDictionary[BattleStateEnum.MyRound];
                        BattleManager.Instance.CurBattleState.EnterStateWithMouse(ref battleMap.mapEnterAction,ref battleMap.mapClickAction,ref battleMap.mapExitAction);
                    },
                    () =>
                    {
                        GlobalUImanager.Instance.CloseMsgBox(MsgBoxObj);
                        _lockUpdateMouse = false;
                    });
            }
        };

        mapExitAction = delegate ()
        {
            _showTip = false;
            tip = null;
        };
    }

    private Vector2 UIpos = Vector2.one;
    Vector3 mousePosition;
    Vector3Int mapCoordinate;
    public override void OnUpdateFunc()
    {
        base.OnUpdateFunc();

        GlobalUImanager.Instance.ShowTip(_showTip && !_lockUpdateMouse);
        //显示每个地块的信息
        if (_showTip&& !_lockUpdateMouse)
        {           
            mousePosition = Input.mousePosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                tip.canvasTransform as RectTransform,
                mousePosition, null, out UIpos))
            {
                mapCoordinate = BattleManager.Instance.BattleMap.MousePos2MapCoordinate(mousePosition);
                tip.SetPosition(UIpos, mapCoordinate);            
                CustomTerrain curTerrain = BattleManager.Instance.BattleMap.GetCurMouseLand(mousePosition).CurTerrian;
                tip.SetText(string.Format("当前地形：{0}\n人口：{1}，金币：{2}，粮食：{3}",
                                curTerrain.name, curTerrain.population, curTerrain.gold, curTerrain.food));
            }
        }

    }

    public override void RefreshUI()
    {
        base.RefreshUI();
        GlobalUImanager.Instance.OpenPopTip().GetComponent<PopTip>().SetContent("请选择首都");
        
        GlobalUImanager.Instance.OpenUI(UIEnum.BattleMainPanel);
        
    }


    public override void ExitState(ref UnityAction mapEnterAction, ref UnityAction<Land, PointerEventData> mapClickAction, ref UnityAction mapExitAction)
    {
        base.ExitState(ref mapEnterAction,ref mapClickAction,ref mapExitAction);      
    }
}
