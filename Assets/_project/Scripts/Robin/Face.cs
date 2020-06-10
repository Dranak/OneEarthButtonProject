using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : MonoBehaviour
{
    public FeelType FaceType;

 
}


public enum FeelType
{
    Normal,
    Happy,
    Fear,
    Sweat,
    Die
}