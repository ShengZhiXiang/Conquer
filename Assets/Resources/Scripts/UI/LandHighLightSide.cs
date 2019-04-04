using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LandHighLightSide : MonoBehaviour {


    public RectTransform rectTransform;

    public RectTransform canvasTransform;

    public void ShowSelf(bool show = true)
    {
        gameObject.SetActive(show);
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
        ShowSelf();
    }
}
