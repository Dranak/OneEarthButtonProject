using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class BlinkingLight : MonoBehaviour
{
    public float DurationBlink;
    public Light2D Light;
    public float MinIntensity;
    public float MaxIntensity;
    private float _chronoBlink;

    // Start is called before the first frame update
    void Start()
    {
        Light.intensity = MinIntensity;
        Blink();

    }



    // Update is called once per frame
    void Update()
    {

    }

    void Blink()
    {
        _chronoBlink += Time.deltaTime;
        if (_chronoBlink >= DurationBlink)
        {
            _chronoBlink = 0f;
        }
        float scaledTime = _chronoBlink / DurationBlink;
        Light.intensity = Mathf.Lerp(MinIntensity, MaxIntensity, Mathf.PingPong(scaledTime,1f));
    }

}
