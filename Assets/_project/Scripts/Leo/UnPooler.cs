using UnityEngine;

public class UnPooler : MonoBehaviour
{
    BlocManager blocManager;

    private void Awake()
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
    }
}
