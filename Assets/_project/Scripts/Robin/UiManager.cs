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
        DontDestroyOnLoad(MainMenu.gameObject);
        DontDestroyOnLoad(MainMenu.gameObject);
        DontDestroyOnLoad(MainMenu.gameObject);
    }


    public void LoadSceneByName(string name)
    {
        SceneManager.LoadScene(name);
    }


    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        MainMenu.gameObject.SetActive(false);
        if (DeathMenu.gameObject.activeSelf)
            DeathMenu.gameObject.SetActive(false);

       
        MainMenu.Play();
       
    }

    public void GoMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.Instance.State = State.InMenu;
        if (DeathMenu.gameObject.activeSelf)
            DeathMenu.gameObject.SetActive(false);

        MainMenu.gameObject.SetActive(true);
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
