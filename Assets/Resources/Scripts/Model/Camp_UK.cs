using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Camp_UK : Camp {
    public override int AttackConsumeGold
    {
        get
        {
            return (base.AttackConsumeGold - 1);
        }
    }

    public override int CannonAttackConsumGold
    {
        get
        {
            return (base.CannonAttackConsumGold - 1);
        }
    }

    public Camp_UK(int campID, string name, int initialLands, Tile campTile, Tile cannonTile, Sprite baseUnitSprite,string PlayerName, int cardStartIndex, int cardEndIndex)
        : base(campID, name, initialLands, campTile,cannonTile, baseUnitSprite, PlayerName, cardStartIndex, cardEndIndex) { }
}
