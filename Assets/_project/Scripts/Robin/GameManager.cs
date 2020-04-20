using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Player Player;
    public GameObject DeathCanvas;
    // Start is called before the first frame update
    void Start()
    {
        Instance = Instance ?? this;
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Player.IsDead)
        {
            Time.timeScale = 0f;
            DeathCanvas.gameObject.SetActive(true);
        }
    }
}
