using UnityEngine;

public abstract class SpawnableObject : MonoBehaviour
{
    public Transform objectBody;

    public virtual void SetSpawnable(Spawnable _spawnableParameters, in Vector2Int _blocPos, in Vector2 _bodyOffset, in int _prefabIndex)
    {
        _spawnableParameters.BlocPosition = _blocPos;
        _spawnableParameters.BodyOffset = _bodyOffset;
        _spawnableParameters.SpawnablePrefabIndex = _prefabIndex;
    }

    public abstract void GetSpawnable(out Spawnable spawnable);
    public abstract Spawnable GetSpawnable();
}
