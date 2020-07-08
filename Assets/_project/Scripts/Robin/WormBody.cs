using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class WormBody : MonoBehaviour
{
    public WormBody Target { get; set; }
    public Rigidbody2D Rigidbody { get; set; }
    public ParticleSystemRenderer Trail { get; set; }
    public SpriteRenderer SpriteExtremity { get; set; }
    public float Damping;
    public Transform Anchor;

    protected virtual void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Trail = GetComponentInChildren<ParticleSystemRenderer>(); // new TRAIL is a Particle System
        SpriteExtremity = GetComponent<SpriteRenderer>();
        Trail.enabled = SpriteExtremity.enabled;
    }

    private void FixedUpdate()
    {
        FollowTarget();
    }

    public void SetTarget(WormBody target)
    {
        Target = target;
    }

    Vector2 currentBodyVel;

    void FollowTarget()
    {
        if (!Target)
            return;

        var moveDeltaTime = Time.fixedDeltaTime * Vector2.Distance(Rigidbody.position, Target.Rigidbody.position) * Damping * (GameManager.Instance.Player.WormHead.Rigidbody.velocity.magnitude / 10);
        Rigidbody.MovePosition(Vector2.SmoothDamp(Rigidbody.position, Target.Rigidbody.position, ref currentBodyVel, 1f, 10f, moveDeltaTime));

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

    

    

   
}
