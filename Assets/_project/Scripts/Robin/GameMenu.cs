using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class GameMenu:MonoBehaviour
{
    public TextMeshProUGUI ScoreText;
    public PauseMenu PauseMenu;


    public void Pause()
    {
        PauseMenu.gameObject.SetActive(true);
        GameManager.Instance.SetState(State.InMenu);

    }

    public void UnPause()
    {
        PauseMenu.gameObject.SetActive(false);
        GameManager.Instance.SetState(State.Play);
    }

    public void ReplayFromPause()
    {
        gameObject.gameObject.SetActive(false);
        PauseMenu.gameObject.SetActive(false);
        UiManager.Instance.Replay();
    }
}