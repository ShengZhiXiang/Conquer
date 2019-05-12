using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Camp_USA : Camp {

    public override int CannonBombPoint
    {
        get
        {
            return (base.CannonBombPoint + 1);
        }
    }

    public Camp_USA(int campID, string name, int initialLands, Tile campTile,Tile cannonTile, Sprite baseUnitSprite, string PlayerName,int cardStartIndex,int cardEndIndex)
        : base(campID, name, initialLands, campTile,cannonTile, baseUnitSprite, PlayerName, cardStartIndex, cardEndIndex)
    {
        CardEnum_FuncDic.Add(BattleCardFuncEnum.ATOMIC_BOMB,AtomicBomb);
    }


    
    private int AtomicBomb(Land land)
    {

        land.BattleUnit = 0;
        land.leftPopulation = 0;
        return 0;
    }

}
