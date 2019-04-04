using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
public class UIGray : MonoBehaviour {
    private bool _isGray = false;
    public bool isGray
    {
        get { return _isGray; }
        set
        {
            if (_isGray != value)
            {
                _isGray = value;
                SetGray(isGray);
            }
        }
    }

    static private Material _defaultGrayMaterial;
    static private Material grayMaterial
    {
        get
        {
            if (_defaultGrayMaterial == null)
            {
                _defaultGrayMaterial = new Material(Shader.Find("UI/Gray"));
            }
            return _defaultGrayMaterial;
        }
    }

    void SetGray(bool isGray)
    {
        Image[] images = transform.GetComponentsInChildren<Image>();
        for (int i=0;i<images.Length;i++)
        {
            Image g = images[i];
            if (isGray)
            {
                g.material = grayMaterial;
            }
            else
            {
                g.material = null;
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(UIGray))]
    public class UIGrayInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            UIGray uIGray = target as UIGray;
            uIGray.isGray = GUILayout.Toggle(uIGray.isGray,"isGray");
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }

        }
    }
#endif

}
