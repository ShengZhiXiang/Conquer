using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour {

    public GameObject panelRoot;

    
    public void SetHidePanel()
    {
        if (panelRoot!=null)
        {
            panelRoot.SetActive(false);
        }    
    }

    public Animator diceResultAni;
    
    public void OnDiceEnd()
    {
        diceResultAni.SetBool("canShow",true);
    }

}
