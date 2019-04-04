using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class UIMsgBox : MonoBehaviour {

    public virtual void Initial()
    {

    }
    private void Start()
    {
        Initial();
    }

    public virtual void SetMsgBox() { }
    public virtual void SetMsgBox(string text) { }
    public virtual void SetMsgBox(string text, UnityAction ConfirmBtnAction) { }
    public virtual void SetMsgBox(string text, UnityAction ConfirmBtnAction, UnityAction CancelBtnAction) { }

}
