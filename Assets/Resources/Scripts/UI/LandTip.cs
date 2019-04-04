using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LandTip : MonoBehaviour {

    public Text text;
    
    public RectTransform rectTransform;

    public RectTransform canvasTransform;

    public void SetText(string content)
    {
        text.text = content;
    }

    public void ShowSelf(bool show = true)
    {
        gameObject.SetActive(show);
    }

    public void SetPosition(Vector2 pos, Vector3Int mapCoordinat)
    {
        
       
        float pivotX;
        float pivotY;

        pivotY = mapCoordinat.x >= 0 ? -0.25f : 1.25f;
        pivotX = mapCoordinat.y >= 0 ? -0.04f : 1.04f;

        rectTransform.pivot = new Vector2(pivotX, pivotY);

        rectTransform.anchoredPosition = pos;
    }
}
