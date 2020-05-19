using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollectibleSpawnable : Spawnable
{
    [HideInInspector] public Vector2 anchorBodyPosition;
    public int PointGain;
    public bool IsEggShell;
}
