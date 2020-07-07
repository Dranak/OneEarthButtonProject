using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button PlayButton, SkinButton, StatsButton, SettingButton;
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

        StartCoroutine(GameManager.Instance.cameraDecentering());
    }

    public void StatsMenu()
    {
        UiManager.Instance.ToDressingMenu(0);
        GameManager.Instance.environmentNoise.volume = 0f;
    }
    public void SkinsMenu()
    {
        UiManager.Instance.ToDressingMenu(1);
        GameManager.Instance.environmentNoise.volume = 0f;
    }
    public void SettingsMenu()
    {
        UiManager.Instance.ToAudioMenu();
        GameManager.Instance.environmentNoise.volume = GameManager.Instance.muffledEnvirVol;
    }
}
