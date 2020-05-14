using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD.MinMaxSlider;

public abstract class Spawnable
{
    public Vector2Int Size;
    public Vector2Int BlocPosition;
    public Vector2 BoundsSize; // the obstacle square size (X and Y it takes)
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
    [HideInInspector] public int ObstaclePrefabIndex;
    public ObstacleKind ObsKind;
    [MinMaxSlider(-6, 6)]
    public Vector2Int OffsetXRange;
    [MinMaxSlider(-9, 9)]
    public Vector2Int OffsetYRange;
    [MinMaxSlider(-16, 16)]
    public Vector2Int RotationOffsetRange;
}

[System.Serializable]
public class CoinsSpawnable : Spawnable
{

}