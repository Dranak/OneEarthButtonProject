using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathMenu:MonoBehaviour
{
    public Image FillingImage;
    public TextMeshProUGUI CurrentLevelText;
    public TextMeshProUGUI NextLevelText;
    public GameObject NormalPanel;
    public GameObject LevelUpPanel;
    private bool _backToNormalPanel = true;

    private void OnEnable()
    {
       
        PlayerPrefs.SetInt("HighScore", GameManager.Instance.Player.HighScore);
        float lastxp = GameManager.Instance.Player.CurrentXp;
        GameManager.Instance.Player.CurrentXp += GameManager.Instance.Player.Score;
        Debug.Log("XP: " + GameManager.Instance.Player.CurrentXp);
        Debug.Log("NeededXp: " + GameManager.Instance.Player.NeededXp);
        SetupFilling(lastxp / GameManager.Instance.Player.NeededXp);
       
        if(GameManager.Instance.Player.CurrentXp > GameManager.Instance.Player.NeededXp)
        {
            ++GameManager.Instance.Player.CurrentLevelPlayer;
            GameManager.Instance.Player.CurrentXp = GameManager.Instance.Player.CurrentXp - GameManager.Instance.Player.NeededXp;
            GameManager.Instance.Player.LoadDataFromFile();
        }
        GameManager.Instance.Player.SaveData();
    }

    void SetupFilling(float currentFilling)
    {
        FillingImage.fillAmount = currentFilling;
        float newFilling = (GameManager.Instance.Player.CurrentXp) / GameManager.Instance.Player.NeededXp;
        Debug.Log("currentFilling: " + FillingImage.fillAmount + "newFilling: "+newFilling);

        float startTime;
        if (newFilling < 1f)
        {
            startTime = Time.unscaledTime;
           StartCoroutine( LerpThreading(currentFilling, newFilling, startTime, 3f));
        }
        else
        {
            startTime = Time.unscaledTime;
            StartCoroutine(LerpThreading(currentFilling, 1f, startTime, 3f));
            //Put here VFX Levelup
            NormalPanel.SetActive(false);
            LevelUpPanel.SetActive(true);

            CurrentLevelText.text = (GameManager.Instance.Player.CurrentLevelPlayer + 1).ToString();

            NextLevelText.text = (GameManager.Instance.Player.CurrentLevelPlayer + 2).ToString();

            startTime = Time.unscaledTime;
            StartCoroutine(LerpThreading(0, newFilling - 1f, startTime, 3f));
        }
        
    }

    IEnumerator LerpThreading(float from,float to,float startTime, float duration)
    {
        
        float scaleTime = 0f;
        while (scaleTime < 1f)
        {
            //Debug.Log("startTime: "+startTime + "scaleTime: "+scaleTime + "Time.time" + Time.time);
            scaleTime = (Time.unscaledTime - startTime) / duration;
            Debug.Log("startTime: " + startTime + "scaleTime: " + scaleTime + "Time.time" + Time.unscaledTime);
            FillingImage.fillAmount = Mathf.Lerp(from, to, scaleTime);
            //  Debug.Log("LerpATM: "+ FillingImage.fillAmount + "scaleTime: " + scaleTime);
            yield return null;
        }
     


    }


    IEnumerator WaitFor()
    {
        while(_backToNormalPanel)
        {
            yield return null;
        }
    }

    public void ButtonLevelUp()
    {
        _backToNormalPanel = false;
    }

}