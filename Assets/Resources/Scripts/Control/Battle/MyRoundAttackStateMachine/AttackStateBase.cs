using System.Collections;
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
    
    
    public MyRoundState MyRound;
    public Land attackLand;
    
    public virtual void ClickAction(Land clickLand){ }
    public virtual void UpdateFunc() { }
    public virtual void OnClickCard() { }

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
    
    public void CancelHighlight()
    {
        List<Land> NeighborEnemyLands = MyRound.NeighborEnemyLands;
        if (NeighborEnemyLands.Count == 0)
        {
            return;
        }
        foreach (Land land in NeighborEnemyLands)
        {
            land.LandHighLightSide.ShowSelf(HighLightType.Mutiple,false);
        }
        
        NeighborEnemyLands.Clear();
    }
   

    
    
    #endregion
}



