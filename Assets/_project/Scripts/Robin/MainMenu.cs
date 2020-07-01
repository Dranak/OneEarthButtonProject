using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button PlayButton;
    public Button SkinButton;
    public Button SettingButton;
    public CanvasGroup group;
    public Canvas Canvas;

    public void Start()
    {

    }

    public void Play()
    {
        Debug.Log("Play");
        Canvas.renderMode = RenderMode.WorldSpace;
        GameManager.Instance.SetState(State.Play);
               
        UiManager.Instance.GameMenu.gameObject.SetActive(true);

        PlayButton.interactable = false;
        SkinButton.interactable = false;
        SettingButton.interactable = false;

        StartCoroutine(GameManager.Instance.cameraDecentering());
    }

    public void StatsMenu()
    {
        UiManager.Instance.ToDressingMenu(0);
    }
    public void SkinsMenu()
    {
        UiManager.Instance.ToDressingMenu(1);
    }
}
