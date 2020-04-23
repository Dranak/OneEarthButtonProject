using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine.Utility;
using Cinemachine;

public class CamFitter : Behaviour
{
    [SerializeField]
    CinemachineVirtualCamera cineCam;
    void Awake()
    {
        var cineCamXOffset = cineCam.GetCinemachineComponent<CinemachineComposer>().m_ScreenX;
        //cineCamXOffset.
    }
}
