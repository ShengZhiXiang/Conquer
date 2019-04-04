using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Terrain_MapModel  {
    public int size;
    public List<Pair> terrainID_AmountList;

    [System.Serializable]
    public struct Pair
    {
        public Pair(int key,int value)
        {
            this.key = key;
            this.value = value;
        }
      public  int key;
      public  int value;
    }
}
