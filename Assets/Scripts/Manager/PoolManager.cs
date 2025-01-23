using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private static GameObject container;
    private static PoolManager instance;
    public static PoolManager Instance
    {
        get
        {
            if (null == instance)
            {
                instance = FindObjectOfType<PoolManager>();
                if (null == instance)
                {
                    container = new GameObject("PoolManager");
                    instance = container.AddComponent<PoolManager>();
                    DontDestroyOnLoad(container);
                }
            }
            return instance;
        }
    }

    private Dictionary<string, Stack<GameObject>> poolDic;

    [SerializeField]
    private List<Poolable> poolPrefab;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        poolDic = new Dictionary<string, Stack<GameObject>>();
    }

    private void Start()
    {
        CreatePool();
    }

    public void CreatePool()
    {
        for (int i = 0; i < poolPrefab.Count; i++)
        {
            Stack<GameObject> stack = new Stack<GameObject>();
            for (int j = 0; j < poolPrefab[i].count; j++)
            {
                GameObject instance = Instantiate(poolPrefab[i].prefab);
                instance.SetActive(false);
                instance.gameObject.name = poolPrefab[i].prefab.name;
                instance.transform.parent = poolPrefab[i].container;
                stack.Push(instance);
            }
            poolDic.Add(poolPrefab[i].prefab.name, stack);
        }
    }

    public GameObject StringGet(string name, Vector3 position, Quaternion rotation)
    {
        Stack<GameObject> stack = poolDic[name];
        if (stack.Count > 0)
        {
            GameObject instance = stack.Pop();
            instance.gameObject.SetActive(true);
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            return instance;
        }
        else
            return null;
    }

    public void Release(GameObject instance)
    {
        Stack<GameObject> stack = poolDic[instance.name];
        Poolable poolable = poolPrefab.Find((x) => instance.name == x.prefab.name);
        instance.transform.SetParent(poolable.container);
        instance.SetActive(false);
        stack.Push(instance);
    }
}

[Serializable]
public struct Poolable
{
    public GameObject prefab;
    public int count;
    public Transform container;
}
