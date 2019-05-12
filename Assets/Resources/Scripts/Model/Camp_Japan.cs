using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Camp_Japan : Camp {

    public Camp_Japan(int campID, string name, int initialLands, Tile campTile,Tile cannonTile,Sprite baseUnitSprite,string PlayerName,int cardStartIndex, int cardEndIndex)
        : base(campID, name, initialLands, campTile, cannonTile, baseUnitSprite, PlayerName, cardStartIndex, cardEndIndex)
    {
        CardEnum_FuncDic.Add(BattleCardFuncEnum.MILITARISM, Militarism);
    }

    public override int GetExtraDiceRollsAttackByLand(Land land, bool isAttack)
    {
        int result = 0;
        if (isAttack)
        {
            if (land.ArmyType == ArmyType.TankWithUnit)
            {
                result += 1;
            }
            else if (land.ArmyType == ArmyType.AirPlaneWithUnit)
            {
                result += 3;
            }
        }
        else
        {
            if (land.ArmyType == ArmyType.AirPlaneWithUnit)
            {
                result += 1;
            }
        }

        return result;
    }

    private int Militarism(Land land)
    {
        BattleManager.Instance.CurCamp.hasCardBuff = true;
        BattleManager.Instance.CurCamp.CampBuffCardEffect.ExtraSupplyUnit += 1;
        return 0;
    }
}
