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


    [Header("Velocity")]
    public float Speed;
    public float VelocityRising { get; set; }
    public float VelocityDig { get; set; }

    public float AccelerationTimeRising;
    public AnimationCurve AccelerationCurveRising;
    float timeaccelerationRising = 0f;

    float timeaccelerationDig = 0f;
    public float AccelerationTimeDig;
    public AnimationCurve AccelerationCurveDig;

    private bool IsDigging = false;

    public Action CallBackDead;
    public Action<CollectibleSpawnable> CallBackPoint;

    void Start()
    {
       
        _collider = GetComponent<PolygonCollider2D>();
        SetupBody();
        StartPosition = Rigidbody.position;
        Line.positionCount = _wormBodies.Count + 1;
      
    }

    void Update()
    {
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
        Debug.Log("Speed: " + Speed);

        if (_isDigging)
        {
            Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, -Accelerate(AccelerationCurveDig, VelocityDig, timeaccelerationDig, AccelerationTimeDig));
            timeaccelerationDig += Time.fixedDeltaTime;
        }
        else
        {
            timeaccelerationDig = 0f;
            if (Rigidbody.position.y < StartPosition.y)
            {
                Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, Accelerate(AccelerationCurveRising, VelocityRising, timeaccelerationRising, AccelerationTimeRising));
                timeaccelerationRising += Time.fixedDeltaTime;
            }

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Death"))
        {
            CallBackDead();
        }
        //else
        //{
        //    CoinsSpawnable collectible = collision.gameObject.GetComponent<CoinsSpawnable>();
        //    if (collectible != null)
        //    {
        //        CallBackPoint(collectible);
        //    }
        //}
    }

    void UpdateLineRenderer()
    {
        Line.SetPosition(0, transform.position);
        for (int index = 0; index < _wormBodies.Count; ++index)
        {
            Line.SetPosition(index + 1, _wormBodies[index].transform.position);
        }
        UpdateCollider();
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
        //List<Vector2> colliderPoints = new List<Vector2>();
        float halfWidth = Line.startWidth / 2f;
        for (int index = 0; index < Line.positionCount; ++index)
        {
            Vector2 upPoint = Line.GetPosition(index);
            Vector2 downPoint = Line.GetPosition(index);
            upPoint.x -= halfWidth;
            downPoint.x += halfWidth;
            _collider.points.ToList().Add(upPoint);
            _collider.points.ToList().Add(downPoint);
        }



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


