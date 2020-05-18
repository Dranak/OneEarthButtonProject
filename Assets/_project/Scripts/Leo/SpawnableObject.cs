using UnityEngine;

public abstract class SpawnableObject : MonoBehaviour
{
    public Transform objectBody;

    public virtual void SetSpawnable(Spawnable _spawnableParameters, in Vector2Int _blocPos, in Quaternion _bodyRot, in Vector2 _rectBounds, in int _prefabIndex)
    {
        _spawnableParameters.BlocPosition = _blocPos;
        _spawnableParameters.BodyRotation = _bodyRot;
        _spawnableParameters.BoundsSize = _rectBounds;
        _spawnableParameters.SpawnablePrefabIndex = _prefabIndex;

    }

    public abstract void GetSpawnable(out Spawnable spawnable);
    public abstract Spawnable GetSpawnable();
}
