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


        if (newFilling < 1f)
        {
            LerpThreading(currentFilling, newFilling, Time.time, 3f);
        }
        else
        {
            LerpThreading(currentFilling, 1f, Time.time, 3f);
            //Put here VFX Levelup

            CurrentLevelText.text = (GameManager.Instance.Player.CurrentLevelPlayer+ 1).ToString();
            NextLevelText.text = (GameManager.Instance.Player.CurrentLevelPlayer + 2).ToString();
            LerpThreading(0, newFilling - 1f, Time.time, 3f);
        }
        
    }

    IEnumerator LerpThreading(float from,float to,float startTime, float duration)
    {
      
        float percentageLerp = 0f;
        while (percentageLerp < 1f)
        {
            percentageLerp = (Time.time - startTime) / duration;
            FillingImage.fillAmount = Mathf.Lerp(from, to, percentageLerp);
            Debug.Log("LerpATM: "+ FillingImage.fillAmount + "percentageLerp: "+ percentageLerp);
            yield return null;
        }
     


    }
}