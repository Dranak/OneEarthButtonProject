using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloc : MonoBehaviour
{
    public enum BlocKind
    {
        COUNTRY = 0,
        TOWN,
        COUNTOWN,
        TOWNTRY
    }

    public BlocKind blockKind;

    public float blocWidth = 200;

    public Transform ObjsAnchor;
}
