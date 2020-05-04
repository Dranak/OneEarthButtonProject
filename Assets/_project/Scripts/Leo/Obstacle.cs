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
        HORIZONTAL = 0,
        SIDEWAYS,
        VERTICAL // default
    }
    public ObstacleSpawnType obstacleSpawnType;

    public Vector2Int size;
    //public Vector2 boxSize;
    public Transform objectT;
}
