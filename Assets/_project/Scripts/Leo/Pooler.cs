using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooler : MonoBehaviour
{
    private void Awake()
    {
        transform.localPosition = Vector2.right * GameManager.Instance.camera.orthographicSize * GameManager.Instance.camera.aspect + Vector2.left * 0.5f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "BlocPoolerTrigger")
        {
            BlocManager.Instance.NewBloc();
            Destroy(other.gameObject);
        }
    }
}
