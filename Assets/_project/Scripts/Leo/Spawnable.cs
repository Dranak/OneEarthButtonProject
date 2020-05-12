using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spawnable
{
    public Vector2Int Size;
    public Vector2Int BlocPosition;
}

public enum ObstacleKind
{
    FIXED = 0,
    ROTATIVE,
    LINEMOVE
}

[System.Serializable]
public class ObstacleSpawnable : Spawnable
{
    public Quaternion BodyRotation;
    public Vector2 BodyOffset;
    /*[HideInInspector] */public uint ObstaclePrefabIndex;
    public ObstacleKind ObsKind;
}

[System.Serializable]
public class CoinsSpawnable : Spawnable
{

}