using UnityEngine;

[System.Serializable]
public class Bloc
{
    public Bloc(BlocArea _blockArea, int _blocLength, int _blocCount, string _blocName = "", uint _blocDifficulty = 0, Spawnable[] _spawnables = null, Vector2Int _blocYRange = new Vector2Int(), Vector4 _globalOffsetRange = new Vector4(), Vector2Int _globalRotationOffsetRange = new Vector2Int())
    {
        blockArea = _blockArea;
        blocCount = _blocCount;
        blocLength = _blocLength;
        blocName = _blocName;
        blocDifficulty = _blocDifficulty;
        spawnlablesParams = _spawnables;

        blocYRange = _blocYRange;
        globalOffsetRange = _globalOffsetRange;
        globalRotationOffsetRange = _globalRotationOffsetRange;
    }
    public enum BlocArea
    {
        GLOBAL = 0,
        FOREST,
        COUNTRY
    }

    public string blocName;
    public uint blocDifficulty = 0;
    public BlocArea blockArea;
    public int blocCount, blocLength;
    // offset ranges
    [SerializeField] public Vector2Int blocYRange;
    [SerializeField] public Vector4 globalOffsetRange;
    [SerializeField] public Vector2Int globalRotationOffsetRange;

    [SerializeReference] public Spawnable[] spawnlablesParams = new Spawnable[0];

    public Bloc Clone()
    {
        Bloc other = new Bloc(blockArea, blocLength, blocCount, blocName, blocDifficulty, spawnlablesParams, blocYRange, globalOffsetRange, globalRotationOffsetRange);
        return other;
    }
}
