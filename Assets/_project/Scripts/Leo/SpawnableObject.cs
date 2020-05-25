using UnityEngine;

public abstract class SpawnableObject : MonoBehaviour
{
    public Vector2 Size; // the grid-wise size -> shall not change ig (part of the prefab)
    public Transform objectBody;

    public virtual void SetSpawnable(Spawnable _spawnableParameters, in Vector2 _blocPos, in int _prefabIndex)
    {
        _spawnableParameters.Tag = name;
        _spawnableParameters.BlocPosition = _blocPos;
        _spawnableParameters.SpawnablePrefabIndex = _prefabIndex;
    }

    public abstract void SetSpawnable(Spawnable _spawnableParameters);

    public abstract void GetSpawnable(out Spawnable spawnable);
    public abstract Spawnable GetSpawnable();
}
