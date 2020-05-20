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
    public Vector2 BodyOffset; // body sub-object offset, used for obstacle offsetting post-rotation
    public ObstacleKind ObsKind;
    [MinMaxSlider(-6, 6)]
    public Vector2Int OffsetXRange;
    [MinMaxSlider(-9, 9)]
    public Vector2Int OffsetYRange;
    [MinMaxSlider(-16, 16)]
    public Vector2Int RotationOffsetRange;
}
