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
    public SpriteRenderer Eyes;
    public SpriteRenderer Mouth;
    [SerializeField] ParticleSystem headDirtParticle;
   // public SkinData DefaultSkin;

    private FeelType _lastFace;
    private FeelType _currentFace;
    public float TimeFaceDisplayed { get; set; }
    private float _chronoFaceDisplayed = 0f;
    [Space]

    private List<WormBody> _wormBodies;
    public Vector3 StartPosition { get; set; }
    public float DistanceFromStart { get { return Vector3.Distance(StartPosition, transform.position); } }
    public LineRenderer Line;
    public LayerMask LayerMaskSpawnables;
    public float FieldOfView { get; set; }

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

    public Action<Obstacle, Player> CallBackDead;
    public Action<Collectible> CallBackPoint;

    private string _lastNameCollectible = string.Empty;

    [SerializeField] Player _player;
    protected override void Awake()
    {
        base.Awake();

        _lastFace = FeelType.Normal;
        _currentFace = FeelType.Normal;
      

        
       
    }

    void Start()
    {
       // SetupBody();
      //  SetSkin(DefaultSkin);
    }

    bool touchingInput = false;
    void Update()
    {
        UpdateLineRenderer();
        if (UiManager.Instance.State == State.Play)
        {
            Sight();
            IncreaseSpeed();

            touchingInput = false;

#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.Space))
            {
                touchingInput = true;
            }
#endif
#if UNITY_STANDALONE
            if (Input.GetKey(KeyCode.Space))
            {
                touchingInput = true;
            }
            if (Input.GetMouseButton(0))
            {
                touchingInput = true;
            }
#endif

#if UNITY_IOS
            if (Input.touchCount > 0)
            {
                touchingInput = true;
            }
#endif
#if UNITY_ANDROID
            if (Input.touchCount > 0)
            {
                touchingInput = true;
            }
#endif

            IsDigging = touchingInput;
        }
    }

    private void FixedUpdate()
    {
        if (UiManager.Instance.State == State.Play)
        {
            transform.localPosition = new Vector2(transform.localPosition.x, Mathf.Clamp(transform.localPosition.y, -9, 0)); // clamping the worm position between the grass (0) and bedrock (-9)
            SetForce(IsDigging);
        }
    }

    void SetForce(bool _isDigging)
    {
        Rigidbody.velocity = new Vector2(Speed, Rigidbody.velocity.y);

        if (_isDigging)
        {
            _chronoAccelerationRising = 0f;
            if (transform.localPosition.y > -9)
            {
                Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, -Accelerate(AccelerationCurveDig, VelocityDig, _chronoAccelerationDig, AccelerationTimeDig));
                _chronoAccelerationDig += Time.fixedDeltaTime;
            }
            else
                Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, 0);
        }
        else
        {
            _chronoAccelerationDig = 0f;
            if (Rigidbody.position.y < StartPosition.y)
            {
                Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, Accelerate(AccelerationCurveRising, VelocityRising, _chronoAccelerationRising, AccelerationTimeRising));
                _chronoAccelerationRising += Time.fixedDeltaTime;
            }
            else
                Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, 0);
        }
    }




    void UpdateLineRenderer()
    {
        Line.SetPosition(0, Anchor.position);
        //Line.SetPosition(1, _wormBodies.First().transform.position);
        for (int index = 0; index < _wormBodies.Count; ++index)
        {

            Line.SetPosition(index + 1, _wormBodies[index].transform.position);
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

    public void SetupBody()
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
        _wormBodies.Last().SpriteExtremity.enabled = true;

        StartPosition = Rigidbody.position;
        Line.positionCount = _wormBodies.Count + 1;
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
        var col = collision.collider;
        if (col.CompareTag("Death"))
        {
            if (_player.currentBonus == Player.Bonus.Shield)
            {
                _player.ActivateBonus(0);
                col.enabled = false;
            }
            else if (_player.currentBonus == Player.Bonus.Rage)
            {
                col.enabled = false;
            }
            else
            {
                //Debug.Log("Dead by " + collision.gameObject.name);
                CallBackDead(collision.transform.parent.GetComponent<Obstacle>(), _player);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Collectible"))
        {
            Collectible collectible = collider.transform.parent.GetComponent<Collectible>();

            if (_lastNameCollectible == collectible.name && collectible.collectibleParameters.EggShellIndex > -1 && collectible.collectibleParameters.EggShellIndex == _player.LastIndexEggShell)
            {
                BlocManager.Instance.PoolOut(collectible);
                // hotfix
            }
            else
            {
                _lastNameCollectible = collectible.name;
                BlocManager.Instance.PoolOut(collectible);
                //Debug.Log("Ate " + collectible.name + " id " + collectible.gameObject.GetInstanceID());
                CallBackPoint(collectible);
            }
        }
        else if (collider.CompareTag("Bonus"))
        {
            var bonusObj = collider.transform.parent.GetComponent<Collectible>();
            var name = bonusObj.collectibleParameters.Tag;

            BlocManager.Instance.PoolOut(bonusObj);

            CallBackPoint(bonusObj);
        }
        else if (collider.CompareTag("BlocPoolerTrigger"))
        {
            // reset egg shells series (_streakEggShell)
            //Debug.Log("Reset StreakEggShell: " + player.StreakEggShell);
            _player.StreakEggShell = 0;
            _player.playingBlocName = BlocManager.Instance.randomBloc.blocName; // going through new bloc
            Destroy(collider.gameObject); // not needed any more
        }
    }

    void Sight()
    {

        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, transform.right, FieldOfView, LayerMaskSpawnables.value);

        if (hit2D)
        {
            SpawnableObject spawnableObject = hit2D.transform.parent.gameObject.GetComponent<SpawnableObject>();

            if (spawnableObject)
            {
                if (spawnableObject is Obstacle)
                {
                    SetFace(FeelType.Fear);

                }
                else if (spawnableObject is Collectible)
                {
                    SetFace(FeelType.Happy);
                }


            }

        }
        else
        {
            _chronoFaceDisplayed += Time.deltaTime;
            if (_chronoFaceDisplayed >= TimeFaceDisplayed)
            {
                if (_lastFace == FeelType.Fear)
                {
                    SetFace(FeelType.Sweat);
                }
                else
                {
                    SetFace(FeelType.Normal);
                }
                _chronoFaceDisplayed = 0f;

            }

        }
    }

    public void SetSkin(SkinData skindata)
    {
        SpriteExtremity.sprite = skindata.HeadSprite;
        _wormBodies.Last().SpriteExtremity.sprite = skindata.TailSprite;
        Line.material = skindata.BodyMaterial;
        AllFaces = skindata.Faces;
        SetFace(FeelType.Normal);
    }

    public void SetFace(FeelType feel )
    {
        _lastFace = _currentFace;
        _currentFace = feel;
        Face face = AllFaces.Where(f => f.FaceType == feel).FirstOrDefault();
        Eyes.sprite = face.Eyes;
        Mouth.sprite = face.Mouth;
        
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
