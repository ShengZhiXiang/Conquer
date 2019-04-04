using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MsgBox2Btns : UIMsgBox {

    public Text _text;
    public Button _confirmBtn;
    public Button _cancelBtn;

    public override void Initial()
    {
        base.Initial();
    }

    public override void SetMsgBox(string content, UnityAction confirmBtnAction, UnityAction cancelBtnAction)
    {
        _text.text = content;
        _confirmBtn.onClick.AddListener(confirmBtnAction);
        _cancelBtn.onClick.AddListener(cancelBtnAction);
    }
}
