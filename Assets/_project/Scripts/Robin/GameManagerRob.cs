using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerRob : MonoBehaviour
{
    public static GameManagerRob Instance;

    public Player Player;
    public GameObject DeathCanvas;
    [HideInInspector] public Camera camera;
    [HideInInspector] public float cameraHalfWidth;
    [SerializeField] Cinemachine.CinemachineVirtualCamera VCam;
    [SerializeField] Transform boundToFollow;

    private void Awake()
    {
      
        camera = Camera.main;
        var cameraHalfWidth = camera.orthographicSize * camera.aspect;
        var camUnitsWidth = cameraHalfWidth * 2;
        var vCamXOffset = (camUnitsWidth - 15) / camUnitsWidth;
        VCam.GetCinemachineComponent<Cinemachine.CinemachineFramingTransposer>().m_ScreenX = vCamXOffset;
        //boundToFollow.localPosition = Vector3.right * 15 + Vector3.left * camera.orthographicSize * Instance.camera.aspect; // LEGACY
    }

    void Start()
    {
        Instance = Instance ?? this;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if(Player.IsDead)
        {
            Time.timeScale = 0f;
            DeathCanvas.gameObject.SetActive(true);
        }
    }
}
