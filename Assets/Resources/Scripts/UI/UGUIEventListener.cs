using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UGUIEventListener : EventTrigger
{

    public UnityAction onClick;
    public UnityAction onEnter;
    public UnityAction onExit;
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (onClick!=null)
        {
            onClick();
        }
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (onEnter!=null)
        {
            onEnter();
        }
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (onExit != null)
        {
            onExit();
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
