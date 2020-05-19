using UnityEngine;

public class Collectible : SpawnableObject
{
    public CollectibleSpawnable collectibleParameters;

    public void SetCollectible(in Vector2Int _blocPos, in int _prefabIndex) // + new parameters?
    {
        base.SetSpawnable(collectibleParameters, _blocPos, _prefabIndex);
        // + new parameters?
    }
    public void SetCollectible(in Vector2Int _blocPos, in int _prefabIndex, in Vector2 _bodyOffset) // + parameter for precise positionning (smaller than 1)
    {
        base.SetSpawnable(collectibleParameters, _blocPos, _prefabIndex, _bodyOffset);
        SetAnchorBodyPos();
    }
    void SetAnchorBodyPos()
    {
        collectibleParameters.anchorBodyPosition = collectibleParameters.BlocPosition + collectibleParameters.BodyOffset - collectibleParameters.BoundsSize / 2;
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
