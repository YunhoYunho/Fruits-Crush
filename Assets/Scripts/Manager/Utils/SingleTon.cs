using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTon<T> : MonoBehaviour where T : MonoBehaviour
{
    public const string gameObjectName = "Manager";

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
                    gameObject.name = gameObjectName;
                    instance = gameObject.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}