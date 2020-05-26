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
    [HideInInspector] public BlocsStorageScriptableObject blocsScriptable;
    public GameObject[] prefabPallete;

    [HideInInspector] public bool useGrid = true;
    [HideInInspector] public int selectedPrefabIndex = 0;
    [HideInInspector] public float maxIntersectionVolume = 0;
    [HideInInspector] public List<string> blocNames; // available blocs names
    [HideInInspector] public string blocName = "Enter Bloc Name"; // bloc name to store
    [HideInInspector] public Bloc currentBlocSelection = null;

    public GameObject SelectedPrefab
    {
        get
        {
            return prefabPallete == null || prefabPallete.Length == 0 ? null : prefabPallete[selectedPrefabIndex];
        }
    }

    public void OnValidate()
    {
        if (prefabPallete != blocsScriptable.obstaclesPrefabs.ToArray())
        {
            blocsScriptable.obstaclesPrefabs = prefabPallete.ToList();
        }
    }
}
