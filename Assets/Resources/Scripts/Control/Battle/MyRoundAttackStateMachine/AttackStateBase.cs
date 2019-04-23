﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AttackStateEnum
{
    NoneAttack,
    ArmyAttack,
    BombAttack,
    CardAttack,
}

public class AttackStateBase  {

    public LandOperateMenu LandOperateMenu = GlobalUImanager.Instance.LandOperateMenu.GetComponent<LandOperateMenu>();
    
    private List<Vector3Int> NeighborEnemyCoordinates = new List<Vector3Int>();
    public MyRoundState MyRound;
    public Land attackLand;
    
    public virtual void ClickAction(Land clickLand){ }
    public virtual void UpdateFunc() { }
    public virtual void OnClickCard(int cardID) { }

    public AttackStateBase(MyRoundState myRound)
    {
        MyRound = myRound;
    }
    #region 可调用函数
    public bool CurMouseLandCanAttack(Land land)
    {
        return CurMouseLandCanSelect(land) && land.ArmyCanMove();
    }
    public bool CurMouseLandCanSelect(Land land)
    {
        return land.CampID == BattleManager.Instance.CurCamp.campID ;
    }
    public void HideLandOpearteMenu()
    {
        LandOperateMenu.ShowSelf(false);
    }
    public void SetNeighborEnemyHighLight(Vector3Int coordinate)
    {
        if (NeighborEnemyCoordinates.Count != 0)
        {
            NeighborEnemyCoordinates.Clear();
        }
        Vector3Int neighborCoordinate;
        //以0，0为中心块，检查其四周环绕的六个地块
        //正上方 （1，0）
        neighborCoordinate = new Vector3Int(coordinate.x + 1, coordinate.y, 0);
        CheckNeighborLandHighlight(neighborCoordinate);
        //右上方 （0，1）
        neighborCoordinate = coordinate.y % 2 == 0 ? new Vector3Int(coordinate.x, coordinate.y + 1, 0)
                                                   : new Vector3Int(coordinate.x + 1, coordinate.y + 1, 0);
        CheckNeighborLandHighlight(neighborCoordinate);
        //右下方 （-1，1）
        neighborCoordinate = coordinate.y % 2 == 0 ? new Vector3Int(coordinate.x - 1, coordinate.y + 1, 0)
                                                   : new Vector3Int(coordinate.x, coordinate.y + 1, 0);
        CheckNeighborLandHighlight(neighborCoordinate);
        //正下方 （-1，0）
        neighborCoordinate = new Vector3Int(coordinate.x - 1, coordinate.y, 0);
        CheckNeighborLandHighlight(neighborCoordinate);
        //左下方 （-1，-1）
        neighborCoordinate = coordinate.y % 2 == 0 ? new Vector3Int(coordinate.x - 1, coordinate.y - 1, 0)
                                                   : new Vector3Int(coordinate.x, coordinate.y - 1, 0);
        CheckNeighborLandHighlight(neighborCoordinate);
        //左上方 （0，-1）
        neighborCoordinate = coordinate.y % 2 == 0 ? new Vector3Int(coordinate.x, coordinate.y - 1, 0)
                                                   : new Vector3Int(coordinate.x + 1, coordinate.y - 1, 0);
        CheckNeighborLandHighlight(neighborCoordinate);

        for (int i = 0; i < NeighborEnemyCoordinates.Count; i++)
        {
            GlobalUImanager.Instance.MultipleLandHighLight[i].GetComponent<LandHighLightSide>().SetPosition(NeighborEnemyCoordinates[i]);
        }


    }

    public void CheckNeighborLandHighlight(Vector3Int coordinate)
    {
        if (BattleManager.Instance.isCoordinateInMap(coordinate))
        {
            Land land = BattleManager.Instance.BattleMap.Coordinate2Land(coordinate);
            if (land.CampID != attackLand.CampID)
            {
                NeighborEnemyCoordinates.Add(land.CoordinateInMap);
            }
        }
    }
    public void CancelHighlight()
    {

        foreach (GameObject landHighlight in GlobalUImanager.Instance.MultipleLandHighLight)
        {
            landHighlight.GetComponent<LandHighLightSide>().ShowSelf(false);
        }
        if (NeighborEnemyCoordinates.Count == 0)
        {
            return;
        }
        NeighborEnemyCoordinates.Clear();
    }
    public void Cancel()
    {
        HideLandOpearteMenu();
        CancelHighlight();
    }
    #endregion
}


