using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool ObjectPoolInstance;
    private List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool;
    private int currentActiveObjs;

    private void Awake()
    {
        ObjectPoolInstance = this;
        currentActiveObjs = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < amountToPool; i++)
        {
            var obj = Instantiate(objectToPool);
            obj.transform.parent = gameObject.transform;
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

    public GameObject GetFirstPooledObject()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                pooledObjects[i].SetActive(true);
                currentActiveObjs++;
                pooledObjects.RemoveAt(i);
                return pooledObjects[i];
            }
        }
        return null;
    }

    public void ReturnPooledObject(GameObject pooledObj)
    {
        pooledObjects.Add(pooledObj);
        pooledObj.SetActive(false);
        currentActiveObjs--;
    }

    private void Update()
    {

    }
}
