using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloc
{
    public Bloc(BlocArea _blocKind, int _blocWidth)
    {
        blockKind = _blocKind;
        blocWidth = _blocWidth;
    }
    public enum BlocArea
    {
        COUNTRY = 0,
        TOWN,
        COUNTOWN,
        TOWNTRY
    }

    public BlocArea blockKind;

    public int blocWidth;
}
