﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleCard
{


    public BattleCardTriggerTime triggerTime;
    


    public Func<Land, int> CardFunc;

    public BattleCard( BattleCardTriggerTime triggerTime,Func<Land,int> CardFunc)
    {
     
        this.triggerTime = triggerTime;
        this.CardFunc = CardFunc;
    }

}
