using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Bloc
{
    public Bloc(BlocArea _blockArea, int _blocCount, int _blocLength, string _blocName = "", ObstacleSpawnable[] _obstacles = null)
    {
        blockArea = _blockArea;
        blocCount = _blocCount;
        blocLength = _blocLength;
        blocName = _blocName;
        obstaclesParams = _obstacles;
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

    public ObstacleSpawnable[] obstaclesParams = new ObstacleSpawnable[0];
}
