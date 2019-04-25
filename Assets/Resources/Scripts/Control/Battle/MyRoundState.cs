using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class MyRoundState : BattleStateBase {

    private AttackStateBase _curAttackState;
    public AttackStateBase CurAttackState
    {
        get   { return _curAttackState;}
        set   { _curAttackState = value;}
    }
    public Dictionary<AttackStateEnum, AttackStateBase> AttackStateDic;
    public override void EnterState()
    {
        base.EnterState();
        if (AttackStateDic==null)
        {
            AttackStateDic = new Dictionary<AttackStateEnum, AttackStateBase>();
            AttackStateDic.Add(AttackStateEnum.NoneAttack, new NoneAttackState(this));
            AttackStateDic.Add(AttackStateEnum.ArmyAttack, new ArmyAttackState(this));
            AttackStateDic.Add(AttackStateEnum.BombAttack, new BombAttackState(this));
            AttackStateDic.Add(AttackStateEnum.CardAttack,new CardAttackState(this));
        }
        
        CurAttackState = AttackStateDic[AttackStateEnum.NoneAttack];


        if (BattleManager.Instance.BATTLE_EVENT_MyTurnStart!=null)
        {
            BattleManager.Instance.BATTLE_EVENT_MyTurnStart();
        }
    }

    #region 鼠标注册方法重写
    private bool _enterMap;
    private Land MouseClickBtnLand;
    public override void RegisteMouseEvent(ref UnityAction mapEnterAction, ref UnityAction<Land, PointerEventData> mapClickAction, ref UnityAction mapExitAction)
    {
        base.RegisteMouseEvent(ref mapEnterAction, ref mapClickAction, ref mapExitAction);
        mapEnterAction = delegate () 
        {
            _enterMap = true;          
        };
        mapClickAction = delegate (Land land, PointerEventData eventData)
        {           
            MouseClickBtnLand = land;
            //如果点击的是左键，则进行游戏操作
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                CurAttackState.ClickAction(MouseClickBtnLand);
                SetCurSelectLandInfo(MouseClickBtnLand);
            }
            else
            {
                Cancel();
                CurAttackState = AttackStateDic[AttackStateEnum.NoneAttack];               
            }
        };

        mapExitAction = delegate ()
        {
            _enterMap = false;
        };
    }
    #endregion


    #region Update方法

    public override void OnUpdateFunc()
    {
        base.OnUpdateFunc();
        if (_enterMap)
        {
            CurAttackState.UpdateFunc();           
        }
        else
        {
            if (GlobalUImanager.Instance.SingleLandHighLight != null)
            {
                GlobalUImanager.Instance.SingleLandHighLight.GetComponent<LandHighLightSide>().ShowSelf(HighLightType.Single,false);
            }   
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            BattleManager.Instance.BattleMap.SetCampLayerVisable(false);
        }
        else if(Input.GetKeyUp(KeyCode.Space))
        {
            BattleManager.Instance.BattleMap.SetCampLayerVisable(true);
        }
    }
    #endregion
    public override void RefreshUI()
    {
        base.RefreshUI();
    }

    public override void ExitState(ref UnityAction mapEnterAction, ref UnityAction<Land, PointerEventData> mapClickAction, ref UnityAction mapExitAction)
    {
        base.ExitState(ref mapEnterAction, ref mapClickAction, ref mapExitAction);
        Cancel();
    }

    private  void Cancel()
    {
        CurAttackState.HideLandOpearteMenu();
        CurAttackState.CancelHighlight();
        BattleCardManager.Instance.CancelSelectCard();
    }

    private void SetCurSelectLandInfo(Land land)
    {

        string CampName = BattleManager.Instance.CampDic[land.CampID].name;
        string battleUnit = land.BattleUnit.ToString();
        string terrianName = land.CurTerrian.name;
        string hasCannon = land.cannon.isOwned ? "是" : "否";
        string leftPopulation = land.leftPopulation.ToString();
        string content = string.Format("**当前地块：**\n*****************\n阵营 ：{0}\n作战单位 ：{1}\n地形 ：{2}\n有炮否？ {3}\n剩余人口{4}\n*****************",
                                   CampName, battleUnit, terrianName, hasCannon, leftPopulation);
        GlobalUImanager.Instance.OpenUI(UIEnum.BattleMainPanel).GetComponent<BattleMainPanel>().RefreshCurLandInfo(content);
    }
}
