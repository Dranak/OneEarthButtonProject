using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowRob : MonoBehaviour
{
    // Start is called before the first frame update
    public float Speed;
    public Rigidbody2D  Target;
    private float screenWidth;

    private void Start()
    {
        screenWidth = Camera.main.orthographicSize;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distance = Vector2.Distance(Camera.main.transform.position, Target.position);
        var currentTarget = new Vector3(Target.position.x + screenWidth, 0, -10);
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, (Vector3)Target.position + Vector3.forward*-10, Speed* Time.fixedDeltaTime * distance);
    }
}
