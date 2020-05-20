using UnityEngine;
using GD.MinMaxSlider;

[System.Serializable]
public abstract class Spawnable
{
    [HideInInspector] public string Tag;
    public Vector2 BlocPosition; // local position within the bloc (from the spawnables anchor)
    [MinMaxSlider(-6, 6)]
    public Vector2Int OffsetXRange;
    [MinMaxSlider(-9, 9)]
    public Vector2Int OffsetYRange;
    [HideInInspector] public int SpawnablePrefabIndex; // prefab index within the scriptable object
}
