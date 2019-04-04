using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private GameObject _loadingUI;
    public GameObject LoadingUI
    {
        get
        {
            if (_loadingUI==null)
            {
                GameObject temp = Resources.Load<GameObject>("Prefabs/UI/LoadingUI");
                _loadingUI = Instantiate(temp,UIRoot);
            }
            return _loadingUI;
        }
    }

    public void ShowTip(bool isShow = true)
    {
        LandTip.SetActive(isShow);

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
