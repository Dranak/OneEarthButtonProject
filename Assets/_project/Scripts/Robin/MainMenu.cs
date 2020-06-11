using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button PlayButton;
    public Button SkinButton;
    public Button SettingButton;

    public void Start()
    {
        if(GameManager.Instance.State == State.Play)
        {
            gameObject.SetActive(false);
            Play();
        }
        
    }

    public void Play()
    {
        Debug.Log("Play");
        GameManager.Instance.SetState(State.Play);
      
        UiManager.Instance.GameMenu.gameObject.SetActive(true);
        
        PlayButton.interactable = false;
        SkinButton.interactable = false;
        SettingButton.interactable = false;

        StartCoroutine(GameManager.Instance.cameraDecentering());
    }

  



    
}