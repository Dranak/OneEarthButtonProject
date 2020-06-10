using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button PlayButton;
    public Button SkinButton;
    public Button SettingButton;

    public void Play()
    {
        Debug.Log("Play");
        GameManager.Instance.State = State.Play;
      
        UiManager.Instance.GameMenu.gameObject.SetActive(true);
        
        PlayButton.interactable = false;
        SkinButton.interactable = false;
        SettingButton.interactable = false;
    }

    private void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        PlayButton.interactable = true;
        SkinButton.interactable = true;
        SettingButton.interactable = true;
    }

}