using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BlocManager : MonoBehaviour
{
    public static BlocManager Instance;

    [Header("Blocs")]

    [SerializeField]
    Bloc.BlocKind startingBlocKind = 0;
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

    // bloc generation
    public float currentBlocMax = 200;

    private void Awake()
    {
        if (Instance == null)
        {
            AddToPool(cansPoolT, ref cansPool);
            AddToPool(bottlesPoolT, ref bottlesPool);
            AddToPool(strawsPoolT, ref strawsPool);
            obstaclePoolsList = new List<List<GameObject>>() { cansPool, bottlesPool, strawsPool };
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
        NewBloc(); // first bloc to be created (after the empty starting bloc)
    }

    public void NewBloc(Bloc.BlocKind blocKind = Bloc.BlocKind.COUNTRY)
    {
        GameObject pooledIn;
        PoolIn(ref blocsPool, Vector3.right * currentBlocMax, out pooledIn);

        Bloc pooledInBloc = pooledIn.GetComponent<Bloc>();
        ObstaclesSpawn(pooledInBloc);

        // incremement X position where to spawn next bloc
        currentBlocMax += pooledInBloc.blocWidth;
    }

    #region procedural

    enum SeriesType
    {
        VERTICAL = 0,
        HORIZONTAL,
        SIDEWAYS,
        MIX
    }
    void ObstaclesSpawn(Bloc spawnedBloc, int lowBound = 43, int highBound = 0, SeriesType seriesType = SeriesType.VERTICAL)
    {
        var regionWidth = 50;
        int regionsCount = (int)(spawnedBloc.blocWidth / regionWidth);
        List<int> yPoss = new List<int>();

        for (int i = highBound; i < lowBound; ++i)
        {
            yPoss.Add(i);
        }

        for (int i = 0; i < regionsCount; ++i)
        {
            // pick an obstacle pool
            List<GameObject> thisObstaclePool = obstaclePoolsList[Random.Range(0, obstaclePoolsList.Count)];

            // set the obstacle type
            Obstacle.ObstacleSpawnType obstacletype = (Obstacle.ObstacleSpawnType)seriesType;
            if ((int)obstacletype > 2)
            {
                obstacletype = (Obstacle.ObstacleSpawnType)Random.Range(0, 3);
            }

            // Get the obstacle height
            int obstacleYoverlapp = 0; // how much the obstacles overlapps in Y
            switch (obstacletype)
            {
                case Obstacle.ObstacleSpawnType.VERTICAL:
                    obstacleYoverlapp = thisObstaclePool[0].GetComponent<Obstacle>().size.y;
                    break;
                case Obstacle.ObstacleSpawnType.HORIZONTAL:
                    obstacleYoverlapp = thisObstaclePool[0].GetComponent<Obstacle>().size.x;
                    break;
                case Obstacle.ObstacleSpawnType.SIDEWAYS:
                    obstacleYoverlapp = Mathf.CeilToInt(Mathf.Sqrt(Mathf.Pow(thisObstaclePool[0].GetComponent<Obstacle>().size.y, 2) / 2)); // close approximation of the sideways height
                    break;
            }

            // create a fixed list of possibilities considering the current list and the obstacle height
            List<int> yFixedPoss = yPoss;
            if (obstacleYoverlapp > 1)
                yFixedPoss = yRealPossibilities(in yPoss, in obstacleYoverlapp);

            // find a random Y position among remaining possibilities
            var randomIndex = Random.Range(0, yFixedPoss.Count);
            int randomY = yFixedPoss[randomIndex];

            ObstacleSpawn(spawnedBloc.ObjsAnchor, i * regionWidth, randomY, thisObstaclePool); // spawn the obstacle

            // remove the coordinates not to use any more in the global range
            yPoss.RemoveRange(yPoss.IndexOf(randomY) - obstacleYoverlapp, obstacleYoverlapp);
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
            var yCheck = _yPoss[i] - 1;
            for (int j = yCheck; j > yCheck - _obstacleH; --j)
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

    void ObstacleSpawn(Transform parentBloc, int xCoord, int yCoord, List<GameObject> obstaclePool, Obstacle.ObstacleSpawnType obstacleType = 0)
    {
        GameObject pooledInObs;
        PoolIn(ref obstaclePool, Vector3.zero, out pooledInObs);
        pooledInObs.transform.parent = parentBloc;

        pooledInObs.transform.localPosition = new Vector2(xCoord, -yCoord);
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

        objectToPoolIn.transform.localPosition = toPosition; // position the object
        objectToPoolIn.SetActive(true); // activate the object
        pooledInObj = objectToPoolIn;
    }
    // objects no longer being seen are pooled out (deactivated)
    public void PoolOut(GameObject toPoolOut)
    {
        toPoolOut.SetActive(false);
    }
    #endregion
}
