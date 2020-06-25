using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class SkyManager : MonoBehaviour
{
    public GameObject CenterUniverse;
    public Light2D Sun;
    public Light2D Moon;

    public MeshRenderer Sky;
    public float DurationOfCycle;
    private float _chronoCycle = 0;
    private float _scaleTime = 0f;
    private float _timeOfDay = 0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _chronoCycle += Time.deltaTime;
        _scaleTime = _chronoCycle / DurationOfCycle;

        if (_chronoCycle == DurationOfCycle)
        {
            _chronoCycle = 0f;
        }
        _timeOfDay = Mathf.Lerp(0f, 1f, Mathf.PingPong(_scaleTime, 1f));
        //Debug.Log("TimeOfDay " + timeOfDay);
        Sky.material.SetFloat("_TimeOfDay", _timeOfDay);
        RotateStar();

    }

    void RotateStar()
    {
        Sun.transform.RotateAround(CenterUniverse.transform.position,Vector3.forward, Mathf.LerpAngle(0, 360, _timeOfDay));
        Moon.transform.RotateAround(CenterUniverse.transform.position,Vector3.forward, Mathf.LerpAngle(0, 360, _timeOfDay));

    }




}
