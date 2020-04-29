using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Player Player;
    public GameObject DeathCanvas;
    [HideInInspector] public Camera camera;
    [SerializeField] Transform boundToFollow;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(this);

        camera = Camera.main;
        boundToFollow.localPosition = Vector3.right * 15 + Vector3.left * camera.orthographicSize * Instance.camera.aspect;
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
