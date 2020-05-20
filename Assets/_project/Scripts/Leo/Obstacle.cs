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

    public void SetObstacle(in Vector2 _blocPos, in int _prefabIndex, in Vector2 _bodyOffset, in Vector2 _rectBounds, in float _bodyRot)
    {
        base.SetSpawnable(obstacleParameters, _blocPos, _prefabIndex);
        obstacleParameters.BoundsSize = _rectBounds;
        obstacleParameters.BodyRotation = _bodyRot;
        obstacleParameters.BodyOffset = _bodyOffset;
    }

    private void OnEnable()
    {
        objectBody.localRotation = Quaternion.Euler(0, 0, obstacleParameters.BodyRotation);
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
