using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Start is called before the first frame update
    public float Speed;
    public GameObject Target;
    private float screenWidth;

    private void Start()
    {
        screenWidth = Camera.main.orthographicSize;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float distance = Vector2.Distance(Camera.main.transform.position, Target.transform.position);
        var currentTarget = new Vector3(Target.transform.position.x + screenWidth, 0, -10);
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, currentTarget + Vector3.forward*-10, Speed * Time.deltaTime * distance);
    }
}
