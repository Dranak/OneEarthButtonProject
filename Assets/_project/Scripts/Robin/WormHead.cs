using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WormHead : WormBody
{
    public int NumberOfParts;
    public float OffsetBodyPart = 5f;
    public float Speed;
    public float DiggingForce;
    public WormBody PrefabWormBody;
    public float _angleDig = 30f;
    private List<WormBody> _wormBodies ;
    private bool IsDigging = false;

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

    // Update is called once per frame
    void Update()
    {
        _wormBodies.ForEach(wB => wB.UpdateLineRender());

        if(Input.GetKey(KeyCode.Space))
        {
            IsDigging = true;
           // Dig();
        }
        else
        {
            IsDigging = false;
            _angleDig = 30f;
        }
    }


    private void FixedUpdate()
    {
        if(IsDigging)
        {
            Dig();
        }

        Move();
    }

    void Move()
    {
        Rigidbody.AddForce(Vector2.right  * Speed);
       
    }


    void Dig()
    {
         
        _angleDig = Mathf.Clamp(_angleDig, 0, 90);
        Rigidbody.MoveRotation(Quaternion.AngleAxis(-_angleDig, Vector3.forward));
       
        Rigidbody.AddForce(transform.right* DiggingForce, ForceMode2D.Force);
        _angleDig += 10f;
    }


}
