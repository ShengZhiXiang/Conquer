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
    public InputField _inputField;
    
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
                 BattleManager.Instance.MyTurnEnd();
            }
        );

        
        _btnEndTurn.gameObject.SetActive(false);
        BattleManager.Instance.BattleFinishAttackOneLand += UpdateCurCampInfo;
        BattleManager.Instance.BattleStart += UpdateCurCampInfo;
        BattleManager.Instance.BattleStart += OnBattleStartPanelChange;
        BattleManager.Instance.BattleEndMyTurn += UpdateCurCampInfo;
        BattleManager.Instance.BattleReSelectMap += UpdateCurCampInfo;
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
        int gold = 0;
        int population = 0;
        int food = 0;
        foreach (Land land in curCamp.ownedLands)
        {
            Debug.Log(string.Format("{0}坐标中战斗单位有{1}个",land.CoordinateInMap,land.battleUnit));
            allBattleUnit += land.battleUnit;
            gold += land.CurTerrian.gold;
            population += land.CurTerrian.population;
            food += land.CurTerrian.food;
        }
        curCampInfo = string.Format("当前阵营:{0},总地块数:{1},总兵力:{2}\n金币:{3},人口:{4},粮食:{5}"
                                    , curCamp.name, curCamp.ownedLands.Count, allBattleUnit, gold, population, food);
        SetText(curCampInfo);
    }

    private void OnBattleStartPanelChange()
    {
        _btnEndTurn.gameObject.SetActive(true);
        _btnReMakeMap.gameObject.SetActive(false);
    }


}
