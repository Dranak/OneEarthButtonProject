using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject PauseUI;
    public GameObject AudioUI;

    public void LoadSceneByName(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Pause()
    {
        PauseUI.SetActive(true);
        Time.timeScale = 0; 
    }

    public void UnPause()
    {
        PauseUI.SetActive(false);
        Time.timeScale = 1;
    }

    public void AudioMenu()
    {
        AudioUI.SetActive(true);
        //Time.timeScale = 0;
    }

}
