using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D), typeof(PolygonCollider2D))]
public class WormHead : WormBody
{
    [Header("Body")]
    public WormBody PrefabWormBody;
    public int NumberOfParts;
    public float OffsetBodyPart = 5f;

    private List<WormBody> _wormBodies;
    public Vector3 StartPosition { get; set; }
    public float DistanceFromStart { get { return Vector3.Distance(StartPosition, transform.position); } }
    public LineRenderer Line;

    PolygonCollider2D _collider;

    public float Speed { get; set; }
    public float TimeToIncreaseSpeed { get; set; }
    public float MaxSpeed { get; set; }
    public float ValueIncreaseSpeed { get; set; }
    float _chronoIncreaseSpeed = 0f;
    public float VelocityRising { get; set; }
    public float VelocityDig { get; set; }

    public float AccelerationTimeRising;
    public AnimationCurve AccelerationCurveRising;
    float _chronoAccelerationRising = 0f;

    float _chronoAccelerationDig = 0f;
    public float AccelerationTimeDig;
    public AnimationCurve AccelerationCurveDig;

    private bool IsDigging = false;

    public Action CallBackDead;
    public Action<Collectible> CallBackPoint;

    void Start()
    {

        _collider = GetComponent<PolygonCollider2D>();
        SetupBody();
        StartPosition = Rigidbody.position;
        Line.positionCount = _wormBodies.Count + 1;
       // UpdateCollider();
    }

    void Update()
    {
        IncreaseSpeed();
        UpdateLineRenderer();

        if (Input.GetKey(KeyCode.Space) || Input.touchCount > 0)
        {
            IsDigging = true;
        }
        else
        {
            IsDigging = false;

        }

    }

    private void FixedUpdate()
    {

        SetForce(IsDigging);

    }

    void SetForce(bool _isDigging)
    {
        Rigidbody.velocity = new Vector2(Vector2.right.x * Speed, Rigidbody.velocity.y);
        //Debug.Log("Speed: " + Speed);

        if (_isDigging)
        {
            _chronoAccelerationRising = 0f;
            Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, -Accelerate(AccelerationCurveDig, VelocityDig, _chronoAccelerationDig, AccelerationTimeDig));
            _chronoAccelerationDig += Time.fixedDeltaTime;
        }
        else
        {
            _chronoAccelerationDig = 0f;
            if (Rigidbody.position.y < StartPosition.y)
            {
                Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, Accelerate(AccelerationCurveRising, VelocityRising, _chronoAccelerationRising, AccelerationTimeRising));
                _chronoAccelerationRising += Time.fixedDeltaTime;
            }

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        SpawnableObject spawnableObject = collision.gameObject.GetComponentInParent<SpawnableObject>();

        if(spawnableObject)
        {
            if (spawnableObject is Obstacle)
            {
                //Debug.Log("Dead by " + spawnableObject.name);
                CallBackDead();
            }
            else if (spawnableObject is Collectible)
            {
                Debug.Log("Ate " + spawnableObject.name);
                CallBackPoint((spawnableObject as Collectible));
                BlocManager.Instance.PoolOut(spawnableObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "BlocPoolerTrigger")
        {
            // reset egg shells series (_streakEggShell)
            Destroy(collision.gameObject); // not needed any more
        }
    }

    void UpdateLineRenderer()
    {
        Line.SetPosition(0, transform.position);
        for (int index = 0; index < _wormBodies.Count; ++index)
        {
            Line.SetPosition(index + 1, _wormBodies[index].transform.position);
        }
       // UpdateCollider();

    }

    float Accelerate(AnimationCurve curve, float maxVelocity, float time, float duration)
    {
        float scaleTime = time / duration;
        if (time <= duration)
            return curve.Evaluate(scaleTime) * maxVelocity;
        else
            return maxVelocity;
    }

    float GetAngleToRotate(float startAngleRotate, float endAngleRotate, float duration, float chrono)
    {

        float scaleTimeRotate = chrono / duration;

        return Mathf.Lerp(startAngleRotate, endAngleRotate, scaleTimeRotate);
    }

    void SetupBody()
    {
        _wormBodies = new List<WormBody>();
        for (int i = 0; i < NumberOfParts; ++i)
        {
            _wormBodies.Add(Instantiate(PrefabWormBody, transform.position + Vector3.left * (i + 1) * OffsetBodyPart, Quaternion.identity));
            _wormBodies[i].Trail.enabled = false;

            if (i > 0)
                _wormBodies.Last().SetTarget(_wormBodies[i - 1]);
            else
                _wormBodies.Last().SetTarget(this);
        }
        _wormBodies.Last().Trail.enabled = true;
        _wormBodies.Last().GetComponent<SpriteRenderer>().enabled = true;
    }

    void UpdateCollider()
    {
        List<Vector2> colliderPoints = new List<Vector2>();
        List<Vector2> upperPoints = new List<Vector2>();
        List<Vector2> downerPoint = new List<Vector2>();
        float halfWidth = Line.startWidth / 2f;
        Vector2 upPoint = transform.position;
        Vector2 downPoint = transform.position;
        upPoint.y -= halfWidth + transform.parent.position.y;
        downPoint.y += halfWidth - transform.parent.position.y;
        upperPoints.Add(new Vector2(transform.position.x + halfWidth, transform.position.y - transform.parent.position.y));
        upperPoints.Add(upPoint);

        downerPoint.Add(downPoint);

        for (int index = 0; index < Line.positionCount; ++index)
        {
             upPoint = Line.GetPosition(index);
            downPoint = Line.GetPosition(index);
            upPoint.y -= halfWidth + transform.parent.position.y;
            downPoint.y += halfWidth - transform.parent.position.y;
            upperPoints.Add(upPoint);
            if(index == Line.positionCount-1)
            {
                upperPoints.Add(new Vector2(Line.GetPosition(index).x - halfWidth, transform.position.y - transform.parent.position.y));
            }

            downerPoint.Add(downPoint);
        }

        colliderPoints.AddRange(upperPoints);
        downerPoint.Reverse();
        colliderPoints.AddRange(downerPoint);
        colliderPoints.ForEach(cp => transform.parent.InverseTransformPoint(cp));
        _collider.SetPath(0, colliderPoints);


    }

    void IncreaseSpeed()
    {

        if (Speed < MaxSpeed)
        {
            if (_chronoIncreaseSpeed >= TimeToIncreaseSpeed)
            {
                Speed += ValueIncreaseSpeed;
                Speed = Mathf.Min(Speed, MaxSpeed);
                _chronoIncreaseSpeed = 0f;
            }
        }
        _chronoIncreaseSpeed += Time.deltaTime;
    }

    //void SetForce(bool _isDigging)
    //{
    //    Rigidbody.velocity = Vector2.right * Speed;

    //    if (_isDigging)
    //    {
    //        _chronoRotateRising = 0f;
    //        if (_chronoRotateDig <= DurationRotateDig)
    //        {
    //            Rigidbody.MoveRotation(-GetAngleToRotate(MinAngleDig, MaxAngleDig, DurationRotateDig, _chronoRotateDig));
    //            _chronoRotateDig += Time.fixedDeltaTime;
    //        }

    //        velocity = VelocityDig;
    //        //timeacceleratioDig += Time.fixedDeltaTime;
    //    }
    //    else
    //    {
    //        //timeacceleratioDig = 0f;
    //        _chronoRotateDig = 0f;

    //        if (Rigidbody.position.y < StartPosition.y)
    //        {
    //            if (_chronoRotateRising == 0)
    //                _startAngleRising = Rigidbody.rotation;

    //            Rigidbody.MoveRotation(GetAngleToRotate(_startAngleRising, MaxAngleRising, DurationRotateRising, _chronoRotateRising));
    //            //timeaccelerationRising += Time.fixedDeltaTime;
    //            _chronoRotateRising += Time.fixedDeltaTime;
    //            velocity = VelocityRising;
    //        }
    //        else
    //        {
    //            velocity = 0f;
    //        }
    //    }

    //    Rigidbody.velocity = (Vector2)(transform.right * velocity) + (Vector2.right * Speed);


    //}

}
