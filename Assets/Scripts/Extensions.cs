using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Extensions
{

    public static void DeActivate(this GameObject obj)
    {
        ObjectPooler.op.poolDictionary[obj.tag].Enqueue(obj);
        obj.SetActive(false);

    }
}
