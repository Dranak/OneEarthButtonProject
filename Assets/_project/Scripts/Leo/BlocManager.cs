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

    //[HideInInspector]
    public List<GameObject> blocsPool;
    public float currentBlocMax = 400;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);
    }
    /*void AddToPool(Object[] parent, ref List<GameObject> pool, bool letActive = false)
    {
        foreach (Object child in parent)
        {
            pool.Add((child as Transform).GetComponent<GameObject>());
            if (!letActive)
                ((child as Transform).GetComponent<GameObject>()).SetActive(false);
        }
    }*/
    
    #region pool
    // objects to appear next are pooled in (activated)
    public void PoolIn(ref List<GameObject> pool, Vector3 toPosition, out GameObject pooledInObj)
    {
        var objectToPoolIn = pool.FirstOrDefault(i => !i.activeInHierarchy); // finds the first inactive object in the pool

        //error (pool full)
        if (objectToPoolIn == null)
            Debug.LogError("No more remaining to pool in");

        objectToPoolIn.transform.localPosition = toPosition; // position the object
        objectToPoolIn.SetActive(true); // activate the object
        pooledInObj = objectToPoolIn;

        // incremement X position where to spawn next bloc (for blocs only)
        var possBloc = pooledInObj.GetComponent<Bloc>();
        if (possBloc != null)
            currentBlocMax += possBloc.blocWidth;
    }
    // objects no longer being seen are pooled out (deactivated)
    public void PoolOut(GameObject toPoolOut)
    {
        toPoolOut.SetActive(false);
    }
    #endregion
}
