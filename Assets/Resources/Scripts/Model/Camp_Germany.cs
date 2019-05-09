using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Camp_Germany : Camp {

    public Camp_Germany(int campID, string name, int initialLands, Tile tile, Sprite baseUnitSprite,string PlayerName, int cardStartIndex, int cardEndIndex)
        : base(campID, name, initialLands, tile, baseUnitSprite,PlayerName, cardStartIndex, cardEndIndex)
    {
        CardEnum_FuncDic.Add(BattleCardFuncEnum.BLITZ,Blitz);
    }

    public override int GetExtraDiceRollsAttackByLand(Land land, bool isAttack)
    {
        int result = 0;
        if (isAttack)
        {
            if (land.ArmyType == ArmyType.TankWithUnit)
            {
                result += 2;
            }
            else if (land.ArmyType == ArmyType.AirPlaneWithUnit)
            {
                result += 2;
            }
        }
        else
        {
            result = 0;
        }
        return result;
    }

    private int Blitz(Land land)
    {
        BattleManager.Instance.CurCamp.hasCardBuff = true;
        BattleManager.Instance.CurCamp.CampBuffCardEffect.AttackConsumeReduce += 1;
        return 0;
    }
}
