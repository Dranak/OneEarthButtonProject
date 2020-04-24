using UnityEngine;

public class UnPooler : MonoBehaviour
{
    [SerializeField]
    bool isCameraUnPooler;

    private void Awake()
    {
        if (isCameraUnPooler)
        {
            transform.localPosition = Vector2.left * Camera.main.orthographicSize * Camera.main.aspect + Vector2.left * 0.5f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var tag = other.tag;
        if (tag == "BlocUnPoolerTrigger")
        {
            /*foreach(Transform obs in other.transform)
            {
                if (obs.tag == "Death")
                {
                    BlocManager.Instance.PoolOut(other.transform.parent.gameObject;
                }
            }*/
            BlocManager.Instance.PoolOut(other.transform.parent.gameObject);
            // Pool in new bloc
            BlocManager.Instance.NewBloc();
        }
        if (tag == "Death")
        {
            BlocManager.Instance.PoolOut(other.transform.parent.gameObject);
        }
        /*
        else if (tag == "")
        {
            
        }*/
    }
}
