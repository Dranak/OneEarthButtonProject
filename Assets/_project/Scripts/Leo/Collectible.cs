using UnityEngine;

public class Collectible : SpawnableObject
{
    public CollectibleSpawnable collectibleParameters;
    public int PointGain;
    public bool IsEggShell;

    public void SetCollectible(in Vector2 _blocPos, in int _prefabIndex) // + new parameters?
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
    public override void SetSpawnable(Spawnable _spawnableParameters)
    {
        collectibleParameters = _spawnableParameters as CollectibleSpawnable;
    }
}
