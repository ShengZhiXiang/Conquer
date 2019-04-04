using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopTip : MonoBehaviour {

    public Text text;
    

    public void SetContent(string content)
    {
        text.text = content;
    }

    private IEnumerator Start()
    {
       
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}
