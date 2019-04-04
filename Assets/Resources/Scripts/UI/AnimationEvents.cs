using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour {

    public GameObject BGobject;
    public void SetHideSelf()
    {
        gameObject.SetActive(false);
        BGobject.SetActive(false);
    }
}
