using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Camp  {
    
    //阵营ID
    public int campID;
    //阵营名字
    public string name;
    //阵营拥有的地块List
    public List<Land> ownedLands;
    //初始分到的地块数
    public int initialLands;
    //首都地块
    public Land capitalLand;
    //该阵营使用的tile
    public Tile tile;
    public Camp(int campID,string name, int initialLands, Tile tile)
    {
        this.campID = campID;
        this.name = name;
        this.tile = tile;
        this.initialLands = initialLands;
        ownedLands = new List<Land>();
    }
}
