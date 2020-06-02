using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooler : MonoBehaviour
{
    private void Awake()
    {
        transform.localPosition = Vector2.right * GetComponentInParent<Camera>().orthographicSize * GetComponentInParent<Camera>().aspect + Vector2.left * 0.5f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "BlocPoolerTrigger")
        {
            BlocManager.Instance.NewBloc();
            //Destroy(other.gameObject); // still needed to reset eggshell series

            // Add background transition here if needed
        }
    }
}
