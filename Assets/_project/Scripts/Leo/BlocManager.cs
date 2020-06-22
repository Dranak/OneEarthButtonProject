﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BlocManager : MonoBehaviour
{
    public static BlocManager Instance;

    [Header("Blocs")]

    [SerializeField] Bloc.BlocArea startingBlocKind = 0;
    [SerializeField] Transform wpDarkT, wpLightT;
    [SerializeField, HideInInspector] List<GameObject> wpPoolDark, wpPoolLight;
    [SerializeField] GameObject blocPoolerPrefab;

    [Space]
    [Header("Obstacles")]

    [SerializeField] PoolersCreator spawnablesPools; // spawnables pools parent
    [SerializeField, HideInInspector] List<GameObject> cansPool, bottlesPool; // used only for the dictionary
    Dictionary<Vector2, List<GameObject>> obstaclePoolsDic = new Dictionary<Vector2, List<GameObject>>(); // only for total randomizer
    [SerializeField] Transform spawnablesAnchor;

    public Transform backTreesAnchor, backRocksAnchor, backBushesAnchor;
    [SerializeField, HideInInspector] List<GameObject> backTreesPool, backRocksPool, backBushesPool;

    // bloc generation
    public int startingBlocMin { get; private set; } = 33;
    int currentBlocMax, currentBlocMin;
    int currentWPMax;
    List<Bloc> allBlocs;
    List<List<Bloc>> allBlocsRanked;

    private void Awake()
    {
        if (Instance == null)
        {
            AddToPool(spawnablesPools.transform.GetChild(0), ref cansPool);
            AddToPool(spawnablesPools.transform.GetChild(1), ref bottlesPool);
            //AddToPool(strawsPoolT, ref strawsPool);

            AddToPool(wpDarkT, ref wpPoolDark, true, false);
            AddToPool(wpLightT, ref wpPoolLight, false, false);
            currentWPMax = (int)wpPoolDark[wpPoolDark.Count - 1].transform.position.x; // wallpapers are centered (and 6 int large)
            currentBlocMax = startingBlocMin;
            allBlocs = spawnablesPools.selectedBlocsScriptable.storedBlocs;
            RankBlocs();
            // add trees to pool
            AddToPool(backTreesAnchor, ref backTreesPool, false, false);
            AddToPool(spawnablesAnchor, ref backTreesPool, true, false, "Tree"); // back objects in the Spawnables on start are also part of the pool
            // add rocks to pool
            AddToPool(backRocksAnchor, ref backRocksPool, false, false);
            AddToPool(spawnablesAnchor, ref backRocksPool, true, false, "Rock"); // back objects in the Spawnables on start are also part of the pool
            // add bushed to pool
            AddToPool(backBushesAnchor, ref backBushesPool, false, false);
            AddToPool(spawnablesAnchor, ref backBushesPool, true, false, "Bush"); // back objects in the Spawnables on start are also part of the pool

            Instance = this;
        }
        else
            Destroy(this);
    }

    void AddToPool(Transform parent, ref List<GameObject> pool, bool letActive = false, bool isObstacle = true, string nameCheck = "")
    {
        foreach (Transform child in parent)
        {
            if (child.name.Contains(nameCheck))
            {
                pool.Add(child.gameObject);
                child.gameObject.SetActive(letActive);
            }
        }
        if (isObstacle)
        {
            Vector2 obstaclesSize = parent.GetChild(0).GetComponent<Obstacle>().Size;
            obstaclePoolsDic.Add(obstaclesSize, pool);
        }
    }

    void RankBlocs()
    {
        allBlocsRanked = new List<List<Bloc>>();
        foreach(Bloc bloc in allBlocs)
        {
            var blocDiff = bloc.blocDifficulty;
            if (allBlocsRanked.Count <= blocDiff)
            {
                for (int i = (int)blocDiff - allBlocsRanked.Count; i >= 0; --i)
                    allBlocsRanked.Add(new List<Bloc>());
            }
            allBlocsRanked[(int)blocDiff].Add(bloc);
        }
    }

    void Start()
    {
        WallpaperChoice(0);
        ChooseBloc(0);
        GameManager.Instance.Player.playingBlocName = randomBloc.blocName;
        //NewBloc(); // create and siplay a whole bloc at once // LEGACY
        //NewRandomBloc(); // first bloc to be created (after the empty starting bloc) // LEGACY
    }

#region procedural

    public Bloc randomBloc { get; private set; }
    float blocRandomY;
    List<Spawnable> randomizedSortedBloc = null;
    int currentBlocDiff = 0;

    List<GameObject> wpPool;
    int currentBlocAreaIdx = 0;

    public void ChooseBloc(int prespacing = 6) // spacing is 6 by default
    {
        currentBlocMax += prespacing; // add the spacing before this bloc
        currentBlocMin = currentBlocMax; // set bloc min // bloc min is the bloc max without the next bloc width

        var previousBlocDiff = currentBlocDiff;
        currentBlocDiff = Mathf.FloorToInt((GameManager.Instance.Player.Score / 500) % (allBlocsRanked.Count));
        if (previousBlocDiff != currentBlocDiff && currentBlocDiff == 0)
        {
            currentBlocAreaIdx = (currentBlocAreaIdx + 1) % 2;
            WallpaperChoice(currentBlocAreaIdx);
        }
        var sortedBlocs = allBlocsRanked[currentBlocDiff];
        randomBloc = sortedBlocs[Random.Range(0, sortedBlocs.Count)].Clone(); // cloning the bloc used, not to change the original

        currentBlocMax += randomBloc.blocLength; // add this bloc size to the bloc max, not working for X random
        randomizedSortedBloc = new List<Spawnable>();
        Debug.Log("Choosen Bloc is " + randomBloc.blocName + ", POS : min = " + currentBlocMin + ", max = " + currentBlocMax);

        // random Y pos for the bloc
        blocRandomY = 0;
        if (randomBloc.blocYRange != Vector2Int.zero) blocRandomY = Random.Range(((Vector2)randomBloc.blocYRange).x, ((Vector2)randomBloc.blocYRange).y);

        foreach (Spawnable spawnable in randomBloc.spawnlablesParams)
        {
            // spawnable self random position
            if (spawnable.OffsetXRange != Vector2Int.zero)
                spawnable.BlocPosition += new Vector2(Random.Range(spawnable.OffsetXRange.x, spawnable.OffsetXRange.y), Random.Range(spawnable.OffsetYRange.x, spawnable.OffsetYRange.y));
            // add to list to be sorted
            randomizedSortedBloc.Add(spawnable);
        }

        // Bloc-wise offsets (position)
        Vector4 globalOffset = randomBloc.globalOffsetRange;
        if (globalOffset != Vector4.zero)
        {
            foreach (ObstacleSpawnable obstacle in randomizedSortedBloc)
            {
                obstacle.BlocPosition += new Vector2(Random.Range((int)(globalOffset).x, (int)(globalOffset).y), Random.Range((int)(globalOffset).z, (int)(globalOffset).w));
            }
        }

        // sort the list by X pos
        randomizedSortedBloc = randomizedSortedBloc.OrderBy(x => x.BlocPosition.x).ToList();

        // spawn pooler trigger to reset the egg count?
        Instantiate(blocPoolerPrefab, new Vector2(currentBlocMax, 0), Quaternion.identity);
    }
    void WallpaperChoice(int choiceIndex)
    {
        if (choiceIndex == 0)
        {
            wpPool = wpPoolDark;
        }
        else if (choiceIndex == 1)
        {
            wpPool = wpPoolLight;
        }
    }

    void SpawnablesSpawn(in float posX) // per WP spawnables spawn
    {
        int rangeToRemove = 0;
        List<SpawnableObject> blocSpList = new List<SpawnableObject>();
        float blocDisplacement = 0;

        foreach (Spawnable spawnable in randomizedSortedBloc)
        {
            if (spawnable.BlocPosition.x + currentBlocMin >= posX)
            { // stop and clear from list when no more spawnables in the spawn range
                randomizedSortedBloc.RemoveRange(0, rangeToRemove); // clearing all spawnables to be spawned from list (performances)
                break;
            }

            ++rangeToRemove;

            GameObject spawnableSpawned;
            List<GameObject> thisSpawnablesPool = spawnablesPools.spawnablePoolsObjects[spawnable.SpawnablePrefabIndex].objectPool;
            PoolIn(ref thisSpawnablesPool, new Vector2(currentBlocMin, blocRandomY), out spawnableSpawned, spawnablesAnchor); // pool in the first inactive spawnable from the pool
            var spawnableObj = spawnableSpawned.GetComponent<SpawnableObject>();

            // prevent spawnables from going out of Y bounds (0; -9)
            spawnable.BlocPosition = new Vector2(spawnable.BlocPosition.x, Mathf.Clamp(spawnable.BlocPosition.y, -9 - blocRandomY, 0 - blocRandomY));

            spawnableObj.SetSpawnable(spawnable);
            SpawnablePlacing(spawnableObj); // adjust position
            blocSpList.Add(spawnableObj);

            // increment bloc max if object goes above it
            var spawnableObjWidth = spawnableObj.Size.x;
            if (spawnableObj is Obstacle && (spawnableObj.Size.x != spawnableObj.Size.y))
                spawnableObjWidth = (spawnableObj.GetSpawnable() as ObstacleSpawnable).BoundsSize.x;
            var spawnableDisplacement = spawnableObjWidth + spawnable.BlocPosition.x - randomBloc.blocLength;
            if (spawnableDisplacement > blocDisplacement)
            {
                blocDisplacement = spawnableDisplacement;
                currentBlocMax += Mathf.CeilToInt(spawnableDisplacement);
                Debug.Log("Choosed Bloc Max override : " + currentBlocMax);
            }
        }

        // global random rotation (only for long obstacles)
        Vector2Int globalRotOffset = randomBloc.globalRotationOffsetRange;
        if (globalRotOffset != Vector2Int.zero)
        {
            foreach (Obstacle obstacle in blocSpList)
            {
                if (obstacle.Size.x != obstacle.Size.y)
                {
                    int rotOffsetMin = globalRotOffset.x;
                    if (rotOffsetMin > 0) rotOffsetMin = 0;

                    obstacle.objectBody.Rotate(Vector3.back, (Random.Range((globalRotOffset).x - rotOffsetMin, (globalRotOffset).y - rotOffsetMin) + rotOffsetMin) * 22.5f);
                    // recalculate and assign body offset and bounds size
                    var fixedColliderBounds = obstacle.objectBody.gameObject.GetBoxColliderFixedBounds();
                    obstacle.objectBody.localPosition += obstacle.transform.position - fixedColliderBounds.min;
                }
            }
        }

        if (posX >= currentBlocMax) // choose the next bloc if we're at the bloc end (latest wp pooling)
        {
            ChooseBloc();
        }
    }

#region LEGACY
    public void NewBloc(int obstaclesXGap = 3)
    {
        randomBloc = allBlocs[Random.Range(0, allBlocs.Count)];

        SpawnablesSpawn(randomBloc);

        // incremement X position where to spawn next bloc
        currentBlocMax += randomBloc.blocLength;

        // spawn pooler trigger that will spawn another obstacle
        Instantiate(blocPoolerPrefab, new Vector2(currentBlocMax, 0), Quaternion.identity);

        currentBlocMax += obstaclesXGap; // blocs gap ? AFTER BLOCK
    }
    public void NewRandomBloc(Bloc.BlocArea blocArea = Bloc.BlocArea.COUNTRY, int blocCount = 3, int obstaclesXGap = 3, int blockLength = 0)
    {
        Bloc createdBloc = new Bloc(blocArea, blocCount, blockLength); // classic bloc with basic parameters (area determining the environment, and width)

        var gapRandomization = 0.25f; // gap randomization percentage -/+ (ceil rounded)
        ObstaclesSpawn(createdBloc, out blockLength, in obstaclesXGap, in gapRandomization);

        // incremement X position where to spawn next bloc
        currentBlocMax += blockLength;

        // spawn pooler trigger that will spawn another obstacle
        Instantiate(blocPoolerPrefab, new Vector2(currentBlocMax, 0), Quaternion.identity);

        currentBlocMax += obstaclesXGap * 2; // blocs gap ? AFTER BLOCK
    }

    enum SeriesType
    {
        HORIZONTAL = 0,
        SIDEWAYS,
        VERTICAL,
        MIX
    }
    void SpawnablesSpawn(in Bloc chosenBloc)
    {
        Vector2Int blocYRange = chosenBloc.blocYRange;
        float randomY = 0;
        if (blocYRange != Vector2Int.zero) randomY = Random.Range(((Vector2)blocYRange).x, ((Vector2)blocYRange).y);
        List<SpawnableObject> blocSpList = new List<SpawnableObject>();
        foreach (Spawnable spawnable in chosenBloc.spawnlablesParams)
        {
            // Spawn Obstacle
            GameObject spawnableSpawned;
            List<GameObject> thisSpawnablesPool = spawnablesPools.spawnablePoolsObjects[spawnable.SpawnablePrefabIndex].objectPool; //spawnablesPools.transform.GetChild(spawnable.SpawnablePrefabIndex).Cast<Transform>().Select(w => w.gameObject).ToList(); // get all spawnables (activated or not)
            PoolIn(ref thisSpawnablesPool, new Vector2(currentBlocMax, randomY), out spawnableSpawned, spawnablesAnchor); // pool in the first inactive spawnable from the pool
            var spawnableObj = spawnableSpawned.GetComponent<SpawnableObject>();
            spawnableObj.SetSpawnable(spawnable);
            SpawnablePlacing(spawnableObj); // adjust position
            blocSpList.Add(spawnableObj);
        }
        // Bloc-wise offsets
        Vector4 globalOffset = chosenBloc.globalOffsetRange;
        Vector2Int globalRotOffset = chosenBloc.globalRotationOffsetRange;
        if (globalOffset != Vector4.zero)
        {
            foreach (SpawnableObject spawnable in blocSpList)
            {
                spawnable.transform.localPosition += (Vector3)new Vector2(Random.Range((int)(globalOffset).x, (int)(globalOffset).y), Random.Range((int)(globalOffset).z, (int)(globalOffset).w));
            }
        }
        // global random rotation
        if (globalRotOffset != Vector2Int.zero)
        {
            foreach (SpawnableObject spawnable in blocSpList)
            {
                if (spawnable is Obstacle && spawnable.Size.x != spawnable.Size.y) // only long obstacles need to be affected by random rotation
                {
                    int rotOffsetMin = globalRotOffset.x;
                    if (rotOffsetMin > 0) rotOffsetMin = 0;
                    spawnable.objectBody.Rotate(Vector3.back, (Random.Range((globalRotOffset).x - rotOffsetMin, (globalRotOffset).y - rotOffsetMin) + rotOffsetMin) * 22.5f);
                    spawnable.objectBody.localPosition += spawnable.transform.position - spawnable.objectBody.gameObject.GetBoxColliderFixedBounds().min; // recalculate and assign body offset
                }
            }
        }
    }
    void ObstaclesSpawn(Bloc generatedBloc, out int _blockLength, in int obstaclesXGap, in float XGapRandomization, int lowBound = 9, int highBound = 0, SeriesType seriesType = SeriesType.MIX)
    {
        int regionsCount = generatedBloc.blocCount;
        List<int> yPoss = new List<int>();
        int currentBlocLength = 0;

        // define random gap possibilities
        int maxGapChange = Mathf.CeilToInt(XGapRandomization * obstaclesXGap);
        List<int> possGaps = new List<int>();
        for (int i = -maxGapChange; i <= maxGapChange; ++i)
        {
            possGaps.Add(obstaclesXGap + i);
        }

        // define all Y position possibilities (no exception)
        for (int i = highBound; i <= lowBound; ++i)
        {
            yPoss.Add(i);
        }

        for (int i = 0; i < regionsCount; ++i)
        {
            // get current largest Y gap among the series of obstacle (BLOC)
            int currentLY = LargestRemainingGap(in yPoss);

            // set the obstacle type
            Obstacle.ObstacleRotation obstacletype = (Obstacle.ObstacleRotation)seriesType; // obstacle type for this series is constant

            // listing each rotation type possibilities for future use
            List<List<GameObject>>[] allPossLists = new List<List<GameObject>>[3];
            if ((int)obstacletype > 2) // random obstacle type series
            {
                // removing orientations that wouldn't fit either way
                var orientationPoss = 0;
                var horizontalPoss = obstaclePoolsDic.Where(x => x.Key.x <= currentLY).Select(x => x.Value).ToList();

                if (horizontalPoss.Count == 0) // no horizontal possibility means no possibility
                {
                    Debug.Log("Couldn't spawn obstacle following the no-y-overlapp rule. Stopping bloc generation.");
                    break;
                }
                else  // horizontal poss at least
                {
                    allPossLists[0] = horizontalPoss;
                    ++orientationPoss;
                    var sidewayPoss = obstaclePoolsDic.Where(x => SidewaysH(x.Key) <= currentLY).Select(x => x.Value).ToList();
                    if (sidewayPoss.Count != 0) // sideway poss ?
                    {
                        allPossLists[1] = sidewayPoss;
                        ++orientationPoss;
                        var vertPoss = obstaclePoolsDic.Where(x => x.Key.y <= currentLY).Select(x => x.Value).ToList();
                        if (vertPoss.Count != 0) // vert poss ?
                        {
                            allPossLists[2] = vertPoss;
                            ++orientationPoss;
                        }
                    }
                }

                obstacletype = (Obstacle.ObstacleRotation)Random.Range(0, orientationPoss); //3 for sideways
            }

            // pick an obstacle pool based on the size the obstacle would take (either rotation)
            List<List<GameObject>> possObstaclePools = new List<List<GameObject>>();
            switch (obstacletype)
            {
                case Obstacle.ObstacleRotation.HORIZONTAL:
                    possObstaclePools = allPossLists[0];
                    break;
                case Obstacle.ObstacleRotation.SIDEWAYS:
                    possObstaclePools = allPossLists[1];
                    break;
                case Obstacle.ObstacleRotation.VERTICAL:
                    possObstaclePools = allPossLists[2];
                    break;
            }
            List<GameObject> thisObstaclePool = possObstaclePools[Random.Range(0, possObstaclePools.Count)];

            // Spawn Obstacle
            GameObject obstacleSpawned;
            PoolIn(ref thisObstaclePool, Vector3.zero, out obstacleSpawned, spawnablesAnchor);
            Obstacle obstacleObj = obstacleSpawned.GetComponent<Obstacle>();
            obstacleObj.obstacleRotation = obstacletype; // set the obstacle type on the spawned obstacle object

            // get how much the obstacle is taking space (X and Y)
            Vector2 obstacleOverlapp = obstacleSpawned.GetComponent<Obstacle>().Size;
            switch (obstacletype)
            {
                case Obstacle.ObstacleRotation.HORIZONTAL:
                    obstacleOverlapp = new Vector2(obstacleOverlapp.y, obstacleOverlapp.x);
                    break;
                case Obstacle.ObstacleRotation.SIDEWAYS:
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

            // find a random Y position among remaining possibilities (if possible)
            if (yFixedPoss.Count == 0)
            {
                Debug.Log("Couldn't spawn desired number of obstacles following the no-y-overlapp rule. Stopping bloc generation.");
                break;
            }
            else
            {
                var randomIndex = Random.Range(0, yFixedPoss.Count);
                int randomY = yFixedPoss[randomIndex];

                // Placing the obstacle
                ObstaclePlacing(in obstacleSpawned, currentBlocMax + currentBlocLength, randomY); // moves/rotates the obstacle
                currentBlocLength += obstacleXoverlapp;
                if (i < regionsCount - 1)
                {
                    var thisGap = possGaps[Random.Range(0, possGaps.Count)];
                    currentBlocLength += thisGap;
                }

                // remove the coordinates not to use any more in the global range
                var indexOfRy = yPoss.IndexOf(randomY - obstacleYoverlapp + 1);
                yPoss.RemoveRange(indexOfRy, obstacleYoverlapp);
            }
        }

        _blockLength = currentBlocLength;
    }
    void ObstaclePlacing(in GameObject obstacleToPlace, in int xCoord, in int yCoord)
    {
        obstacleToPlace.transform.localPosition = new Vector2(xCoord, -yCoord);
        var obstacleObject = obstacleToPlace.GetComponent<Obstacle>();
        var obstacleBody = obstacleObject.objectBody;

        Vector2 obstacleOffset = Vector2.zero;
        switch (obstacleObject.obstacleRotation) // Rotates then adds position offset
        {
            case Obstacle.ObstacleRotation.HORIZONTAL:
                obstacleBody.transform.localEulerAngles = Vector3.forward * -90;
                obstacleOffset = Vector2.up * obstacleObject.Size.x;
                break;
            case Obstacle.ObstacleRotation.SIDEWAYS:
                var backOrForth = Random.Range(0, 2) * 2 - 1;
                obstacleBody.transform.localEulerAngles = Vector3.back * 45 * backOrForth;
                if (backOrForth < 0)
                    obstacleOffset = Vector2.right * HypotenusHalfAntecedent(obstacleObject.Size.y);
                else
                    obstacleOffset = Vector2.up * HypotenusHalfAntecedent(obstacleObject.Size.x);
                break;
        }
        obstacleBody.transform.localPosition = obstacleOffset;
    }
    #region OBSTACLES_CALCULS
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
    int LargestRemainingGap(in List<int> _yPoss)
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
            var yCheck = _yPoss[i] - (_obstacleH - 1);
            for (int j = yCheck; j < yCheck + _obstacleH; ++j)
            {
                if (!_yPoss.Contains(j)) // obstacle can't fit at this Y position
                {
                    toRemove.Add(_yPoss[i]);
                    break;
                }
            }
        }
        foreach (int y in toRemove)
        {
            possCopy.Remove(y);
        }
        return possCopy;
    }
    #endregion
#endregion

    void SpawnablePlacing(in SpawnableObject spawnableToPlace)
    {
        var spawnableParams = spawnableToPlace.GetSpawnable();
        spawnableToPlace.transform.localPosition += (Vector3)spawnableParams.BlocPosition;

        if (spawnableParams is ObstacleSpawnable && spawnableToPlace.Size.x != spawnableToPlace.Size.y) // Rotation range setup only for long Obstacles, otherwise random rotation
        {
            spawnableToPlace.objectBody.localRotation = Quaternion.Euler(0, 0, (spawnableParams as ObstacleSpawnable).BodyRotation);
            spawnableToPlace.objectBody.localPosition = (spawnableParams as ObstacleSpawnable).BodyOffset;
            // random rotation offset
            if ((spawnableParams as ObstacleSpawnable).RotationOffsetRange != Vector2Int.zero)
            {
                int rotOffsetMin = (spawnableParams as ObstacleSpawnable).RotationOffsetRange.x;
                if (rotOffsetMin > 0) rotOffsetMin = 0;
                spawnableToPlace.objectBody.Rotate(Vector3.back, (Random.Range((spawnableParams as ObstacleSpawnable).RotationOffsetRange.x - rotOffsetMin, (spawnableParams as ObstacleSpawnable).RotationOffsetRange.y - rotOffsetMin) + rotOffsetMin)*22.5f);
                spawnableToPlace.objectBody.localPosition += spawnableToPlace.transform.position - spawnableToPlace.objectBody.gameObject.GetBoxColliderFixedBounds().min; // recalculate and assign body offset
            }
        }
        else // Random rotation for squared spawnables
        {
            var randomRot = Random.Range(0, 16);
            spawnableToPlace.objectBody.localRotation = Quaternion.Euler(0, 0, 22.5f * randomRot);
        }

        // random position offset // moved to bloc choice
        /*if (spawnableParams.OffsetXRange != Vector2Int.zero)
            spawnableToPlace.transform.localPosition += (Vector3)new Vector2(Random.Range(spawnableParams.OffsetXRange.x, spawnableParams.OffsetXRange.y), Random.Range(spawnableParams.OffsetYRange.x, spawnableParams.OffsetYRange.y));*/
    }

    // Spawns WPapers
    public void NewWP(Transform toUnpool)
    {
        PoolOut(toUnpool.parent.gameObject);
        GameObject pooledIn;
        currentWPMax += 6;
        PoolIn(ref wpPool, Vector3.right * currentWPMax, out pooledIn, transform);
        // Also pool in background objects
        WPObjects(in currentWPMax);
        // Also pool in spawnables
        SpawnablesSpawn(currentWPMax + 3);
    }

    bool isBack = false;
    int previousFrontSo = 3;
    int previousBackSo = -1;
    float previousFrontPos = 0;
    void WPObjects(in int thisWPX)
    {
        WPTrees(thisWPX);
        if (Random.Range(0, 2) == 0)
            WPRocks(thisWPX);
        if (Random.Range(0, 2) == 0)
            WPBushes(thisWPX);
    }

    void WPTrees(in int thisWPX)
    {
        switch (currentBlocAreaIdx)
        {
            case 0:

                break;
            case 1:
                
                break;
        }

        int objectCount = Random.Range(2, 5);
        List<int> xPoss = new List<int> { 0, 1, 2, 3, 4, 5 };
        List<GameObject> pooledInList = new List<GameObject>();

        for (int i = 0; i < objectCount; ++i)
        {
            GameObject pooledIn;
            var thisX = xPoss[Random.Range(0, xPoss.Count)];
            xPoss.Remove(thisX);
            PoolIn(ref backTreesPool, Vector3.right * (thisWPX - 3 + thisX), out pooledIn, spawnablesAnchor);

            pooledInList.Add(pooledIn);
            pooledIn.transform.localPosition += Vector3.up * 0.278f;
        }

        pooledInList = pooledInList.OrderBy(x => x.transform.localPosition.x).ToList(); // order the objects list from left to right
        foreach (GameObject pooledIn in pooledInList)
        {
            var renderer = pooledIn.GetComponent<SpriteRenderer>();

            if (pooledIn.transform.localPosition.x % 2 != 0) // put to back if even
                isBack = true;

            var thisFrontPos = pooledIn.transform.position.x;
            if (isBack) // second or every second object => goes to the back
            {
                renderer.sortingOrder = --previousBackSo;
                renderer.color = Color.HSVToRGB(0, 0, 0.5f); // half brightness
            }
            else // front obj
            {
                previousBackSo = -1; // back sorting order reset to minimum

                if (previousFrontPos > thisFrontPos) // this front obj touches the previous front obj
                {
                    if (previousFrontSo > 1)
                        --previousFrontSo;
                    else // bring to the back, too many objects are touching in a row (3)
                    {
                        previousFrontSo = 3; // reset front objects sorting order
                        renderer.sortingOrder = --previousBackSo;
                        renderer.color = Color.HSVToRGB(0, 0, 0.5f); // half brightness
                        continue;
                    }
                }
                else
                    previousFrontSo = 3; // reset front objects sorting order

                renderer.sortingOrder = previousFrontSo;
                renderer.color = Color.white;

                // calculating this sprite right bound world pos to be next previous Pos
                var sprite = renderer.sprite;
                var visibleWidth = (1 - (sprite.border.x + sprite.border.z) / sprite.texture.width) * sprite.bounds.size.x; // visible width = world width of pixels within the sprite borders (green box in editor)
                previousFrontPos = thisFrontPos + visibleWidth;
            }
            isBack = !isBack;
        }
    }

    float previousRockPos = 0;
    void WPRocks(in int thisWPX)
    {
        GameObject pooledIn;
        WPSingle(thisWPX, ref backRocksPool, out pooledIn, ref previousRockPos, 0);
        pooledIn.transform.localPosition += Vector3.up * 0.5f;
    }

    float previousBushPos = 0;
    void WPBushes(in int thisWPX)
    {
        GameObject pooledIn;
        WPSingle(thisWPX, ref backBushesPool, out pooledIn, ref previousBushPos, 5);
        pooledIn.transform.localPosition += Vector3.up * 0.15f;
    }

    void WPSingle(in int thisWPX, ref List<GameObject> pool, out GameObject pooledIn, ref float previousObjPos, in int defaultSortingOrder)
    {
        var thisX = Random.Range(0, 5);
        PoolIn(ref pool, Vector3.right * (thisWPX - 3 + thisX), out pooledIn, spawnablesAnchor);

        var renderer = pooledIn.GetComponent<SpriteRenderer>();

        var thisFrontPos = pooledIn.transform.position.x;
        if (thisFrontPos < previousObjPos)
            renderer.sortingOrder = defaultSortingOrder - 1;

        previousObjPos = thisFrontPos + renderer.sprite.bounds.size.x;
    }

    #endregion

    #region pool
    // objects to appear next are pooled in (activated)
    public void PoolIn(ref List<GameObject> pool, Vector2 toPosition, out GameObject pooledInObj, Transform parent = null)
    {
        var objectToPoolIn = pool.FirstOrDefault(i => !i.activeInHierarchy); // finds the first inactive object in the pool

        //error (pool full)
        if (objectToPoolIn == null)
            Debug.LogError("No more remaining to pool in");

        objectToPoolIn.transform.parent = parent;
        objectToPoolIn.transform.localPosition = toPosition; // position the object
        objectToPoolIn.SetActive(true); // activate the object
        pooledInObj = objectToPoolIn;
    }
    // objects no longer being seen are pooled out (deactivated)
    public void PoolOut(GameObject toPoolOut, in Transform _parent = null) // for WPapers and background objects
    {
        toPoolOut.SetActive(false);
        if (_parent) // background objects
            toPoolOut.transform.parent = _parent;
    }
    public void PoolOut (in SpawnableObject toPoolOut) // for spawnables
    {
        PoolOut(toPoolOut.gameObject);
        toPoolOut.transform.parent = spawnablesPools.transform.GetChild(toPoolOut.GetSpawnable().SpawnablePrefabIndex); // reparent to original pool
    }
#endregion
}
