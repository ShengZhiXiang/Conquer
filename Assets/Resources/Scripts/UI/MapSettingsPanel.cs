using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSettingsPanel : UINode {

    public Button startBtn;

    public CustomDropDown[] campSelectDropdowns;

    public Dropdown mapSizeDropDown;
    private int _mapHeight;
    //当前所有阵营的名字
    List<string> allCampNames = new List<string>();
    //已选择的阵营ID列表
    List<string> chosenCamps = new List<string>();
    //最终发给BattleManager的阵营参数列表
    List<InitalCampParam> campParams = new List<InitalCampParam>();
    public override void Initial()
    {
        base.Initial();
        startBtn.onClick.RemoveAllListeners();
        startBtn.onClick.AddListener(OnStartBtnClick);

        
        
        InitalCampDropDowns();
        
    }
    private void InitalCampDropDowns()
    {
        allCampNames.Add("无");
        foreach (CampModel campModel in GameDataSet.Instance.CampModelDic.Values)
        {
            allCampNames.Add(campModel.campName);
        }

        for (int i = 0; i < campSelectDropdowns.Length; i++)
        {
            campSelectDropdowns[i].ClearOptions();
            campSelectDropdowns[i].AddOptions(allCampNames);
            campSelectDropdowns[i].OnSelectOption = OnSelectDropDownOption;
            campSelectDropdowns[i].OnShowDropDownList = SetDropDownListOptionEnable;
        }
    }
    private void OnStartBtnClick()
    {
       GlobalUImanager.Instance.CloseUI(UIEnum.MapSettingsPanel);

        GetPanelData();
       
        BattleManager.Instance.GenerateMapInScene((MapHeight)_mapHeight, campParams);
    }

    private void GetPanelData()
    {
        Dictionary<string, int> campName_IDDic = GameDataSet.Instance.CampName_IDDic;
        if (campParams!=null)
        {
            campParams.Clear();
        }
        foreach (CustomDropDown dropdown in campSelectDropdowns)
        {
            string campName = dropdown.captionText.text;       
            if (!campName.Equals("无"))
            {
                int campID = campName_IDDic[campName];
                campParams.Add(new InitalCampParam(dropdown.PlayerName, campName, campID));
                Debug.Log(dropdown.PlayerName + "玩的是" + campName);
            }
        }

        switch (mapSizeDropDown.captionText.text)
        {
            case "小":
                _mapHeight = 4;
                break;
            case "中":
                _mapHeight = 6;
                break;
            case "大":
                _mapHeight = 8;
                break;
            default:
                break;

        }
    }

    private void OnSelectDropDownOption(string oldContent, string newContent)
    {  
        if (!oldContent.Equals("无") && !newContent.Equals("无"))
        {
            //把已选阵营列表中的旧阵营踢出去
            chosenCamps.Remove(oldContent);
            //把新阵营加入已选列表
            chosenCamps.Add(newContent);   
        }
        else if (!oldContent.Equals("无") && newContent.Equals("无"))
        {
            //把已选阵营列表中的旧阵营踢出去
            chosenCamps.Remove(oldContent);
        } else if (oldContent.Equals("无") &&! newContent.Equals("无"))
        {
            //新阵营加入已选列表
            chosenCamps.Add(newContent); 
        }
     
    }
    //DropdownItem
    private void SetDropDownListOptionEnable(GameObject root)
    {
        Toggle[] toggles = root.GetComponentsInChildren<Toggle>(false);
        for (int i = 0; i < toggles.Length; i++)
        {
            string campName = toggles[i].GetComponentInChildren<Text>().text;
            if (chosenCamps.Contains(campName))
            {
                toggles[i].interactable = false;
            }  

        }
    }
}
