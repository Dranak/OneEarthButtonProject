using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu:MonoBehaviour
{
    public TextMeshProUGUI ScoreText;
    public PauseMenu PauseMenu;

    [SerializeField] Image pauseButtonImage;
    [SerializeField] Sprite pauseSprite, playSprite;

    public void PauseUnpause()
    {
        switch (UiManager.Instance.State)
        {
            case State.Play:
                Pause();
                break;
            case State.Pause:
                UnPause();
                break;
        }
    }
    void Pause()
    {
        PauseMenu.gameObject.SetActive(true);
        GameManager.Instance.SetState(State.Pause);
        pauseButtonImage.sprite = playSprite;
    }
    void UnPause()
    {
        PauseMenu.gameObject.SetActive(false);
        GameManager.Instance.SetState(State.Play);
        ResetPauseButton();
    }

    public void ResetPauseButton()
    {
        pauseButtonImage.sprite = pauseSprite;
    }

    public void ReplayFromPause()
    {
        HideGameMenus();
        UiManager.Instance.Replay();
    }

    public void MenuFromPause()
    {
        HideGameMenus();
        UiManager.Instance.GoMainMenu();
    }

    void HideGameMenus()
    {
        gameObject.SetActive(false);
        PauseMenu.gameObject.SetActive(false);
    }
}