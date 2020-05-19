using UnityEngine;

public abstract class SpawnableObject : MonoBehaviour
{
    public Vector2Int Size; // the grid-wise size (1 as minimum unit) -> shall not change ig (part of the prefab)
    public Transform objectBody;

    public virtual void SetSpawnable(Spawnable _spawnableParameters, in Vector2Int _blocPos, in int _prefabIndex, in Vector2 _bodyOffset)
    {
        SetSpawnable(_spawnableParameters, _blocPos, _prefabIndex);
        _spawnableParameters.BodyOffset = _bodyOffset;
    }
    public virtual void SetSpawnable(Spawnable _spawnableParameters, in Vector2Int _blocPos, in int _prefabIndex)
    {
        _spawnableParameters.Tag = name;
        _spawnableParameters.BlocPosition = _blocPos;
        _spawnableParameters.SpawnablePrefabIndex = _prefabIndex;
    }

    public abstract void GetSpawnable(out Spawnable spawnable);
    public abstract Spawnable GetSpawnable();
}
