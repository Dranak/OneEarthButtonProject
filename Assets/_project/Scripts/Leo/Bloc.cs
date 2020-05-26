using UnityEngine;

[System.Serializable]
public class Bloc
{
    public Bloc(in BlocArea _blockArea, in int _blocCount, in int _blocLength, in string _blocName = "", Spawnable[] _spawnables = null, in Vector2Int _blocYRange = new Vector2Int(), in Vector4 _globalOffsetRange = new Vector4(), in Vector2Int _globalRotationOffsetRange = new Vector2Int())
    {
        blockArea = _blockArea;
        blocCount = _blocCount;
        blocLength = _blocLength;
        blocName = _blocName;
        spawnlablesParams = _spawnables;

        blocYRange = _blocYRange;
        globalOffsetRange = _globalOffsetRange;
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
    [SerializeField] public Vector2Int blocYRange;
    [SerializeField] public Vector4 globalOffsetRange;
    [SerializeField] public Vector2Int globalRotationOffsetRange;

    [SerializeReference] public Spawnable[] spawnlablesParams = new Spawnable[0];
}
