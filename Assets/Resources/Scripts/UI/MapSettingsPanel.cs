using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSettingsPanel : UINode {

    public Button stratBtn;
    public Toggle[] sizeGroup;
    public Toggle[] campGroup;

    private int _mapHeight;
    private int _campCount;

    public override void Initial()
    {
        //添加事件监听
        AddListener2Toggles(sizeGroup, true);
        AddListener2Toggles(campGroup, false);

        stratBtn.onClick.AddListener(OnStartBtnClick);
      
        base.Initial();
    }

    private void OnStartBtnClick()
    {        
        BattleManager.Instance.GenerateMapInScene(_mapHeight, _campCount);
        GlobalUImanager.Instance.CloseUI(UIEnum.MapSettingsPanel);
    }

    public void AddListener2Toggles(Toggle[] toggles, bool isMapSize)
    {
        //先把当前选中的值赋值给数据
        foreach (Toggle toggle in toggles)
        {
            if (toggle.isOn)
            {
                if (isMapSize)
                {
                    _mapHeight = int.Parse(toggle.name);                   
                }
                else
                {                   
                    _campCount = int.Parse(toggle.name);
                }
            }
        }
        //添加点选监听事件
        foreach (Toggle toggle in toggles)
        {
            toggle.onValueChanged.AddListener(delegate(bool isOn) 
            {
                if (isOn)
                {
                    if (isMapSize)
                    {
                        Debug.Log(toggle.name);
                        _mapHeight = int.Parse(toggle.name);
                        Debug.Log("地图大小" + _mapHeight);
                    }
                    else
                    {
                        Debug.Log(toggle.name);
                        _campCount = int.Parse(toggle.name);
                        Debug.Log("阵营数量" + _campCount);
                    }
                }
            });
        }
    }

}
