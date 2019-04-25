using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleBaseUnitSupplyTip : UINode {
    public RectTransform imageRect;
    public Image image;
    public RectTransform canvasTransform;
    public RectTransform rectTransform;
    public Animator animator;
    public void ShowSelf()
    {
        this.gameObject.SetActive(true);
        int rectHeight =(int)(0.7f * (Screen.height / ((int)Camera.main.orthographicSize * 2)));
        imageRect.sizeDelta = new Vector2(rectHeight, rectHeight);
        
    }
    public void SetImage(Sprite sprite)
    {
        image.sprite = sprite;
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
    public void SetPosition(Vector2 pos)
    {
        rectTransform.anchoredPosition = pos;
    }

    public float GetAnimationTime()
    {
        float time = animator.GetCurrentAnimatorStateInfo(0).length;
        return time;
    }

}
