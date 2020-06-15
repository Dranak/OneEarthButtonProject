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
    string thisSceneName;

    public int SessionGameCount { get; set; } = 0;
    public int BestSessionScore { get; set; } = 0;
    public int SessionStrikesTotal { get; set; } = 0; // total ammount of eggshells strike during session

    // Start is called before the first frame update
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            thisSceneName = SceneManager.GetActiveScene().name;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }


    public void LoadSceneByName(string name)
    {
        SceneManager.LoadScene(name);
    }


    public void Replay()
    {
        if (DeathMenu.gameObject.activeInHierarchy)
        {
            DeathMenu.gameObject.SetActive(false);
        }

        MainMenu.group.alpha = 0; // hide main menu
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(thisSceneName);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (thisSceneName == scene.name)
            MainMenu.Play();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void GoMainMenu()
    {
        MainMenu.group.alpha = 1; // unhide main menu

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.Instance.SetState(State.InMenu);
        if (DeathMenu.gameObject.activeInHierarchy)
        {
            DeathMenu.gameObject.SetActive(false);
        }

        //MainMenu.gameObject.SetActive(true);
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
