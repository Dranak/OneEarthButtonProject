using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BlocManager : MonoBehaviour
{
    public static BlocManager Instance;

    [SerializeField]
    Bloc.BlocKind startingBlocKind = 0;
    [SerializeField]
    Bloc[] countryBG2, townBG2, transitionsBG;//...
    [SerializeField]
    Transform cansPoolT;

    //[HideInInspector]
    public List<GameObject> blocsPool;
    [SerializeField][HideInInspector]
    List<GameObject> cansPool;
    public float currentBlocMax = 200;

    private void Awake()
    {
        if (Instance == null)
        {
            AddToPool(cansPoolT, ref cansPool);
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

    private void Start()
    {
        NewBloc(); // first bloc to be created (after the empty starting bloc)
    }

    public void NewBloc()
    {
        GameObject pooledIn;
        PoolIn(ref blocsPool, Vector3.right * currentBlocMax, out pooledIn);



        // incremement X position where to spawn next bloc
        currentBlocMax += pooledIn.GetComponent<Bloc>().blocWidth;
    }

    #region procedural

    void ObstacleSpawn()
    {
        
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
