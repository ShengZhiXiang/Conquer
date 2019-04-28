using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Camp_Japan : Camp {

    public Camp_Japan(int campID, string name, int initialLands, Tile tile, Sprite baseUnitSprite,string PlayerName)
        : base(campID, name, initialLands, tile, baseUnitSprite, PlayerName) { }

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
}
