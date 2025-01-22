using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTon<T> : MonoBehaviour where T : Component
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (null == instance)
            {
                instance = FindObjectOfType<T>();
                if (null == instance)
                {
                    GameObject gameObject = new GameObject();
                    gameObject.name = typeof(T).Name;
                    instance = gameObject.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    public virtual void Awake()
    {
        if (null == instance)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
}
