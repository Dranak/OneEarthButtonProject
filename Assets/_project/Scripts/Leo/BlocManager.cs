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
    public List<GameObject> blocsPool;

    [Space]
    [Header("Obstacles")]

    [SerializeField]
    Transform cansPoolT;
    [SerializeField]
    Transform bottlesPoolT, strawsPoolT;
    [SerializeField][HideInInspector]
    List<GameObject> cansPool, bottlesPool, strawsPool;
    List<List<GameObject>> obstaclePoolsList;
    [SerializeField]
    Transform obstaclesAnchor;

    // bloc generation
    public int currentBlocMax;

    private void Awake()
    {
        if (Instance == null)
        {
            AddToPool(cansPoolT, ref cansPool);
            AddToPool(bottlesPoolT, ref bottlesPool);
            AddToPool(strawsPoolT, ref strawsPool);
            obstaclePoolsList = new List<List<GameObject>>() { cansPool,  strawsPool, bottlesPool };
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
    }

    void Start()
    {
        //NewBloc(); // first bloc to be created (after the empty starting bloc)
    }

    public void NewBloc(Bloc.BlocArea blocArea = Bloc.BlocArea.COUNTRY, int blocWidth = 4)
    {
        GameObject pooledIn;
        PoolIn(ref blocsPool, Vector3.right * currentBlocMax, out pooledIn);

        Bloc createdBloc = new Bloc(blocArea, blocWidth); // classic bloc with basic parameters (area determining the environment, and width)
        pooledIn.transform.position += Vector3.right * 6; // 6 is one BG wallpaper width

        ObstaclesSpawn(createdBloc);

        // incremement X position where to spawn next bloc
        currentBlocMax += blocWidth * 6;
    }

    #region procedural

    enum SeriesType
    {
        VERTICAL = 0,
        HORIZONTAL,
        SIDEWAYS,
        MIX
    }
    void ObstaclesSpawn(Bloc generatedBloc, int lowBound = 9, int highBound = 0, SeriesType seriesType = SeriesType.MIX)
    {
        int regionsCount = generatedBloc.blocWidth;
        List<int> yPoss = new List<int>();

        for (int i = highBound; i < lowBound; ++i)
        {
            yPoss.Add(i);
        }

        for (int i = 0; i < regionsCount; ++i)
        {
            // pick an obstacle pool
            List<GameObject> thisObstaclePool = obstaclePoolsList[Random.Range(0, 1/*obstaclePoolsList.Count*/)];

            // set the obstacle type
            Obstacle.ObstacleSpawnType obstacletype = (Obstacle.ObstacleSpawnType)seriesType;
            if ((int)obstacletype > 2)
            {
                obstacletype = (Obstacle.ObstacleSpawnType)Random.Range(0, 3); //3 for sideways
            }

            // Get the obstacle height
            int obstacleYoverlapp = 0; // how much the obstacles overlapps in Y
            Vector2 obstacleOverlapp = thisObstaclePool[0].GetComponent<Obstacle>().size;

            switch (obstacletype)
            {
                case Obstacle.ObstacleSpawnType.VERTICAL:
                    obstacleYoverlapp = (int)obstacleOverlapp.y;
                    break;
                case Obstacle.ObstacleSpawnType.HORIZONTAL:
                    obstacleYoverlapp = (int)obstacleOverlapp.x;
                    break;
                case Obstacle.ObstacleSpawnType.SIDEWAYS:
                    var sideWaysH = Mathf.Sqrt(Mathf.Pow(thisObstaclePool[0].GetComponent<Obstacle>().size.y, 2) / 2);
                    obstacleOverlapp = new Vector2(obstacleOverlapp.x, sideWaysH);
                    obstacleYoverlapp = Mathf.CeilToInt(sideWaysH); // close approximation of the sideways height
                    break;
            }

            // create a fixed list of possibilities considering the current list and the obstacle height
            List<int> yFixedPoss = yPoss;
            if (obstacleYoverlapp > 1)
                yFixedPoss = yRealPossibilities(in yPoss, in obstacleYoverlapp);

            // find a random Y position among remaining possibilities
            var randomIndex = Random.Range(0, yFixedPoss.Count);
            int randomY = yFixedPoss[randomIndex];

            ObstacleSpawn(obstaclesAnchor, currentBlocMax + 3 /*half a paperW width*/ + i * generatedBloc.blocWidth, randomY, thisObstaclePool, obstacletype, obstacleOverlapp); // spawn the obstacle

            // remove the coordinates not to use any more in the global range
            var indexOfRy = yPoss.IndexOf(randomY - obstacleYoverlapp + 1);
            yPoss.RemoveRange(indexOfRy, obstacleYoverlapp);
            // remove the Y coordinates already used (where the obstacle overlaps)
            /*for (int j = randomY - obstacleYoverlapp; j < randomY; ++j)
            {
                if (yPoss.coun)
            }*/
        }
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
    /*bool canObsFitInBlock(int obsY, int overlapHeight, in List<int> _yPoss)
    {
        for (int i = obsY; i < obsY + overlapHeight; ++i)
        {
            if (!_yPoss.Contains(i))
                return false;
        }
        return true;
    }*/

    void ObstacleSpawn(Transform parentBloc, int xCoord, int yCoord, List<GameObject> obstaclePool, Obstacle.ObstacleSpawnType obstacleType, Vector2 overlapSize)
    {
        GameObject obstacleSpawned;
        PoolIn(ref obstaclePool, Vector3.zero, out obstacleSpawned);
        obstacleSpawned.transform.parent = parentBloc;

        obstacleSpawned.transform.localPosition = new Vector2(xCoord, -yCoord);
        var obstacleBody = obstacleSpawned.GetComponent<Obstacle>().objectT;

        switch (obstacleType)
        {
            case Obstacle.ObstacleSpawnType.HORIZONTAL:
                obstacleBody.transform.localEulerAngles = Vector3.forward * -90;
                obstacleBody.transform.localPosition = Vector2.up * overlapSize.x;
                break;
            case Obstacle.ObstacleSpawnType.SIDEWAYS:
                var backOrForth = Random.Range(0, 2) * 2 - 1;
                obstacleBody.transform.localEulerAngles = Vector3.back * 45 * backOrForth;
                obstacleBody.transform.localPosition = Vector2.up * 0.5f * overlapSize.x;
                if (backOrForth < 0)
                    obstacleBody.transform.localPosition += Vector3.right * overlapSize.y;
                break;
        }

        //return obstacleSpawned;
    }
    #endregion

    #region pool
    // objects to appear next are pooled in (activated)
    void PoolIn(ref List<GameObject> pool, Vector3 toPosition, out GameObject pooledInObj)
    {
        var objectToPoolIn = pool.FirstOrDefault(i => !i.activeInHierarchy); // finds the first inactive object in the pool

        //error (pool full)
        if (objectToPoolIn == null)
            Debug.LogError("No more remaining to pool in");

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
