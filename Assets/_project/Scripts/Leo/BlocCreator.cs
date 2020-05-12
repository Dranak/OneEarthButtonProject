using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class BlocCreator : MonoBehaviour
{
    public enum CollisionTest
    {
        RendererBounds,
        ColliderBounds
    }

    public LayerMask layerMask;
    public Transform rootTransform;
    public GameObject[] prefabPallete;
    public BlocsStorageScriptableObject blocScriptable;

    [HideInInspector] public bool useGrid = true;
    [HideInInspector] public int selectedPrefabIndex = 0;
    public float maxIntersectionVolume = 0;
    public CollisionTest collisionTest;

    public GameObject SelectedPrefab
    {
        get
        {
            return prefabPallete == null || prefabPallete.Length == 0 ? null : prefabPallete[selectedPrefabIndex];
        }
    }

    public Bloc createdBloc;
}
