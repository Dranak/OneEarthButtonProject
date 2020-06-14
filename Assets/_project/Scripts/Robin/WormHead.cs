using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CircleCollider2D))]
public class WormHead : WormBody
{
    [Header("Body")]
    public WormBody PrefabWormBody;
    public int NumberOfParts;
    public float OffsetBodyPart = 5f;

    public List<Face> AllFaces { get; set; } = new List<Face>();

    private Face _lastFace;
    [Space]

    private List<WormBody> _wormBodies;
    public Vector3 StartPosition { get; set; }
    public float DistanceFromStart { get { return Vector3.Distance(StartPosition, transform.position); } }
    public LineRenderer Line;



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

    public Action<Obstacle> CallBackDead;
    public Action<Collectible> CallBackPoint;

    protected override void Awake()
    {
        base.Awake();
        AllFaces = GetComponentsInChildren<Face>().ToList();
        _lastFace = AllFaces.Where(f => f.FaceType == FeelType.Normal).FirstOrDefault();
    }

    void Start()
    {
        SetupBody();
        StartPosition = Rigidbody.position;
        Line.positionCount = _wormBodies.Count + 1;
      

    }

    void Update()
    {
        UpdateLineRenderer();
        if (GameManager.Instance.State == State.Play)
        {
            Sight();
            IncreaseSpeed();
      
            if (Input.GetKey(KeyCode.Space) || Input.touchCount > 0)
            {
                IsDigging = true;
            }
            else
            {
                IsDigging = false;

            }
        }


    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.State == State.Play)
        {
            SetForce(IsDigging);
        }
    }

    void SetForce(bool _isDigging)
    {
        Rigidbody.velocity = new Vector2(Vector2.right.x * Speed, Rigidbody.velocity.y);


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




    void UpdateLineRenderer()
    {
        Line.SetPosition(0, Anchor.position);
        //Line.SetPosition(1, _wormBodies.First().transform.position);
        for (int index = 0; index < _wormBodies.Count; ++index)
        {
           
            Line.SetPosition(index + 1,  _wormBodies[index].transform.position);
        }
        Line.SetPosition(_wormBodies.Count, _wormBodies.Last().Anchor.position);
        //Line.SetPosition(inde)


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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Death"))
        {
            Debug.Log("Dead by " + collision.gameObject.name);
            CallBackDead(collision.transform.parent.GetComponent<Obstacle>());
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Collectible"))
        {
            Collectible collectible = collider.transform.parent.GetComponent<Collectible>();
            Debug.Log("Ate " + collectible.name);
            CallBackPoint(collectible);

            BlocManager.Instance.PoolOut(collectible);
        }
        else if (collider.CompareTag("BlocPoolerTrigger"))
        {
            // reset egg shells series (_streakEggShell)
            Debug.Log("Reset Egg shell Index: " + GameManager.Instance.Player.StreakEggShell);
            GameManager.Instance.Player.StreakEggShell = 0;
            GameManager.Instance.Player.playingBlocName = BlocManager.Instance.randomBloc.blocName; // going through new bloc
            Destroy(collider.gameObject); // not needed any more
        }
    }

    void Sight()
    {

        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, transform.right, 2f);

        if (hit2D)
        {
            SpawnableObject spawnableObject = hit2D.transform.parent.gameObject.GetComponent<SpawnableObject>();

            if (spawnableObject)
            {
                if (spawnableObject is Obstacle)
                {
                    _lastFace.gameObject.SetActive(false);
                    _lastFace = AllFaces.Where(f => f.FaceType == FeelType.Fear).FirstOrDefault();
                    _lastFace.gameObject.SetActive(true);

                }
                else if (spawnableObject is Collectible)
                {
                    _lastFace.gameObject.SetActive(false);
                    _lastFace = AllFaces.Where(f => f.FaceType == FeelType.Happy).FirstOrDefault();
                    _lastFace.gameObject.SetActive(true);
                }


            }
            else
            {
                if (_lastFace.FaceType == FeelType.Fear)
                {
                    _lastFace.gameObject.SetActive(false);
                    _lastFace = AllFaces.Where(f => f.FaceType == FeelType.Sweat).FirstOrDefault();
                    _lastFace.gameObject.SetActive(true);
                }
                else
                {
                    _lastFace.gameObject.SetActive(false);
                    _lastFace = AllFaces.Where(f => f.FaceType == FeelType.Normal).FirstOrDefault();
                    _lastFace.gameObject.SetActive(true);
                }
            }
        }
    }



    //LEGACY
    //void UpdateCollider()
    //{
    //    List<Vector2> colliderPoints = new List<Vector2>();
    //    List<Vector2> upperPoints = new List<Vector2>();
    //    List<Vector2> downerPoint = new List<Vector2>();
    //    float halfWidth = Line.startWidth / 2f;
    //    Vector2 upPoint = transform.position;
    //    Vector2 downPoint = transform.position;
    //    upPoint.y -= halfWidth + transform.parent.position.y;
    //    downPoint.y += halfWidth - transform.parent.position.y;
    //    upperPoints.Add(new Vector2(transform.position.x + halfWidth, transform.position.y - transform.parent.position.y));
    //    upperPoints.Add(upPoint);

    //    downerPoint.Add(downPoint);

    //    for (int index = 0; index < Line.positionCount; ++index)
    //    {
    //        upPoint = Line.GetPosition(index);
    //        downPoint = Line.GetPosition(index);
    //        upPoint.y -= halfWidth + transform.parent.position.y;
    //        downPoint.y += halfWidth - transform.parent.position.y;
    //        upperPoints.Add(upPoint);
    //        if (index == Line.positionCount - 1)
    //        {
    //            upperPoints.Add(new Vector2(Line.GetPosition(index).x - halfWidth, transform.position.y - transform.parent.position.y));
    //        }

    //        downerPoint.Add(downPoint);
    //    }

    //    colliderPoints.AddRange(upperPoints);
    //    downerPoint.Reverse();
    //    colliderPoints.AddRange(downerPoint);
    //    colliderPoints.ForEach(cp => transform.parent.InverseTransformPoint(cp));



    //}


}
