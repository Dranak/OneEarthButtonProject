using UnityEngine;
using GD.MinMaxSlider;


[System.Serializable]
public class ObstacleSpawnable : Spawnable
{
    /*public enum ObstacleKind
    {
        FIXED = 0,
        ROTATIVE,
        LINEMOVE
    }*/
    public float BodyRotation;
    public Vector2 BoundsSize; // the spawnable square size (X and Y it takes)
    public Vector2 BodyOffset; // body sub-object offset, used for obstacle offsetting post-rotation
    [MinMaxSlider(-16, 16)]
    public Vector2Int RotationOffsetRange;
    //public ObstacleKind ObsKind;

    ObstacleSpawnable(string _tag, Vector2 blocPos, Vector2Int OffsetXr, Vector2Int OffsetYr, int prefabIdx, float BodyRot, Vector2 boundS, Vector2 BodyOff, Vector2Int RotationOffR) : base (_tag, blocPos, OffsetXr, OffsetYr, prefabIdx)
    {
        BodyRotation = BodyRot;
        BoundsSize = boundS;
        BodyOffset = BodyOff;
        RotationOffsetRange = RotationOffR;
    }

    public override Spawnable Clone()
    {
        var _thisSpawnableClone = new ObstacleSpawnable(Tag, BlocPosition, OffsetXRange, OffsetYRange, SpawnablePrefabIndex, BodyRotation, BoundsSize, BodyOffset, RotationOffsetRange);
        return _thisSpawnableClone;
    }
}
