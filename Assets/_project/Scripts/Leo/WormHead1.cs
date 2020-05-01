using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WormHead1 : WormBody
{
    public int NumberOfParts;
    public float OffsetBodyPart = 5f;
    public float Speed;
    public float DiggingForce;
    public WormBody PrefabWormBody;
    public float _angleDig = 30f;
    private List<WormBody> _wormBodies;
    private bool IsDigging = false;
    public bool IsDead { get; set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        _wormBodies = new List<WormBody>();
        for (int i =0;i<NumberOfParts;++i)
        {
            _wormBodies.Add(Instantiate(PrefabWormBody, transform.position + Vector3.left * (i+1 )* OffsetBodyPart, Quaternion.identity));

            if (i > 0)
                _wormBodies.Last().SetTarget(_wormBodies[i-1]);
            else
                _wormBodies.Last().SetTarget(this);

        }
        _wormBodies.Last().Trail.enabled = true;
        Debug.Log("NTM");
    }

    Vector2 currentForce;

    void Update()
    {
        _wormBodies.ForEach(wB => wB.UpdateLineRender());

        if(Input.GetKey(KeyCode.Space) || Input.touchCount > 0)
        {
            IsDigging = true;
        }
        else
        {
            IsDigging = false;
            _angleDig = 30f;
        }
        SetForce(IsDigging);
    }

    private void FixedUpdate()
    {
        //Move(IsDigging);
        Rigidbody.AddForce(currentForce);

        /*if (IsDigging)
        {
            Dig();
        }
        Move();*/
    }

    void SetForce(bool _isDigging)
    {
        currentForce = Vector2.right * Speed;
        if (_isDigging)
        {
            _angleDig = Mathf.Clamp(_angleDig, 0, 90);
            Rigidbody.MoveRotation(Quaternion.AngleAxis(-_angleDig, Vector3.forward));
            _angleDig += 10f;
            currentForce += Vector2.down * DiggingForce;
        }
    }

    void Dig()
    {
        _angleDig = Mathf.Clamp(_angleDig, 0, 90);
        Rigidbody.MoveRotation(Quaternion.AngleAxis(-_angleDig, Vector3.forward));
       
        Rigidbody.AddForce(Vector2.down * DiggingForce);
        _angleDig += 10f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Death"))
        {
            
            IsDead = true;

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BlocPoolerTrigger"))
        {
            Speed = Mathf.Min(Speed + 0.1f, 8);
        }
    }
}
