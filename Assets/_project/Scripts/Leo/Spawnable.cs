using UnityEngine;

[System.Serializable]
public class Spawnable
{
    public Vector2Int Size; // the grid-wise size (1 as minimum)
    public Vector2Int BlocPosition;
    public Vector2 BodyOffset;
    public Vector2 BoundsSize; // the obstacle square size (X and Y it takes)

    [HideInInspector] public int SpawnablePrefabIndex;
}
