using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Face", menuName = "Face")]
public class Face : ScriptableObject
{
    public FeelType FaceType;
    public Sprite Eyes;
    public Sprite Pupil;
    public Sprite Mouth;
       
}


public enum FeelType
{
    Normal,
    Happy,
    Fear,
    Sweat,
    Die
}