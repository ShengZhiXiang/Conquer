using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleMainPanel : UINode {
    [SerializeField]
    public Button _btnReMakeMap;
    [SerializeField]
    public Text _text;
    [SerializeField]
    public Button _btnEndTurn;
    [SerializeField]
    public GameObject _curLandInfo;
    [SerializeField]
    public GameObject _cardList;
    
    public override void Initial()
    {
        base.Initial();

        _btnReMakeMap.onClick.AddListener(delegate ()
             {
                 StartCoroutine(BattleManager.Instance.ReGenerateMap());    
             }
         );
        _btnEndTurn.onClick.AddListener(delegate() 
             {
                 StartCoroutine(BattleManager.Instance.BattleTurnEnd());
            }
        );

        
        _btnEndTurn.gameObject.SetActive(false);
        BattleManager.Instance.BATTLE_EVENT_FinishAttackOneLand += UpdateCurCampInfo;
       // BattleManager.Instance.BATTLE_EVENT_BattleStart += UpdateCurCampInfo;
        BattleManager.Instance.BATTLE_EVENT_BattleStart += OnBattleStartPanelChange;
       
        BattleManager.Instance.BATTLE_EVENT_ReSelectMap += UpdateCurCampInfo;
        BattleManager.Instance.BATTLE_EVENT_MyTurnStart += ShowCurLandInfoTip;
        BattleManager.Instance.BATTLE_EVENT_MyTurnStart += UpdateCurCampInfo;
        BattleManager.Instance.BATTLE_EVENT_AITURNStart += HideCurLandInfoTip;
        BattleManager.Instance.BATTLE_EVENT_AITURNStart += UpdateCurCampInfo;
        BattleManager.Instance.BATTLE_EVENT_BOMB_ANOTHER_LAND += UpdateCurCampInfo;
        BattleManager.Instance.BATTLE_EVENT_PURCHASE_CANNON += UpdateCurCampInfo;
        BattleManager.Instance.BATTLE_EVENT_USE_CARD += UpdateCurCampInfo;
        UpdateCurCampInfo();

       

    }

   
    public void SetText(string text)
    {
        _text.text = text;
    }

    string curCampInfo;

    /// <summary>
    /// 刷新右上角当前阵营信息
    /// </summary>
    private  void UpdateCurCampInfo()
    {
        Camp curCamp = BattleManager.Instance.CurCamp;
        int allBattleUnit = 0;
        int ownedGold = curCamp.OwnGold;
        int curNumOfRound = BattleManager.Instance.CurNumOfRounds;
        int leftPopulation = 0;
        foreach (Land land in curCamp.ownedLands)
        {
            allBattleUnit += land.BattleUnit;
            leftPopulation += land.leftPopulation;
        }
        curCampInfo = string.Format("*****当前回合:{0}*****\n" +
                                    "当前阵营:{1},总地块数:{2},总兵力:{3}\n" +
                                    "****持有总金币:{4},剩余总人口:{5}****",
                                    curNumOfRound,curCamp.name, curCamp.ownedLands.Count, allBattleUnit, ownedGold, leftPopulation);
        SetText(curCampInfo);
    }

    private void OnBattleStartPanelChange()
    {
        _btnEndTurn.gameObject.SetActive(true);
        _btnReMakeMap.gameObject.SetActive(false);
        _curLandInfo.SetActive(true);
    }
    private void ShowCurLandInfoTip()
    {
        _curLandInfo.SetActive(true);
        _cardList.SetActive(true);
    }

    private void HideCurLandInfoTip()
    {
        _curLandInfo.SetActive(false);
        //_cardList.SetActive(false);
    }

    public void RefreshCurLandInfo(string content)
    {
        _curLandInfo.GetComponentInChildren<Text>().text = content;
    }


}
