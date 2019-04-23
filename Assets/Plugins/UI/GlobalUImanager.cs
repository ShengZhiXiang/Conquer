﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum UIEnum
{
    MapSettingsPanel,
    BattleMainPanel

}

public enum MsgBoxEnum
{
    MsgBox2Btns
}


public class GlobalUImanager : Singleton<GlobalUImanager> {

    private  Dictionary<UIEnum, GameObject> _openedUIs;
    public  Dictionary<UIEnum,GameObject> OpenedUIs
    {
        get
        {
            if (_openedUIs==null)
            {
                _openedUIs = new Dictionary<UIEnum, GameObject>();
            }
            return _openedUIs;
        }
    }

    private Dictionary<MsgBoxEnum, GameObject> _openedMsgBoxs;
    public Dictionary<MsgBoxEnum, GameObject> OpenedMsgBoxs
    {
        get
        {
            if (_openedMsgBoxs == null)
            {
                _openedMsgBoxs = new Dictionary<MsgBoxEnum, GameObject>();
            }
            return _openedMsgBoxs;
        }
    }
    #region 战斗相关界面和方法
    private GameObject _singleLandHighLight;
    public GameObject SingleLandHighLight
    {
        get
        {
            if (_singleLandHighLight == null)
            {
                GameObject temp = Resources.Load<GameObject>("Prefabs/UI/LandHighLightSide");
                _singleLandHighLight = Instantiate(temp, UIRoot);
            }
            return _singleLandHighLight;
        }

    }

    private List<GameObject> _multipleLandHighLight;
    public List<GameObject> MultipleLandHighLight
    {
        get
        {
            if (_multipleLandHighLight == null)
            {
                _multipleLandHighLight = new List<GameObject>();
                for (int i = 0;i<6;i++)
                {
                    GameObject temp = Resources.Load<GameObject>("Prefabs/UI/LandHighLightSide");
                    GameObject landHighlight = Instantiate(temp, UIRoot);
                    landHighlight.SetActive(false);
                    _multipleLandHighLight.Add(landHighlight) ;

                }
                
            }
            return _multipleLandHighLight;
        }

    }
    private GameObject _landOperateMenu;
    public GameObject LandOperateMenu
    {
        get
        {
            if (_landOperateMenu == null)
            {
                GameObject temp = Resources.Load<GameObject>("Prefabs/UI/LandOperateMenu");
                _landOperateMenu = Instantiate(temp, Instance.UIRoot);
                _landOperateMenu.gameObject.SetActive(false);
            }
            return _landOperateMenu;
        }
    }
    /// <summary>
    /// 补兵的动画
    /// </summary>
    private List<GameObject> _baseUnitSupplyTips;
    public List<GameObject> BaseUnitSupplyTips
    {
        get
        {
            if (_baseUnitSupplyTips == null)
            {
                _baseUnitSupplyTips = new List<GameObject>();               
                for (int i = 0; i < 7 ; i++)
                {                  
                    GameObject temp = Resources.Load<GameObject>("Prefabs/UI/BattleBaseUnitSupplyTip");
                    GameObject baseUnitSupplyTip = Instantiate(temp, UIRoot);
                    _baseUnitSupplyTips.Add(baseUnitSupplyTip);
                }               
            }
            return _baseUnitSupplyTips;
        }
    }
    public void SetBaseUnitSupplyTips(int count)
    {
        _baseUnitSupplyTips = new List<GameObject>();
        for (int i = 0; i < count; i++)
        {
            GameObject temp = Resources.Load<GameObject>("Prefabs/UI/BattleBaseUnitSupplyTip");
            GameObject baseUnitSupplyTip = Instantiate(temp, UIRoot);
            baseUnitSupplyTip.SetActive(false);
            _baseUnitSupplyTips.Add(baseUnitSupplyTip);
        }
    }
    private GameObject _landTip;
    public GameObject LandTip
    {
        get
        {
            if (_landTip == null)
            {
                GameObject temp = Resources.Load<GameObject>("Prefabs/UI/LandTip");
                _landTip = Instantiate(temp, UIRoot);
            }
            return _landTip;
        }
    }

    private GameObject _battleReloadMapUI;
    public GameObject BattleReloadMap
    {
        get
        {
            if (_battleReloadMapUI == null)
            {
                GameObject temp = Resources.Load<GameObject>("Prefabs/UI/BattleReloadMapUI");
                _battleReloadMapUI = Instantiate(temp,UIRoot);
            }
            return _battleReloadMapUI;
        }
    }

    public void ShowTip(bool isShow = true)
    {
        LandTip.SetActive(isShow);
    }

    public GameObject _battleDicePanel;
    public GameObject BattleDicePanel
    {
        get
        {
            if (_battleDicePanel == null)
            {
                GameObject temp = Resources.Load<GameObject>("Prefabs/UI/BattleDicePanel");
                _battleDicePanel = Instantiate(temp, UIRoot);
            }
            return _battleDicePanel;
        }
    }

    #endregion
    private Transform _uiRoot;
    public Transform UIRoot
    {
        get
        {
            if (_uiRoot == null)
            {
                _uiRoot = GameObject.Find("UIRoot").transform;
            }
            return _uiRoot;
        }
    }



    public GameObject OpenUI(UIEnum ui)
    {
        GameObject uiNode;
        //如果当前已打开过的UI列表有，则重新打开
        if (OpenedUIs.TryGetValue(ui, out uiNode))
        {           
            uiNode.gameObject.SetActive(true);
            return uiNode;
        }
        //否则从Prefebs中加载
        else
        {
            string path = string.Format("Prefabs/UI/{0}",ui.ToString());          
            GameObject UI = Resources.Load(path) as GameObject;
            uiNode = Instantiate(UI, UIRoot);
           // uiNode.SetActive(false);
        
            OpenedUIs.Add(ui, uiNode);
            return uiNode;
        }
    }

    /// <summary>
    /// 关闭，但不销毁一个OpenedUIs中的UI
    /// </summary>
    /// <param name="ui"></param>
    public void CloseUI(UIEnum ui)
    {
        
        if (OpenedUIs.ContainsKey(ui))
        {         
             OpenedUIs[ui].SetActive(false);          
           
        }
    }

    public GameObject OpenMsgBox(MsgBoxEnum msgBoxEnum)
    {
      
        GameObject msgBoxObj;
        if (OpenedMsgBoxs.TryGetValue(msgBoxEnum, out msgBoxObj))
        {
            msgBoxObj.gameObject.SetActive(true);
        }
        else
        {
            string path = string.Format("Prefabs/UI/{0}", msgBoxEnum.ToString());
            GameObject MsgBox = Resources.Load(path) as GameObject;
            msgBoxObj = Instantiate(MsgBox, UIRoot);
            OpenedMsgBoxs.Add(msgBoxEnum,msgBoxObj);
        }

        return msgBoxObj;
       
    }


    public void CloseMsgBox(GameObject toCloseMsgBox)
    {
        if (toCloseMsgBox)
        {
            toCloseMsgBox.SetActive(false);
        }      
    }

   


    public GameObject OpenPopTip()
    {
        GameObject temp = Resources.Load<GameObject>("Prefabs/UI/PopTip");
        return Instantiate(temp, UIRoot);
    }
}
