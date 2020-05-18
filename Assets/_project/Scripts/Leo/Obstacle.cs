using UnityEngine;

public class Obstacle : SpawnableObject
{
    public enum ObstacleRotation // LEGACY
    {
        HORIZONTAL = 0,
        SIDEWAYS,
        VERTICAL // default
    }
    public ObstacleRotation obstacleRotation;

    public ObstacleSpawnable obstacleParameters;

    public void SetObstacle(in Vector2Int _blocPos, in Vector2 _rectBounds, in Quaternion _bodyRot, in Vector2 _bodyOffset, in int _prefabIndex)
    {
        base.SetSpawnable(obstacleParameters, _blocPos, _bodyRot, _rectBounds, _prefabIndex);
        obstacleParameters.BodyOffset = _bodyOffset;
    }

    private void OnEnable()
    {
        objectBody.localRotation = obstacleParameters.BodyRotation;
        objectBody.localPosition = obstacleParameters.BodyOffset;
    }

    public override void GetSpawnable(out Spawnable obstacleSpawnable)
    {
        obstacleSpawnable = obstacleParameters as ObstacleSpawnable;
    }
    public override Spawnable GetSpawnable()
    {
        return obstacleParameters as ObstacleSpawnable;
    }
}
