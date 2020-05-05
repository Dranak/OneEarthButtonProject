using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WormHead : WormBody
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
    public LineRenderer Line;
    Vector2 currentForce = Vector2.zero;

    void Start()
    {
        _wormBodies = new List<WormBody>();

        for (int i = 0; i < NumberOfParts; ++i)
        {
            _wormBodies.Add(Instantiate(PrefabWormBody, transform.position + Vector3.left * (i + 1) * OffsetBodyPart, Quaternion.identity));

            if (i > 0)
                _wormBodies.Last().SetTarget(_wormBodies[i - 1]);
            else
                _wormBodies.Last().SetTarget(this);

        }
        _wormBodies.Last().Trail.enabled = true;
        _wormBodies.Last().GetComponent<SpriteRenderer>().enabled = true;
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
            _angleDig = 30f;
        }
        SetForce(IsDigging);

        Debug.Log(Rigidbody.velocity.x);
    }

    private void FixedUpdate()
    {
        Rigidbody.AddForce(currentForce);
    }

    void SetForce(bool _isDigging)
    {
        currentForce = Vector2.right * Speed;
        if (_isDigging)
        {
            _angleDig = Mathf.Clamp(_angleDig, 0, 90);
            _angleDig += 10f;
            currentForce += Vector2.down * DiggingForce;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Death"))
        {
            IsDead = true;
        }
    }
    void UpdateLineRenderer()
    {
        Line.SetPosition(0, transform.position);
        for (int index = 0; index < _wormBodies.Count; ++index)
        {
            Line.SetPosition(index + 1, _wormBodies[index].transform.position);
        }
    }
}
