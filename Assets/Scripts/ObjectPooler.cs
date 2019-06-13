﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        [HideInInspector] public Transform _parent;
        public int size;
    }
    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    public static ObjectPooler op;

    // Start is called before the first frame update
    void Awake()
    {
        if (op==null) op = this;
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, pool._parent);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }



    public GameObject Spawn(string tag, Vector2? position = null,  Quaternion? rotation = null, Transform parent = null, bool willSpawn = false)
    {
        GameObject obj = poolDictionary[tag].Dequeue();
        obj.transform.SetParent(parent);
        obj.SetActive(true);
        obj.transform.position = position ?? Vector2.zero;
        poolDictionary[tag].Enqueue(obj);
        return obj;

    }

   

 
}
