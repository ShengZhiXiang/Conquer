using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UGUIEventListener : EventTrigger
{

    public UnityAction onClick;
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (onClick!=null)
        {
            onClick();
        }
    }

    static public UGUIEventListener Get(GameObject go)
    {
       
        if (go.GetComponent<UGUIEventListener>()==null)
        {
            go.AddComponent<UGUIEventListener>();
        }
        return go.GetComponent<UGUIEventListener>();
    }
}
