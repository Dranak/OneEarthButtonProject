using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollectibleSpawnable : Spawnable
{
    public int EggShellIndex = -1;

    CollectibleSpawnable(string _tag, Vector2 blocPos, Vector2Int OffsetXr, Vector2Int OffsetYr, int prefabIdx, int eggShellIdx) : base(_tag, blocPos, OffsetXr, OffsetYr, prefabIdx)
    {
        EggShellIndex = eggShellIdx;
    }

    public override Spawnable Clone()
    {
        var _thisSpawnableClone = new CollectibleSpawnable(Tag, BlocPosition, OffsetXRange, OffsetYRange, SpawnablePrefabIndex, EggShellIndex);
        return _thisSpawnableClone;
    }
}
