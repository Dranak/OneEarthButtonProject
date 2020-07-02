using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinData", menuName = "SkinData")]
public class SkinData : ScriptableObject
{
    public SkinType SkinType;
    public Sprite HeadSprite;
    public Sprite TailSprite;
    public Material BodyMaterial;
    public List<Face> Faces;
}


public enum SkinType
{
    Normal,
    Anecics,
    Bipaliul_Kewense,
    Ancestor,
    Megascolides_australis,
    Lineus_longissimus,
    Myth_reality,
    Worms


}