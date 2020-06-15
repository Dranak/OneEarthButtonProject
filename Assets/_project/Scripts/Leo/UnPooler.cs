using UnityEngine;

public class UnPooler : MonoBehaviour
{
    BlocManager blocManager;

    private void Start()
    {
        blocManager = BlocManager.Instance;
        transform.localPosition = Vector2.left * GetComponentInParent<Camera>().orthographicSize * GetComponentInParent<Camera>().aspect + Vector2.left * 0.5f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var tag = other.tag;
        if (tag == "WPUnPoolerTrigger")
        {
            blocManager.NewWP(other.transform);
        }
        if (tag == "Death" || tag == "Collectible")
        {
            blocManager.PoolOut(other.transform.GetComponentInParent<SpawnableObject>());
        }

        if (tag == "BackTree")
        {
            blocManager.PoolOut(other.transform.parent.gameObject, blocManager.backTreesAnchor);
        }
        if (tag == "BackRock")
        {
            blocManager.PoolOut(other.transform.parent.gameObject, blocManager.backRocksAnchor);
        }
        if (tag == "BackBush")
        {
            blocManager.PoolOut(other.transform.parent.gameObject, blocManager.backBushesAnchor);
        }
    }
}
