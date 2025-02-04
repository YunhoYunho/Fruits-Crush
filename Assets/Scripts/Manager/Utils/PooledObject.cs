using System.Collections;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    [SerializeField]
    private float returnTime;

    public PoolManager poolManager;

    private void Start()
    {
        poolManager = FindObjectOfType<PoolManager>();
    }

    private void OnEnable()
    {
        StartCoroutine(DelayToReturn());
    }

    private IEnumerator DelayToReturn()
    {
        yield return new WaitForSeconds(returnTime);

        poolManager.Release(gameObject);
    }
}
