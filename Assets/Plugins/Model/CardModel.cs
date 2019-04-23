using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CardModel  {
    public int cardID;
    public string cardName;
    public string spritePath;
    public bool isCampCard;
    public int belongCampID;
    public string cardFuncEnum;
    public string cardTriggerTime;
    public string funcDescription;
    public int costGold;
    public int costPopulation;
    public int isSelfCard;

}
