using UnityEngine;

public class Collectible : SpawnableObject
{
    public CollectibleSpawnable collectibleParameters;

    public void SetCollectible(in Vector2Int _blocPos, in int _prefabIndex) // + new parameters?
    {
        base.SetSpawnable(collectibleParameters, _blocPos, _prefabIndex);
        // + new parameters?
    }

    private void OnEnable()
    {

    }
    public override void GetSpawnable(out Spawnable collectibleSpawnable)
    {
        collectibleSpawnable = collectibleParameters as CollectibleSpawnable;
    }
    public override Spawnable GetSpawnable()
    {
        return collectibleParameters as CollectibleSpawnable;
    }
}
