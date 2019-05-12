using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Camp_Soviet : Camp {



    public Camp_Soviet(int campID, string name, int initialLands, Tile campTile, Tile cannonTile, Sprite baseUnitSprite,string PlayerName, int cardStartIndex, int cardEndIndex)
        : base(campID, name, initialLands, campTile,cannonTile, baseUnitSprite, PlayerName, cardStartIndex, cardEndIndex)
    {
        CardEnum_FuncDic.Add(BattleCardFuncEnum.COMMUNIST,Communist);
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
                result += 2;
            }
        }
        else
        {
            if (land.BattleUnit <= 3)
            {
                result += 1;
            }
        }
        return result;
    }

    private int Communist(Land land)
    {
        Camp curCamp = BattleManager.Instance.CurCamp;
        curCamp.hasCardBuff = true;
        curCamp.CampBuffCardEffect.BombConsumeReduce += curCamp.CannonAttackConsumGold;
        return 0;
    }
}
