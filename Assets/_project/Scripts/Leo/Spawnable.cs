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

    protected Spawnable(string _tag, Vector2 blocPos, Vector2Int OffsetXr, Vector2Int OffsetYr, int prefabIdx)
    {
        Tag = _tag;
        BlocPosition = blocPos;
        OffsetXRange = OffsetXr;
        OffsetYRange = OffsetYr;
        SpawnablePrefabIndex = prefabIdx;
    }

    public abstract Spawnable Clone();
}
