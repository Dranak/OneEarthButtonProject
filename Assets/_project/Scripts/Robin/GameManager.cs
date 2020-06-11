using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public State State;
    public Player Player;
   
   
    [HideInInspector] public Camera camera;
    [HideInInspector] public float cameraHalfWidth;
    [SerializeField] Cinemachine.CinemachineVirtualCamera VCam;
    [HideInInspector] public float savedStartingOffset;
    [SerializeField] GameObject unPoolerLeft, poolerRight;
    [SerializeField] SpriteAtlas backObjAtlas;

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
        if(State == State.Play)
        {
            Time.timeScale = 1f;
        }
        else
        {
            State = State.InMenu;
            Time.timeScale = 0f;
        }
        
        
    }

    void Update()
    {
        switch (State)
        {
            case State.Play:
               
                break;
            case State.InMenu:
               
                break;
            case State.Dead:
            
                break;
            case State.Pause:
                break;
        }

    }

    public void SetState(State state)
    {
        State = state;
        switch (state)
        {
            case State.Play:
                Time.timeScale = 1f;
                UiManager.Instance.MainMenu.PlayButton.interactable = false;
                UiManager.Instance.MainMenu.SkinButton.interactable = false;
                UiManager.Instance.MainMenu.SettingButton.interactable = false;
                break;
            case State.InMenu:
                Time.timeScale = 0f;
                break;
            case State.Dead:
              
                Time.timeScale = 0f;
                UiManager.Instance.Death();
                break;
            case State.Pause:
                break;
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


public enum State
{
    Play,
    InMenu,
    Dead,
    Pause,

}