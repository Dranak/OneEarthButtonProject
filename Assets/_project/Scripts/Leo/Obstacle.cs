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

    public enum ObstacleRotation
    {
        HORIZONTAL = 0,
        SIDEWAYS,
        VERTICAL // default
    }
    public ObstacleRotation obstacleRotation;

    public Vector2Int size;
    //public Vector2 boxSize;
    public Transform objectBody;
}
