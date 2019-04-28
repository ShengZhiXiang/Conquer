using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Test : MonoBehaviour {

  
	// Use this for initialization
	void Start () {
        GameDataSet.Instance.Empty();
        //只是为了测试一局游戏开局，以后会走正常逻辑
        GlobalUImanager.Instance.OpenUI(UIEnum.MapSettingsPanel);
       
        BattleCardManager.Instance.Inital();
       
    }

   


}
