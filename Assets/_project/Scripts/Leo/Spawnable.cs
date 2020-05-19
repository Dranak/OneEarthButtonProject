using UnityEngine;

[System.Serializable]
public abstract class Spawnable
{
    [HideInInspector] public string Tag;
    public Vector2Int BlocPosition; // local position within the bloc (from the spawnables anchor)
    public Vector2 BodyOffset; // body sub-object offset, used either for obstacle offsetting post-rotation or collectibles fine-positioning
    public Vector2 BoundsSize; // the spawnable square size (X and Y it takes)
    [HideInInspector] public int SpawnablePrefabIndex; // prefab index within the scriptable object
}
