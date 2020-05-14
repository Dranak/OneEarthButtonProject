using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD.MinMaxSlider;

[System.Serializable]
public class Bloc
{
    public Bloc(in BlocArea _blockArea, in int _blocCount, in int _blocLength, in string _blocName = "", in ObstacleSpawnable[] _obstacles = null, in Vector2Int _blocYRange = new Vector2Int(), in Vector4 _globalOffsetRange = new Vector4(), in Vector2Int _globalRotationOffsetRange = new Vector2Int())
    {
        blockArea = _blockArea;
        blocCount = _blocCount;
        blocLength = _blocLength;
        blocName = _blocName;
        obstaclesParams = _obstacles;

        if (_blocYRange != Vector2Int.zero)
            blocYRange = _blocYRange;
        if (_globalOffsetRange != Vector4.zero)
            globalOffsetRange = _globalOffsetRange;
        if (_globalRotationOffsetRange != Vector2Int.zero)
            globalRotationOffsetRange = _globalRotationOffsetRange;
    }
    public enum BlocArea
    {
        COUNTRY = 0,
        TOWN,
        COUNTOWN,
        TOWNTRY
    }

    public string blocName;
    public BlocArea blockArea;
    public int blocCount, blocLength;
    // offset ranges
    public Vector2Int? blocYRange = null;
    public Vector4? globalOffsetRange = null;
    public Vector2Int? globalRotationOffsetRange = null;

    public ObstacleSpawnable[] obstaclesParams = new ObstacleSpawnable[0];
}
