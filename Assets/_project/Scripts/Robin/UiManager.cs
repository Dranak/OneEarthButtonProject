using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;

    public State State;
    public MainMenu MainMenu;
    public GameMenu GameMenu;
    public DeathMenu DeathMenu;
    public DressingManager DressingMenu;
    public AudioManager AudioMenu;
    public EventSystem eventSystem;
    string thisSceneName;

    public int SessionGameCount { get; set; } = 0;
    public int BestSessionScore { get; set; } = 0;
    public int SessionStrikesTotal { get; set; } = 0; // total ammount of eggshells strike during session
    public List<SkinData> allSkins;
    [HideInInspector] public bool isSkinPreviewsSet = false;

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
        State = State.Play;
        SceneManager.LoadSceneAsync(thisSceneName);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (thisSceneName == scene.name)
            GameMenu.ResetPauseButton();
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

    public void ToAudioMenu()
    {
        AudioMenu.gameObject.SetActive(true);
    }
    public void ToDressingMenu(int menuID)
    {
        DressingMenu.gameObject.SetActive(true);
        DressingMenu.SwitchMenuPanel(menuID);
    }
    public void BackToMainMenu(in GameObject panel) // back to main menu from a sub-menu in the main menu, or the pause menu
    {
        panel.SetActive(false); // deactivate menu panel
        
        // unmute environment noise
        var environmentNoise = GameManager.Instance.muffledEnvirVol;
        if (State == State.InMenu)
            environmentNoise = GameManager.Instance.normalEnvirVol;
        GameManager.Instance.environmentNoise.volume = environmentNoise;
    }
}

public enum State
{
    InMenu,
    Play,
    Dead,
    Pause
}
