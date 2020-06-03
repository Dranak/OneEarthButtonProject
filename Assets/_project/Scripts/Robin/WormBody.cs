using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class WormBody : MonoBehaviour
{

    public WormBody Target { get; set; }
    public Action<Collision2D> GetRekt;
    //public CircleCollider2D Collider { get; set; }
    //public TrailRenderer Trail { get; set; } // LEGACY
    public Rigidbody2D Rigidbody { get; set; }
    public ParticleSystemRenderer Trail { get; set; }
    public float Damping;

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Trail = GetComponentInChildren<ParticleSystemRenderer>(); // new TRAIL is a Particle System
        //Collider = GetComponent<CircleCollider2D>();
    }

    private void FixedUpdate()
    {
        FollowTarget();
    }

    public void SetTarget(WormBody target)
    {
        Target = target;
    }

    void FollowTarget()
    {
        if (!Target)
            return;
 
           Rigidbody.MovePosition( Vector2.Lerp(Rigidbody.position, (Vector2)Target.Rigidbody.position, Time.fixedDeltaTime * Vector2.Distance(Rigidbody.position, Target.Rigidbody.position) * Damping));
        Vector3 diff = Vector3.zero;
        if (Target is WormHead)
        {
            diff = Target.transform.position - transform.position;
            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

            Target.transform.rotation = Quaternion.Euler(0f, 0f, rot_z);

        }
        else if (Trail.enabled)
        {
            diff = transform.position - Target.transform.position;
            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0f, 0f, rot_z + 180f);
        }
    }

    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GetRekt(collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BlocPoolerTrigger"))
        {
            // reset egg shells series (_streakEggShell)
            GameManager.Instance.Player.StreakEggShell = 0;

            Destroy(collision.gameObject); // not needed any more
        }
    }
}
