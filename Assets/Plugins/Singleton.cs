using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 单例类
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour where T:MonoBehaviour {
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance==null)
            {
                GameObject go = new GameObject(typeof(T).Name+"Instance");
                _instance = go.AddComponent<T>();
                DontDestroyOnLoad(go);
            }

            return _instance;

        }
    }

    public virtual void Initial()
    {

    }

    private void Start()
    {
        Initial();
    }

}
