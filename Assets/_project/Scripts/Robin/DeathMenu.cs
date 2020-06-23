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


    private void OnEnable()
    {
       
        PlayerPrefs.SetInt("HighScore", GameManager.Instance.Player.HighScore);
        int lastxp = GameManager.Instance.Player.CurrentXp;
        GameManager.Instance.Player.CurrentXp += GameManager.Instance.Player.Score;
        Debug.Log("NeededXp: " + GameManager.Instance.Player.NeededXp);
        SetupFilling(lastxp / GameManager.Instance.Player.NeededXp);
        Debug.Log("XP: " + GameManager.Instance.Player.CurrentXp);
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
        float newFilling = (GameManager.Instance.Player.CurrentXp * 100) / GameManager.Instance.Player.NeededXp;


        if (newFilling < 1f)
        {
            LerpThreading(currentFilling, newFilling, Time.time, 1f);
        }
        else
        {
            LerpThreading(currentFilling, 1f, Time.time, 1f);
            //Put here VFX Levelup

            CurrentLevelText.text = (GameManager.Instance.Player.CurrentLevelPlayer+ 1).ToString();
            NextLevelText.text = (GameManager.Instance.Player.CurrentLevelPlayer + 2).ToString();
            LerpThreading(0, newFilling - 1f, Time.time, 1f);
        }
        
    }

    IEnumerator LerpThreading(float from,float to,float startTime, float duration)
    {
      
        float percentageLerp = 0f;
        while (percentageLerp < 1f)
        {
            percentageLerp = (Time.time - startTime) / duration;
            FillingImage.fillAmount = Mathf.Lerp(from, to, percentageLerp);
            yield return null;
        }
     


    }
}