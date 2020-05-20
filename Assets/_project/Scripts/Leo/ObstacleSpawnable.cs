using UnityEngine;
using GD.MinMaxSlider;


[System.Serializable]
public class ObstacleSpawnable : Spawnable
{
    public enum ObstacleKind
    {
        FIXED = 0,
        ROTATIVE,
        LINEMOVE
    }
    public float BodyRotation;
    public Vector2 BoundsSize; // the spawnable square size (X and Y it takes)
    public Vector2 BodyOffset; // body sub-object offset, used for obstacle offsetting post-rotation
    [MinMaxSlider(-16, 16)]
    public Vector2Int RotationOffsetRange;
    public ObstacleKind ObsKind;
}
