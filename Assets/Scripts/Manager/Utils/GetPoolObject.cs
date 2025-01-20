using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPoolObject : MonoBehaviour
{
    public Dictionary<string, GameObject> dic = new Dictionary<string, GameObject>();
    public PoolManager poolManager;

    [SerializeField]
    private GameObject[] prefabs;
    private string key;

    public GameObject GetPool(string get, Vector3 position, Quaternion rotation)
    {
        key = get;
        return poolManager.StringGet(key, position, rotation);
    }
}
