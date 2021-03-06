﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Cinemachine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Player Player;

    [HideInInspector] public Camera camera;
    [HideInInspector] public float cameraHalfWidth;
    [SerializeField] CinemachineVirtualCamera VCam;
    CinemachineFramingTransposer framingTransposer;
    [HideInInspector] public float savedStartingOffset;
    [SerializeField] GameObject unPoolerLeft, poolerRight;
    [SerializeField] SpriteAtlas backObjAtlas;

    [Header ("Sound Options")]
    public MusicManager MusicManager;
    public AudioSource environmentNoise;
    public float normalEnvirVol = 0.3f;
    public float muffledEnvirVol = 0.15f;
    public AudioSource cameraSoundSource;
    public AudioClip deathEffect;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            environmentNoise.volume = normalEnvirVol;
        }
        else
            Destroy(gameObject);

        GameManagerSetup();
    }

    // starting values
    public void GameManagerSetup()
    {
        camera = Camera.main;

        float twoRatioBias = 2 - camera.aspect;
        VCam.m_Lens.OrthographicSize += Mathf.Sign(twoRatioBias) * Mathf.Pow(twoRatioBias, 2) * 4.5f; // resize cam based on ratio
        framingTransposer = VCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        framingTransposer.m_ScreenY += twoRatioBias * 0.15f;

        // camera offset saving
        cameraHalfWidth = VCam.m_Lens.OrthographicSize * camera.aspect;
        var camUnitsWidth = cameraHalfWidth * 2;
        savedStartingOffset = (camUnitsWidth - 15) / camUnitsWidth; // offset for seeing 15 units after the worm's head
    }

    public IEnumerator cameraDecentering()
    {
        var endWormPos = BlocManager.Instance.startingBlocMin - 15; // position the worm is at when the first bloc becomes visible (seeing 15 units after the worm head at the end of transition)
        var wormHeadT = Player.WormHead.transform;

        while (wormHeadT)
        {
            var newScreenX = Mathf.Lerp(0.5f, savedStartingOffset, wormHeadT.position.x / endWormPos);
            if (newScreenX <= savedStartingOffset)
                break;
            framingTransposer.m_ScreenX = newScreenX;
            yield return new WaitForFixedUpdate();
        }
        framingTransposer.m_ScreenX = savedStartingOffset;
    }

    void Start()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        if (UiManager.Instance.State == State.Play)
        {
            Time.timeScale = 1f;
        }
        else
        {
            UiManager.Instance.State = State.InMenu;
            Time.timeScale = 0f;
        }
    }

    private void OnSceneUnloaded(Scene current)
    {
        PlayerPrefs.SetInt("HighScore", Player.HighScore);
    }

    void Update()
    {
        switch (UiManager.Instance.State)
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
        UiManager.Instance.State = state;
        switch (state)
        {
            case State.Play:
                Time.timeScale = 1f;
                UiManager.Instance.MainMenu.group.interactable = false;
                environmentNoise.volume = normalEnvirVol;
                if (Player.WormHead.dirtAudioSource.isVirtual)
                    Player.WormHead.dirtAudioSource.UnPause();
                else
                    Player.WormHead.dirtAudioSource.Play();
                break;
            case State.InMenu:
                Time.timeScale = 0f;
                UiManager.Instance.MainMenu.group.interactable = true;
                environmentNoise.volume = normalEnvirVol;
                break;
            case State.Dead:
                Player.StreakEggShell = 0;
                Time.timeScale = 0f;
                if (UiManager.Instance.BestSessionScore < Player.Score)
                    UiManager.Instance.BestSessionScore = Player.Score;
                UiManager.Instance.Death();
                cameraSoundSource.PlayOneShot(deathEffect);
                environmentNoise.volume = 0f;
                Player.WormHead.dirtAudioSource.Stop();
                break;
            case State.Pause:
                Time.timeScale = 0f;
                environmentNoise.volume = muffledEnvirVol;
                Player.WormHead.dirtAudioSource.Pause();
                break;
        }
        MusicManager.SwitchMusic(state);
    }

    public void BGWPSetup()
    {
        /*
        var WPpool = BlocManager.Instance.wpPool;
        // DISABLES EXTRA USELESS BACKGROUND WP DEPENDING ON ASPECT RATIO
        var lastWpLeftBoundPos = WPpool[WPpool.Count - 1].transform.position.x - 3;
        // comparing the WP right bound to the camera left bound
        if (cameraHalfWidth < lastWpLeftBoundPos)
        {
             WPpool[WPpool.Count - 1].SetActive(false);
        }
        */
        // enables camera unpooler bounds
        unPoolerLeft.SetActive(true);
        poolerRight.SetActive(true);
    }
}
