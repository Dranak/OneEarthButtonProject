using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Player Player;
    public GameObject DeathCanvas;
    [HideInInspector] public Camera camera;
    [HideInInspector] public float cameraHalfWidth;
    [SerializeField] Cinemachine.CinemachineVirtualCamera VCam;
    [HideInInspector] public float savedStartingOffset;
    [SerializeField] GameObject unPoolerLeft, poolerRight;

    private void Awake()
    {
        Instance = Instance ?? this; // Nice compact way ! (Rob)
        camera = Camera.main;
        cameraHalfWidth = camera.orthographicSize * camera.aspect;
        var camUnitsWidth = cameraHalfWidth * 2;
        savedStartingOffset = (camUnitsWidth - 15) / camUnitsWidth; // offset for seeing 15 units after the worm's head
        VCam.GetCinemachineComponent<Cinemachine.CinemachineFramingTransposer>().m_ScreenX = savedStartingOffset;
    }

    void Start()
    {
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

    public void BGWPSetup()
    {
        // DISABLES EXTRA USELESS BACKGROUND WP DEPENDING ON ASPECT RATIO
        var firstWpRightBoundPos = BlocManager.Instance.wpPool[0].transform.position.x + 3;
        // comparing the WP right bound to the camera left bound
        if (firstWpRightBoundPos < savedStartingOffset * camera.aspect - cameraHalfWidth)
        {
            BlocManager.Instance.wpPool[0].SetActive(false);
        }

        // enables camera unpooler bounds
        unPoolerLeft.SetActive(true);
        poolerRight.SetActive(true);
    }
}
