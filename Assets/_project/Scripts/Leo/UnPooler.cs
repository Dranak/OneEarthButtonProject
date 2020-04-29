using UnityEngine;

public class UnPooler : MonoBehaviour
{
    BlocManager blocManager;

    private void Awake()
    {
        blocManager = BlocManager.Instance;
        transform.localPosition = Vector2.left * GameManager.Instance.camera.orthographicSize * GameManager.Instance.camera.aspect + Vector2.left * 0.5f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var tag = other.tag;
        if (tag == "WPUnPoolerTrigger")
        {
            blocManager.NewWP(other.transform);
        }
        if (tag == "Death")
        {
            blocManager.PoolOut(other.transform.parent.gameObject);
        }
    }
}
