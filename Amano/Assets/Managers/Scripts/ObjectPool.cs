using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool ObjectPoolInstance;
    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool;
    private int currentActiveObjs;

    private void Awake()
    {
        if (ObjectPoolInstance == null)
        {
            ObjectPoolInstance = this;
        }

        currentActiveObjs = 0;
    }

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
                /*var obj = pooledObjects[i];
                pooledObjects.RemoveAt(i);*/
                currentActiveObjs++;
                return pooledObjects[i];
            }
        }
        return null;
    }

    public void ReturnPooledObject(GameObject pooledObj)
    {
        pooledObj.SetActive(false);
        pooledObj.transform.parent = transform;
        // pooledObjects.Add(pooledObj);
        currentActiveObjs--;
    }

    public bool IsObjectAvailable()
    {
        return !pooledObjects[0].activeInHierarchy;
    }

    private void Update()
    {

    }
}
