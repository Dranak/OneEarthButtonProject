using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloc
{
    public Bloc(BlocArea _blockArea, int _blocCount, int _blocLength)
    {
        blockArea = _blockArea;
        blocCount = _blocCount;
        blocLength = _blocLength;
    }
    public enum BlocArea
    {
        COUNTRY = 0,
        TOWN,
        COUNTOWN,
        TOWNTRY
    }

    public BlocArea blockArea;
    public int blocCount, blocLength;
}
