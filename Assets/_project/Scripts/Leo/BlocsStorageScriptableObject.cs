using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class BlocsStorageScriptableObject : ScriptableObject
{
    [SerializeField]
    public List<Bloc> storedBlocs;
    [SerializeField]
    public List<GameObject> obstaclesPrefabs; // must be gameObject prefabs with Obstacle attached (obstacle prefabs)
}
