using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleReloadMapUI : MonoBehaviour {

    public Animator animator;
    public GameObject BGobject;

    public void PlayAnimation()
    {
        this.gameObject.SetActive(true);   
    }

    public float GetAnimationTime()
    {
        float time = animator.GetCurrentAnimatorStateInfo(0).length;
        return time;
    }

   



}
