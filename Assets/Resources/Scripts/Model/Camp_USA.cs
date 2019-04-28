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

    public Camp_USA(int campID, string name, int initialLands, Tile tile, Sprite baseUnitSprite,string PlayerName)
        : base(campID,name,initialLands,tile,baseUnitSprite, PlayerName) { }


}
