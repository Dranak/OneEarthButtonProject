using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteAlways]
public class BlocCreator : MonoBehaviour
{
    public enum CollisionTest
    {
        RendererBounds,
        ColliderBounds
    }

    [HideInInspector] public LayerMask layerMask;
    [HideInInspector] public Transform rootTransform;
    public GameObject[] prefabPallete;
    public BlocsStorageScriptableObject blocScriptable;

    [HideInInspector] public bool useGrid = true;
    [HideInInspector] public int selectedPrefabIndex = 0;
    [HideInInspector] public float maxIntersectionVolume = 0;
    public CollisionTest collisionTest;
    [HideInInspector] public List<string> blocNames; // available blocs names

    public GameObject SelectedPrefab
    {
        get
        {
            return prefabPallete == null || prefabPallete.Length == 0 ? null : prefabPallete[selectedPrefabIndex];
        }
    }

    public void OnValidate()
    {
        if (prefabPallete != blocScriptable.obstaclesPrefabs.ToArray())
        {
            blocScriptable.obstaclesPrefabs = prefabPallete.ToList();
        }
    }
}
