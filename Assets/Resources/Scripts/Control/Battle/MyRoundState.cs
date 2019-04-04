using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class MyRoundState : BattleStateBase {

    private List<Vector3Int> NeighborEnemyCoordinates = new List<Vector3Int>();

  
    private MapRightMouseMenu _mapRightMouseMenu;
    public MapRightMouseMenu MapRightMouseMenu
    {
        get
        {
            if (_mapRightMouseMenu == null)
            {
                GameObject temp = Resources.Load<GameObject>("Prefabs/UI/MapRightMouseMenu");
                _mapRightMouseMenu = Object.Instantiate(temp, GlobalUImanager.Instance.UIRoot).GetComponent<MapRightMouseMenu>();
                _mapRightMouseMenu.gameObject.SetActive(false);
            }
            return _mapRightMouseMenu;
        }
    }

    private Land _curAttackLand;


    private bool _enterMap;
    private Land MouseClickBtnLand;
    public override void RegisteMouseEvent(ref UnityAction mapEnterAction, ref UnityAction<Land, PointerEventData> mapClickAction, ref UnityAction mapExitAction)
    {
        base.RegisteMouseEvent(ref mapEnterAction, ref mapClickAction, ref mapExitAction);
        mapEnterAction = delegate () 
        {
            _enterMap = true;
            
        };
        mapClickAction = delegate (Land land, PointerEventData eventData)
        {           
            MouseClickBtnLand = land;
            //如果点击的是左键，则进行游戏操作
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                //如果当前没有进攻地块，则视为选进攻地块的操作
                if (_curAttackLand == null &&
                CurMouseLandCanAttack(MouseClickBtnLand))
                {
                    //弹出菜单选项
                    MapRightMouseMenu.ShowSelfOnLandPosition(MouseClickBtnLand.CoordinateInMap);
                    MapRightMouseMenu.attackBtn.onClick.AddListener(
                        delegate ()
                        {
                            _curAttackLand = MouseClickBtnLand;
                            //周围敌方格子有高亮框
                            SetNeighborEnemyHighLight(_curAttackLand.CoordinateInMap);
                            HideMouseRightMenu();
                        });
                }
                //如果有进攻地块，则视为进攻其他地块的操作
                else
                if (_curAttackLand != null &&
                    BattleManager.Instance.IsTwoLandNeighbor(_curAttackLand.CoordinateInMap, MouseClickBtnLand.CoordinateInMap) &&
                    MouseClickBtnLand.CampID != _curAttackLand.CampID)
                {
                    BattleManager.Instance.TwoLandFight(_curAttackLand, MouseClickBtnLand);
                    Cancel();
                }
                else
                {
                    Cancel();
                }
            }
            //点右键或其他都视为取消
            else 
            {
                Cancel();
            }
        };

        mapExitAction = delegate ()
        {
            _enterMap = false;
        };

    }


    #region Update方法

    private Vector3 mousePosition;
    
    private Land MouseHoverLand;
    public override void OnUpdateFunc()
    {
        base.OnUpdateFunc();

        if (_enterMap && BattleManager.Instance.CanAttack && !MapRightMouseMenu.isActiveAndEnabled && _curAttackLand == null)
        {
            mousePosition = Input.mousePosition;
            MouseHoverLand = BattleManager.Instance.BattleMap.GetCurMouseLand(mousePosition);
            //如果指向的是己方阵营，并且当前阵营可进攻，显示高亮框
            if (CurMouseLandCanAttack(MouseHoverLand))
            {
                GlobalUImanager.Instance.SingleLandHighLight.GetComponent<LandHighLightSide>().SetPosition(MouseHoverLand.CoordinateInMap);
            }
            else
            {
                GlobalUImanager.Instance.SingleLandHighLight.GetComponent<LandHighLightSide>().ShowSelf(false);
            }


        }
        else
        {
            GlobalUImanager.Instance.SingleLandHighLight.GetComponent<LandHighLightSide>().ShowSelf(false);
        }

       
    }

    #endregion
    public override void RefreshUI()
    {
        base.RefreshUI();
    }

    public override void ExitState(ref UnityAction mapEnterAction, ref UnityAction<Land, PointerEventData> mapClickAction, ref UnityAction mapExitAction)
    {
        base.ExitState(ref mapEnterAction, ref mapClickAction, ref mapExitAction);
        Cancel();
    }

    public  void Cancel()
    {
        HideMouseRightMenu();
        _curAttackLand = null;
        CancelHighlight();
    }
    private bool CurMouseLandCanAttack(Land land)
    {
        return land.CampID == BattleManager.Instance.CurCamp.campID && land.ArmyCanMove();
    }

    private void HideMouseRightMenu()
    {
        MapRightMouseMenu.ShowSelf(false);
    }

    private void SetNeighborEnemyHighLight(Vector3Int coordinate)
    {
        if (NeighborEnemyCoordinates.Count!=0)
        {
            NeighborEnemyCoordinates.Clear();
        }
        Vector3Int neighborCoordinate;
        //以0，0为中心块，检查其四周环绕的六个地块
        //正上方 （1，0）
        neighborCoordinate = new Vector3Int(coordinate.x+1, coordinate.y, 0);
        CheckNeighborLandHighlight(neighborCoordinate);
        //右上方 （0，1）
        neighborCoordinate = coordinate.y % 2 == 0 ? new Vector3Int(coordinate.x, coordinate.y + 1, 0)
                                                   : new Vector3Int(coordinate.x+1, coordinate.y + 1, 0);
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
                                                   : new Vector3Int(coordinate.x , coordinate.y - 1, 0);
        CheckNeighborLandHighlight(neighborCoordinate);
        //左上方 （0，-1）
        neighborCoordinate = coordinate.y % 2 == 0 ? new Vector3Int(coordinate.x , coordinate.y - 1, 0) 
                                                   : new Vector3Int(coordinate.x + 1, coordinate.y - 1, 0);
        CheckNeighborLandHighlight(neighborCoordinate);

        for (int i = 0; i < NeighborEnemyCoordinates.Count; i++)
        {
            GlobalUImanager.Instance.MultipleLandHighLight[i].GetComponent<LandHighLightSide>().SetPosition(NeighborEnemyCoordinates[i]);
        }


    }

    private void CheckNeighborLandHighlight(Vector3Int coordinate)
    {
        if (BattleManager.Instance.isCoordinateInMap(coordinate))
        {
            Land land = BattleManager.Instance.BattleMap.Coordinate2Land(coordinate);
            if (land.CampID != _curAttackLand.CampID)
            {
                NeighborEnemyCoordinates.Add(land.CoordinateInMap);
            }
        }
    }
    private void CancelHighlight()
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

    
}
