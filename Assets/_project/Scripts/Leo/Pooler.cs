﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooler : MonoBehaviour
{
    private void Start()
    {
        transform.localPosition = Vector2.right * GetComponentInParent<Camera>().orthographicSize * GetComponentInParent<Camera>().aspect + Vector2.left * 0.5f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

    }
}
