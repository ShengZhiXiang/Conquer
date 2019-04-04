using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Test : MonoBehaviour {

  
	// Use this for initialization
	void Start () {

        //只是为了测试一局游戏开局，以后会走正常逻辑
        GlobalUImanager.Instance.OpenUI(UIEnum.MapSettingsPanel);
        GameDataSet.Instance.Empty();
        //Click3D.click3DEvent.AddListener(delegate (GameObject gameObject, PointerEventData eventData)
        //{
        //    Debug.Log("进入图片" + gameObject.name);
        //});
    }

   


}
