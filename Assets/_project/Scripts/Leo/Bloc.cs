using UnityEngine;

[System.Serializable]
public class Bloc
{
    public Bloc(BlocArea _blockArea, int _blocCount, int _blocLength, string _blocName = "", Spawnable[] _spawnables = null, Vector2Int _blocYRange = new Vector2Int(), Vector4 _globalOffsetRange = new Vector4(), Vector2Int _globalRotationOffsetRange = new Vector2Int())
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

    public Bloc Clone()
    {
        Bloc other = new Bloc(blockArea, blocCount, blocLength, blocName, spawnlablesParams, blocYRange, globalOffsetRange, globalRotationOffsetRange);
        //(Bloc)this.MemberwiseClone();
        /*other.blocName = String.Copy(blocName);
        other.blockArea = (BlocArea)((int)blockArea);
        other.blocCount = blocCount;
        other.blocLength = blocLength;
        other.blocYRange = blocYRange;
        other.globalOffsetRange = globalOffsetRange;
        other.globalRotationOffsetRange = globalRotationOffsetRange;*/
        return other;
    }
}
