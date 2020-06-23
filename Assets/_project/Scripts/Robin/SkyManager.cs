using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class SkyManager : MonoBehaviour
{
    public GameObject CenterUniverse;
    public MeshRenderer Sky;
    public float DurationOfCycle;
    private float _chronoCycle = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _chronoCycle += Time.deltaTime;
        float scaleTime = _chronoCycle / DurationOfCycle;

        if (_chronoCycle == DurationOfCycle)
        {
            _chronoCycle = 0f;
        }
        float timeOfDay = Mathf.Lerp(0f, 1f, Mathf.PingPong(scaleTime, 1f));
        //Debug.Log("TimeOfDay " + timeOfDay);
        Sky.material.SetFloat("_TimeOfDay", timeOfDay);
        
    }
}
