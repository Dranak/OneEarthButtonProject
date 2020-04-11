using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]

public class WormBody : MonoBehaviour
{

    public WormBody Target { get; set; }
    public Rigidbody2D Rigidbody { get; set; }
    public CircleCollider2D Collider { get; set; }
    public TrailRenderer Trail { get; set; }
    public LineRenderer LineRenderer { get; set; }
    public float Damping;

    // Start is called before the first frame update
    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Trail = GetComponent<TrailRenderer>();
        Collider = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        FollowTarget();
    }


    public void SetTarget(WormBody target)
    {
        Target = target;    
        LineRenderer = GetComponent<LineRenderer>();

    }

    public void UpdateLineRender()
    {
        LineRenderer.SetPosition(0, Rigidbody.position);
        LineRenderer.SetPosition(1, Target.Rigidbody.transform.position);
    }

    void FollowTarget()
    {
        if (!Target)
            return;
        Rigidbody.MovePosition(Vector2.Lerp(Rigidbody.position,(Vector2) Target.Rigidbody.transform.position, Time.fixedDeltaTime * Vector2.Distance(Rigidbody.transform.position, Target.Rigidbody.transform.position) * Damping));
    }



}
