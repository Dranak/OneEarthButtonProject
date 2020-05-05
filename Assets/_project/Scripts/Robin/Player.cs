using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public WormHead WormHead;
    public bool IsDead { get { return WormHead.IsDead; }}

    public float SpeedUnDig;
    public float SpeedDig;

    [Range(0, 360)]
    public float MinAngleDig;
    [Range(0, 360)]
    public float MaxAngleDig;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    void SetupMassBySpeed()
    {
        WormHead.Rigidbody.mass = 1f;
    }
}
