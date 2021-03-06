﻿using UnityEngine;

public class Collectible : SpawnableObject
{
    public CollectibleSpawnable collectibleParameters;
    public int PointGain;
    public ParticleSystem starParticle;

    public void SetCollectible(in Vector2 _blocPos, in int _prefabIndex, in int _eggshellId = -1)
    {
        base.SetSpawnable(collectibleParameters, _blocPos, _prefabIndex);
        collectibleParameters.EggShellIndex = _eggshellId;
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
