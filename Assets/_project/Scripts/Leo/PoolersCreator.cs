using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class PoolersCreator : MonoBehaviour
{
    public BlocsStorageScriptableObject selectedBlocsScriptable;
    public int poolsSize;
    public int breadPoolMult = 3;
    [SerializeField, HideInInspector] public SerializableList[] spawnablePoolsObjects; // lists of objects within the pools (spawnables only)

    public void SetSpawnablesPools()
    {
        spawnablePoolsObjects = new SerializableList[transform.childCount];
        //System.Array.Resize(ref spawnablePoolsObjects, transform.childCount);
        int counter = 0;
        foreach (Transform poolC in transform)
        {
            spawnablePoolsObjects[counter] = new SerializableList();
            spawnablePoolsObjects[counter].objectPool.AddRange(poolC.Cast<Transform>().Select(w => w.gameObject));
            ++counter;
        }
    }
}

[System.Serializable]
public class SerializableList
{
    [SerializeField] public List<GameObject> objectPool = new List<GameObject>();
}
