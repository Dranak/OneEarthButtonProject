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
    private bool _state = false;

    // Start is called before the first frame update
    void Start()
    {
        Light.intensity = MinIntensity;
        

    }



    // Update is called once per frame
    void Update()
    {
        Blink();
    }

    void Blink()
    {
        _chronoBlink += Time.deltaTime;
        if (_chronoBlink >= DurationBlink)
        {
            _chronoBlink = 0f;
            _state = !_state;
        }
        float scaledTime = _chronoBlink / DurationBlink;

        if(!_state)
        {
            Light.intensity = Mathf.Lerp(MinIntensity, MaxIntensity, scaledTime);
        }
        else
        {
            Light.intensity = Mathf.Lerp(MaxIntensity, MinIntensity, scaledTime);
        }
       
      
    }

}
