using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum HighLightType
{
    Single,
    Mutiple,
}

public class LandHighLightSide : MonoBehaviour {


    public RectTransform rectTransform;

    public RectTransform canvasTransform;

    public void ShowSelf(HighLightType highLightType, bool show = true)
    {
        if (GlobalUImanager.Instance.SingleLandHighLight == null)
        {
            GlobalUImanager.Instance.SingleLandHighLight = gameObject;
        }
       
        if (highLightType == HighLightType.Single)
        {
            //把上一个先隐藏掉
            GlobalUImanager.Instance.SingleLandHighLight.SetActive(false);
            GlobalUImanager.Instance.SingleLandHighLight = gameObject;
        }
        gameObject.SetActive(show);
        //适应屏幕宽高，使一个图片正好占地图一个单元格大小
        int rectHeight = Screen.height / ((int)Camera.main.orthographicSize * 2);
        rectTransform.sizeDelta = new Vector2(rectHeight, rectHeight);
        
    }

    public void SetPosition(Vector2 pos)
    {
        rectTransform.anchoredPosition = pos;
    }

    public void SetPosition(Vector3Int coordinate)
    {
        Vector2 UIpos = Vector2.zero;
        Vector2 screenPos = BattleManager.Instance.BattleMap.MapCoordinate2ScreenPos(coordinate);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
        canvasTransform as RectTransform, screenPos, null, out UIpos);
        SetPosition(UIpos);
       // ShowSelf();
    }
}
