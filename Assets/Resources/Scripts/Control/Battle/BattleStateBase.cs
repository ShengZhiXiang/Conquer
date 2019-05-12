using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BattleStateBase   {

    public virtual void RefreshUI() { }
    public virtual void RegisteMouseEvent(ref UnityAction mapEnterAction,ref UnityAction<Land, PointerEventData> mapClickAction,ref UnityAction mapExitAction) { }
    public virtual void OnUpdateFunc() { }
    public virtual void EnterState() { }

    /// <summary>
    /// 需要注册鼠标事件的EnterState方法
    /// </summary>
    /// <param name="mapEnterAction"></param>
    /// <param name="mapClickAction"></param>
    /// <param name="mapExitAction"></param>
    public  void EnterStateWithMouse(ref UnityAction mapEnterAction, ref UnityAction<Land, PointerEventData> mapClickAction, ref UnityAction mapExitAction)
    {
        RefreshUI();
        RegisteMouseEvent(ref mapEnterAction,ref mapClickAction,ref mapExitAction);
        EnterState();
    }  
    
    /// <summary>
    /// 不需要注册鼠标事件的EnterState方法
    /// </summary>
    public  void EnterStateWithoutMouse()
    {
        RefreshUI();
        EnterState();
    }

    /// <summary>
    /// 退出当前状态时把鼠标事件清空
    /// </summary>
    /// <param name="mapEnterAction"></param>
    /// <param name="mapClickAction"></param>
    /// <param name="mapExitAction"></param>
    public virtual void ExitState(ref UnityAction mapEnterAction, ref UnityAction<Land, PointerEventData> mapClickAction, ref UnityAction mapExitAction)
    {
        mapEnterAction = null;
        mapClickAction = null;
        mapExitAction = null;
    }
    #region 状态公用函数和变量

    public  List<Land> NeighborEnemyLands = new List<Land>();
    public List<Land> GetNeighborEnemyLandList(Land centerLand)
    {
        Vector3Int centerLandCoordinate = centerLand.CoordinateInMap;
        if (NeighborEnemyLands.Count != 0)
        {
            NeighborEnemyLands.Clear();
        }
        Vector3Int neighborCoordinate;
        //以0，0为中心块，检查其四周环绕的六个地块
        //正上方 （1，0）
        neighborCoordinate = new Vector3Int(centerLandCoordinate.x + 1, centerLandCoordinate.y, 0);
        CheckNeighborLandEnemy(centerLand,neighborCoordinate);
        //右上方 （0，1）
        neighborCoordinate = centerLandCoordinate.y % 2 == 0 ? new Vector3Int(centerLandCoordinate.x, centerLandCoordinate.y + 1, 0)
                                                   : new Vector3Int(centerLandCoordinate.x + 1, centerLandCoordinate.y + 1, 0);
        CheckNeighborLandEnemy(centerLand,neighborCoordinate);
        //右下方 （-1，1）
        neighborCoordinate = centerLandCoordinate.y % 2 == 0 ? new Vector3Int(centerLandCoordinate.x - 1, centerLandCoordinate.y + 1, 0)
                                                   : new Vector3Int(centerLandCoordinate.x, centerLandCoordinate.y + 1, 0);
        CheckNeighborLandEnemy(centerLand,neighborCoordinate);
        //正下方 （-1，0）
        neighborCoordinate = new Vector3Int(centerLandCoordinate.x - 1, centerLandCoordinate.y, 0);
        CheckNeighborLandEnemy(centerLand,neighborCoordinate);
        //左下方 （-1，-1）
        neighborCoordinate = centerLandCoordinate.y % 2 == 0 ? new Vector3Int(centerLandCoordinate.x - 1, centerLandCoordinate.y - 1, 0)
                                                   : new Vector3Int(centerLandCoordinate.x, centerLandCoordinate.y - 1, 0);
        CheckNeighborLandEnemy(centerLand,neighborCoordinate);
        //左上方 （0，-1）
        neighborCoordinate = centerLandCoordinate.y % 2 == 0 ? new Vector3Int(centerLandCoordinate.x, centerLandCoordinate.y - 1, 0)
                                                   : new Vector3Int(centerLandCoordinate.x + 1, centerLandCoordinate.y - 1, 0);
        CheckNeighborLandEnemy(centerLand,neighborCoordinate);

        return NeighborEnemyLands;
    }
    private void CheckNeighborLandEnemy(Land centerLand,Vector3Int neighborCoordinate)
    {
        if (BattleManager.Instance.isCoordinateInMap(neighborCoordinate))
        {
            Land land = BattleManager.Instance.BattleMap.Coordinate2Land(neighborCoordinate);
            if (land.CampID != centerLand.CampID)
            {
                NeighborEnemyLands.Add(land);
            }
        }
    }
    public void HighLightNeighborEnemyLands(Land  centerLand)
    {
        GetNeighborEnemyLandList(centerLand);
        foreach (Land land in NeighborEnemyLands)
        {
            land.LandHighLightSide.ShowSelf(HighLightType.Mutiple);
        }
    }
  
    #endregion



}
