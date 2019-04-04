using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class CustomTerrain {
    public string name;
    public int ID;
    public int population;
    public int gold;
    public int food;
    public string description;
    public string tileName;
    public Tile tile;

    public CustomTerrain(int ID, string name, Tile tile)
    {
        this.ID = ID;
        this.name = name;
        this.tile = tile;
    }

    public CustomTerrain(TerrainModel terrainModel)
    {
        this.ID = terrainModel.terrainID;
        this.name = terrainModel.terrainName;
        this.population = terrainModel.population;
        this.gold = terrainModel.gold;
        this.food = terrainModel.food;
        this.description = terrainModel.description;
        this.tile = Resources.Load<Tile>(terrainModel.tilePath) as Tile;
    }

}
