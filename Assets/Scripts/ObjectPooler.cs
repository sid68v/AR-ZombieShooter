using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class ObjectPoolItem
{
    public GameObject objectToPool;
    public int amountToPool;
    public bool isPoolExpandable = false;
}


public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    [HideInInspector]
    public List<GameObject> pooledObjects;

    public List<ObjectPoolItem> itemsToPool;


    private void Awake()
    {
        if (!Instance)
            Instance = this;
    }

   

    // Start is called before the first frame update
    void Start()
    {

        // Instantiate and fill the pool at start.

        pooledObjects = new List<GameObject>();

        foreach (ObjectPoolItem item in itemsToPool)
        {
            for (int poolCount = 0; poolCount < item.amountToPool; poolCount++)
            {
                GameObject go = Instantiate(item.objectToPool);
                go.SetActive(false);
                pooledObjects.Add(go);
            }
        }


    }


    // gets an inactive pooled object from the pool.
    public GameObject GetObjectFromPool(string itemTag)
    {

        // return object if exists.
        for (int poolCount = 0; poolCount < pooledObjects.Count; poolCount++)
        {
            if (!pooledObjects[poolCount].activeInHierarchy && pooledObjects[poolCount].CompareTag(itemTag))
                return pooledObjects[poolCount];
        }

        // create and return object if it does not exist.
        foreach (ObjectPoolItem item in itemsToPool)
        {
            if (item.objectToPool.CompareTag(itemTag))
            {
                if (item.isPoolExpandable)
                {
                    GameObject go = Instantiate(item.objectToPool);
                    go.SetActive(false);
                    pooledObjects.Add(go);
                    return go;
                }
            }
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
