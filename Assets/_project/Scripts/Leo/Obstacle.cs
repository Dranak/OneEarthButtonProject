using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public void SetObstacle(in Vector2Int _size, in Vector2Int _blocPos, in Quaternion _bodyRot, in Vector2 _bodyOffset, in int _prefabIndex, in Vector2 _rectBounds)
    {
        obstacleParameters.Size = _size;
        obstacleParameters.BlocPosition = _blocPos;
        obstacleParameters.BodyRotation = _bodyRot;
        obstacleParameters.BodyOffset = _bodyOffset;
        obstacleParameters.ObstaclePrefabIndex = _prefabIndex;
        obstacleParameters.BoundsSize = _rectBounds;
    }

    public enum ObstacleRotation // LEGACY
    {
        HORIZONTAL = 0,
        SIDEWAYS,
        VERTICAL // default
    }
    public ObstacleRotation obstacleRotation;

    public ObstacleSpawnable obstacleParameters;

    public Transform objectBody;

    private void OnEnable()
    {
        objectBody.localRotation = obstacleParameters.BodyRotation;
        objectBody.localPosition = obstacleParameters.BodyOffset;
    }
}
