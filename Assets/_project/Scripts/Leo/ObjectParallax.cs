using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectParallax : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteR;
    [SerializeField] int parallaxLevel;

    void FixedUpdate()
    {
        if (spriteR.sortingOrder < 0)
        {
            transform.position += Vector3.right * parallaxLevel * (GameManager.Instance.Player.WormHead.Speed / 2000);
        }
    }
}
