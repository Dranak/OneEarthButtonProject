using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class BlocManager : MonoBehaviour
{
    public static BlocManager Instance;

    [Header("Blocs")]

    [SerializeField]
    Bloc.BlocArea startingBlocKind = 0;
    [SerializeField]
    Bloc[] countryBG2, townBG2, transitionsBG;//...
    public List<GameObject> wpPool;
    [SerializeField]
    GameObject blocPoolerPrefab;

    [Space]
    [Header("Obstacles")]

    [SerializeField]
    Transform cansPoolT;
    [SerializeField]
    Transform bottlesPoolT, strawsPoolT;
    [SerializeField][HideInInspector]
    List<GameObject> cansPool, bottlesPool, strawsPool;
    Dictionary<Vector2Int, List<GameObject>> obstaclePoolsDic = new Dictionary<Vector2Int, List<GameObject>>();
    [SerializeField]
    Transform obstaclesAnchor;

    // bloc generation
    public int currentBlocMax;
    int currentWPMax;

    private void Awake()
    {
        if (Instance == null)
        {
            AddToPool(cansPoolT, ref cansPool);
            AddToPool(bottlesPoolT, ref bottlesPool);
            //AddToPool(strawsPoolT, ref strawsPool);

            currentWPMax = currentBlocMax;
            Instance = this;
        }
        else
            Destroy(this);
    }

    void AddToPool(Transform parent, ref List<GameObject> pool, bool letActive = false)
    {
        foreach (Transform child in parent)
        {
            pool.Add(child.gameObject);
            if (!letActive)
                child.gameObject.SetActive(false);
        }

        Vector2Int obstaclesSize = parent.GetChild(0).GetComponent<Obstacle>().size;
        obstaclePoolsDic.Add(obstaclesSize, pool);
    }

    void Start()
    {
        var firstWpRightBoundPos = wpPool[0].transform.position.x + 3;
        if (firstWpRightBoundPos < GameManager.Instance.camera.transform.position.x - GameManager.Instance.cameraHalfWidth)
        {
            wpPool[0].SetActive(false);
        }
        NewBloc(); // first bloc to be created (after the empty starting bloc)
    }

    public void NewBloc(Bloc.BlocArea blocArea = Bloc.BlocArea.COUNTRY, int blocCount = 4, int blockLength = 16)
    {
        Bloc createdBloc = new Bloc(blocArea, blocCount, blockLength); // classic bloc with basic parameters (area determining the environment, and width)

        ObstaclesSpawn(createdBloc, out blockLength);

        // incremement X position where to spawn next bloc
        currentBlocMax += blockLength + 1;

        // spawn pooler trigger that will spawn another obstacle
        Instantiate(blocPoolerPrefab, new Vector2(currentBlocMax, 0), Quaternion.identity);
    }

    #region procedural

    enum SeriesType
    {
        VERTICAL = 0,
        HORIZONTAL,
        SIDEWAYS,
        MIX
    }
    void ObstaclesSpawn(Bloc generatedBloc, out int _blockLength, int lowBound = 9, int highBound = 0, SeriesType seriesType = SeriesType.MIX)
    {
        int regionsCount = generatedBloc.blocCount;
        List<int> yPoss = new List<int>();
        int currentBlocLength = 0;

        for (int i = highBound; i < lowBound; ++i)
        {
            yPoss.Add(i);
        }

        for (int i = 0; i < regionsCount; ++i)
        {
            // get current largest Y gap among the series of obstacle (BLOC)
            int currentLY = LargerRemainingYGap(in yPoss);

            // set the obstacle type
            Obstacle.ObstacleSpawnType obstacletype = (Obstacle.ObstacleSpawnType)seriesType;
            if ((int)obstacletype > 2)
            {
                obstacletype = (Obstacle.ObstacleSpawnType)Random.Range(0, 3); //3 for sideways
            }

            // pick an obstacle pool based on the 
            List<List<GameObject>> possObstaclePools = new List<List<GameObject>>();
            switch (obstacletype)
            {
                case Obstacle.ObstacleSpawnType.VERTICAL:
                    possObstaclePools = obstaclePoolsDic.Where(x => x.Key.y <= currentLY).Select(x => x.Value).ToList();
                    break;
                case Obstacle.ObstacleSpawnType.HORIZONTAL:
                    possObstaclePools = obstaclePoolsDic.Where(x => x.Key.x <= currentLY).Select(x => x.Value).ToList();
                    break;
                case Obstacle.ObstacleSpawnType.SIDEWAYS:
                    possObstaclePools = obstaclePoolsDic.Where(x => SidewaysH(x.Key) <= currentLY).Select(x => x.Value).ToList();
                    break;
            }
            List<GameObject> thisObstaclePool = possObstaclePools[Random.Range(0, possObstaclePools.Count)];

            // Spawn Obstacle
            GameObject obstacleSpawned;
            PoolIn(ref thisObstaclePool, Vector3.zero, out obstacleSpawned, obstaclesAnchor);

            // get how much the obstacle is taking space (X and Y)
            Vector2 obstacleOverlapp = obstacleSpawned.GetComponent<Obstacle>().size;
            switch (obstacletype)
            {
                case Obstacle.ObstacleSpawnType.HORIZONTAL:
                    obstacleOverlapp = new Vector2(obstacleOverlapp.y, obstacleOverlapp.x);
                    break;
                case Obstacle.ObstacleSpawnType.SIDEWAYS:
                    var sideWaysH = SidewaysH(in obstacleOverlapp);
                    obstacleOverlapp = new Vector2(sideWaysH, sideWaysH);
                    break;
            }
            int obstacleXoverlapp = Mathf.CeilToInt(obstacleOverlapp.x);
            int obstacleYoverlapp = Mathf.CeilToInt(obstacleOverlapp.y);

            // create a fixed list of possibilities considering the current list and the obstacle height
            List<int> yFixedPoss = yPoss;
            if (obstacleYoverlapp > 1)
                yFixedPoss = yRealPossibilities(in yPoss, in obstacleYoverlapp);

            // find a random Y position among remaining possibilities
            var randomIndex = Random.Range(0, yFixedPoss.Count);
            int randomY = yFixedPoss[randomIndex];

            // Placing the obstacle
            ObstaclePlacing(in obstacleSpawned, currentBlocMax + 3 /*half a paperW width*/ + i * generatedBloc.blocCount, randomY); // moves/rotates the obstacle
            currentBlocLength += obstacleXoverlapp;

            // remove the coordinates not to use any more in the global range
            var indexOfRy = yPoss.IndexOf(randomY - obstacleYoverlapp + 1);
            yPoss.RemoveRange(indexOfRy, obstacleYoverlapp);
        }

        _blockLength = currentBlocLength;
    }

    float SidewaysH(in Vector2 obstacleSize)
    {
        var bodyH = HypotenusHalfAntecedent(obstacleSize.y);
        var endsH = HypotenusHalfAntecedent(obstacleSize.x);
        return bodyH + endsH;
    }
    float HypotenusHalfAntecedent(in float hypoLength)
    {
        return Mathf.Sqrt(Mathf.Pow(hypoLength, 2) / 2);
    }

    int LargerRemainingYGap(in List<int> _yPoss)
    {
        int largestGap = 0;

        for (int i = 0; i < _yPoss.Count; ++i)
        {
            int inspectedY = _yPoss[i];
            int thisGap = 1;
            while ((i + 1) < _yPoss.Count && _yPoss[i + 1] == inspectedY + 1)
            {
                ++thisGap;
                inspectedY = _yPoss[++i];
            }
            if (thisGap > largestGap) largestGap = thisGap; // final gap from that cell size (set as largest gap or not)
        }

        return largestGap;
    }

    List<int> yRealPossibilities(in List<int> _yPoss, in int _obstacleH)
    {
        var possCopy = new List<int>(_yPoss);
        List<int> toRemove = new List<int>();

        for (int i = 0; i < _yPoss.Count; ++i)
        {
            var yCheck = _yPoss[i] - _obstacleH;
            for (int j = yCheck; j < yCheck + _obstacleH; ++j)
            {
                if (!_yPoss.Contains(j)) // obstacle can't fit at this Y position
                {
                    toRemove.Add(_yPoss[i]);
                    break;
                }
            }
        }

        foreach(int y in toRemove)
        {
            possCopy.Remove(y);
        }

        return possCopy;
    }

    void ObstaclePlacing(in GameObject obstacleToPlace, in int xCoord, in int yCoord)
    {
        obstacleToPlace.transform.localPosition = new Vector2(xCoord, -yCoord);
        var obstacleObject = obstacleToPlace.GetComponent<Obstacle>();
        var obstacleBody = obstacleObject.objectT;

        obstacleObject.boxSize = obstacleObject.size;

        Vector2 obstacleOffset = Vector2.zero;
        switch (obstacleObject.obstacleSpawnType)
        {
            case Obstacle.ObstacleSpawnType.HORIZONTAL:
                obstacleOffset = Vector2.up * obstacleObject.size.y;
                obstacleBody.transform.localEulerAngles = Vector3.forward * -90;
                break;
            case Obstacle.ObstacleSpawnType.SIDEWAYS:
                var backOrForth = Random.Range(0, 2) * 2 - 1;
                obstacleBody.transform.localEulerAngles = Vector3.back * 45 * backOrForth;
                if (backOrForth < 0)
                    obstacleOffset = Vector2.right * obstacleObject.size.y;
                else
                    obstacleOffset = Vector2.up * HypotenusHalfAntecedent(obstacleObject.size.x);
                break;
        }
        obstacleBody.transform.localPosition += (Vector3)obstacleOffset;
    }

    // Spawns WPapers
    public void NewWP(Transform toUnpool)
    {
        PoolOut(toUnpool.parent.gameObject);
        GameObject pooledIn;
        currentWPMax += 6;
        PoolIn(ref wpPool, Vector3.right * currentWPMax, out pooledIn);
    }
    #endregion

    #region pool
    // objects to appear next are pooled in (activated)
    public void PoolIn(ref List<GameObject> pool, Vector3 toPosition, out GameObject pooledInObj, Transform parent = null)
    {
        var objectToPoolIn = pool.FirstOrDefault(i => !i.activeInHierarchy); // finds the first inactive object in the pool

        //error (pool full)
        if (objectToPoolIn == null)
            Debug.LogError("No more remaining to pool in");

        objectToPoolIn.transform.parent = parent;
        objectToPoolIn.transform.position = toPosition; // position the object
        objectToPoolIn.SetActive(true); // activate the object
        pooledInObj = objectToPoolIn;
    }
    // objects no longer being seen are pooled out (deactivated)
    public void PoolOut(GameObject toPoolOut)
    {
        toPoolOut.transform.rotation = Quaternion.identity;
        toPoolOut.SetActive(false);
    }
    #endregion
}
