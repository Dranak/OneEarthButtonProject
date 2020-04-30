using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public enum ObstacleKind
    {
        FIXED = 0,
        ROTATIVE,
        LINEMOVE
    }

    public enum ObstacleSpawnType
    {
        VERTICAL = 0,
        HORIZONTAL,
        SIDEWAYS
    }
    public ObstacleSpawnType obstacleSpawnType;

    public Vector2Int size;
    [HideInInspector]
    public Vector2 boxSize;
    public Transform objectT;
}
