using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;

    public MainMenu MainMenu;
    public GameMenu GameMenu;
    public DeathMenu DeathMenu;
    public DressingManager DressingMenu;
    public EventSystem eventSystem;
    string thisSceneName;

    public int SessionGameCount { get; set; } = 0;
    public int BestSessionScore { get; set; } = 0;
    public int SessionStrikesTotal { get; set; } = 0; // total ammount of eggshells strike during session
    public List<SkinData> allSkins;
    public bool isSkinPreviewsSet = false;

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
        if (GameMenu.gameObject.activeInHierarchy)
        {
            GameMenu.gameObject.SetActive(false);
        }
        if (MainMenu.gameObject.activeInHierarchy)
        {
            GameMenu.gameObject.SetActive(false);
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

    public void Quit()
    {
        Application.Quit();
    }

    public void Death()
    {
        if (GameMenu.gameObject.activeInHierarchy)
        {
            GameMenu.gameObject.SetActive(false);
        }
        DeathMenu.gameObject.SetActive(true);
    }

   

    public void AudioMenu()
    {

        //Time.timeScale = 0;
    }

    public void ToDressingMenu(int menuID)
    {
        DressingMenu.gameObject.SetActive(true);
        DressingMenu.SwitchMenuPanel(menuID);
    }
}
