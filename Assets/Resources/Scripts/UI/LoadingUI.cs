using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingUI : MonoBehaviour {

    public Animator animator;
    public GameObject BGobject;

    public void PlayAnimation()
    {
        animator.gameObject.SetActive(true);
        BGobject.SetActive(true);      
    }

    public float GetAnimationTime()
    {
        float time = animator.GetCurrentAnimatorStateInfo(0).length;
        return time;
    }

   



}
