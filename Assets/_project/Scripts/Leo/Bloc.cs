using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloc
{
    public Bloc(BlocArea _blocKind, int _blocCount, int _blocLength)
    {
        blockKind = _blocKind;
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

    public BlocArea blockKind;

    public int blocCount, blocLength;
}
