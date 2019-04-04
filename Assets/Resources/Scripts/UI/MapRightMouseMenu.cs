using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapRightMouseMenu : MonoBehaviour {

    public Button attackBtn;
    public Button bombBtn;
    public Button friendBtn;

    public RectTransform canvasTransform;

    public RectTransform rectTransform;
    private void Start()
    {
        //attackBtn.onClick.AddListener(OnAttackBtnClicked);
    }

    private void OnAttackBtnClicked(Land land)
    {

    }
    public void ShowSelf(bool show = true)
    {
        gameObject.SetActive(show);
    }

    public void SetPosition(Vector2 pos, Vector3Int mapCoordinate)
    {
        float pivotX;
        float pivotY;

        pivotY = mapCoordinate.x >= 0 ? 1.1f : -0.12f;
        pivotX = mapCoordinate.y >= 0 ? -0.2f : 1.12f;

        rectTransform.pivot = new Vector2(pivotX, pivotY);

        rectTransform.anchoredPosition = pos;
    }

    /// <summary>
    /// 在该Land位置上显示自身
    /// </summary>
    /// <param name="land"></param>
    public void ShowSelfOnLandPosition(Vector3Int coordinate)
    {
        Vector3 screenPos = BattleManager.Instance.BattleMap.MapCoordinate2ScreenPos(coordinate);
        Vector2 UIpos ;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasTransform as RectTransform, screenPos, null, out UIpos);
        SetPosition(UIpos, coordinate);
        ShowSelf();
    }
}
