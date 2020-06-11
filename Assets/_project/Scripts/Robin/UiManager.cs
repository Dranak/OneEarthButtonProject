using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;
    public MainMenu MainMenu;
    public GameMenu GameMenu;
    public DeathMenu DeathMenu;

    // Start is called before the first frame update
    private void Awake()
    {
        Instance = Instance ?? this;
        DontDestroyOnLoad(this);

    }


    public void LoadSceneByName(string name)
    {
        SceneManager.LoadScene(name);
    }


    public void Replay()
    {
        MainMenu.gameObject.SetActive(false);
        if (DeathMenu.gameObject.activeInHierarchy)
        {
            DeathMenu.gameObject.SetActive(false);
        }


        MainMenu.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    

    }

    public void GoMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.Instance.SetState(State.InMenu);
        if (DeathMenu.gameObject.activeInHierarchy)
        {
            DeathMenu.gameObject.SetActive(false);
        }


        MainMenu.gameObject.SetActive(true);
        MainMenu.PlayButton.interactable = true;
        MainMenu.SkinButton.interactable = true;
        MainMenu.SettingButton.interactable = true;

    }


    public void Death()
    {

        DeathMenu.gameObject.SetActive(true);
    }










    public void Pause()
    {
        Time.timeScale = 0;
    }

    public void UnPause()
    {
        Time.timeScale = 1;
    }

    public void AudioMenu()
    {

        //Time.timeScale = 0;
    }
}
