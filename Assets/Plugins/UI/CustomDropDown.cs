using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomDropDown : Dropdown {
    public string old;
    public Action<string, string> OnSelectOption;
    public Action<GameObject> OnShowDropDownList;
    public string PlayerName;
    public override void OnPointerClick(PointerEventData eventData)
    {
        old = captionText.text;
        Show();
    }
    protected override void Start()
    {
        base.Start();
        onValueChanged.AddListener(delegate(int index)
        {
            if (OnSelectOption!=null)
            {
                OnSelectOption(old, options[index].text);
            }
        });
        PlayerName = transform.Find("PlayerName").GetComponentInChildren<Text>().text;
    }

    public  void  Show()
    {
        base.Show();
        if (OnShowDropDownList!=null)
        {
            OnShowDropDownList(transform.Find("Dropdown List/Viewport/Content").gameObject);
        }
    }
    
}
