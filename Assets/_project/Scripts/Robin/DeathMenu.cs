using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathMenu : MonoBehaviour
{
    public Image FillingImage;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI CurrentLevelText;
    public TextMeshProUGUI NextLevelText;
    public GameObject NormalPanel;
    public GameObject LevelUpPanel;
    private DeathState _state;

    private float _currentFilling = 0;
    private float _newFilling = 0;



    private void OnEnable()
    {
        SetState(DeathState.Start);
        Debug.Log("WESH ALORS");


    }

    private void Update()
    {
        switch (_state)
        {
            case DeathState.Start:



                break;
            case DeathState.LerpBar:
                break;
            case DeathState.LevelUp:
                break;
            case DeathState.NextBar:
                break;
        }
    }

    //void SetupFilling(float currentFilling)
    //{
    //    FillingImage.fillAmount = currentFilling;
    //    _newFilling = (GameManager.Instance.Player.CurrentXp) / GameManager.Instance.Player.NeededXp;
    //    Debug.Log("currentFilling: " + FillingImage.fillAmount + "newFilling: " + newFilling);

    //    float startTime;
    //    if (newFilling < 1f)
    //    {
    //        startTime = Time.unscaledTime;
    //        StartCoroutine(LerpThreading(currentFilling, newFilling, startTime, 3f));
    //    }
    //    else
    //    {
    //        startTime = Time.unscaledTime;
    //        StartCoroutine(LerpThreading(currentFilling, 1f, startTime, 3f));
    //        //Put here VFX Levelup
    //        NormalPanel.SetActive(false);
    //        LevelUpPanel.SetActive(true);
    //        StartCoroutine(WaitFor());
    //        NormalPanel.SetActive(true);
    //        LevelUpPanel.SetActive(false);

    //        CurrentLevelText.text = (GameManager.Instance.Player.CurrentLevelPlayer + 1).ToString();

    //        NextLevelText.text = (GameManager.Instance.Player.CurrentLevelPlayer + 2).ToString();

    //        startTime = Time.unscaledTime;
    //        StartCoroutine(LerpThreading(0, newFilling - 1f, startTime, 3f));
    //    }

    //}

    IEnumerator LerpThreading(float from, float to, float startTime, float duration,bool levelUp =false)
    {

        float scaleTime = 0f;
        while (scaleTime < 1f)
        {
            //Debug.Log("startTime: "+startTime + "scaleTime: "+scaleTime + "Time.time" + Time.time);
            scaleTime = (Time.unscaledTime - startTime) / duration;
            //Debug.Log("startTime: " + startTime + "scaleTime: " + scaleTime + "Time.time" + Time.unscaledTime);
            FillingImage.fillAmount = Mathf.Lerp(from, to, scaleTime);
            //  Debug.Log("LerpATM: "+ FillingImage.fillAmount + "scaleTime: " + scaleTime);
            yield return null;
        }

        if(levelUp)
         SetState(DeathState.LevelUp);

    }



    public void ButtonLevelUp()
    {
        SetState(DeathState.NextBar);
    }

    IEnumerator LerpPoint(int score, float startTime, float duration)
    {
        float scaleTime = 0f;
        while (scaleTime < 1f)
        {
            Debug.Log("OOF " + score);
            scaleTime = (Time.unscaledTime - startTime) / duration;

            ScoreText.text = Mathf.Lerp(0, score, scaleTime).ToString();

            yield return null;
        }
    }

    void LerpBarNormal()
    {
        //startTime = Time.unscaledTime;
        if (_newFilling < 1f)
            StartCoroutine(LerpThreading(_currentFilling, _newFilling, Time.unscaledTime, 3f));
        else
        {
            StartCoroutine(LerpThreading(_currentFilling, 1f, Time.unscaledTime, 3f,true));
          
        }
    }

    void LerpBarLevelUp()
    {
        CurrentLevelText.text = (GameManager.Instance.Player.CurrentLevelPlayer + 1).ToString();

        NextLevelText.text = (GameManager.Instance.Player.CurrentLevelPlayer + 2).ToString();


        StartCoroutine(LerpThreading(0, _newFilling - 1f, Time.unscaledTime, 3f));

        ++GameManager.Instance.Player.CurrentLevelPlayer;
        GameManager.Instance.Player.CurrentXp = GameManager.Instance.Player.CurrentXp - GameManager.Instance.Player.NeededXp;
        GameManager.Instance.Player.LoadDataFromFile();

        GameManager.Instance.Player.SaveData();
    }

    void SetState(DeathState state)
    {
        _state = state;
        switch (_state)
        {
            case DeathState.Start:
                StartCoroutine(LerpPoint(GameManager.Instance.Player.Score, Time.unscaledTime, 2f));

                CurrentLevelText.text = (GameManager.Instance.Player.CurrentLevelPlayer).ToString();

                NextLevelText.text = (GameManager.Instance.Player.CurrentLevelPlayer + 1).ToString();

                PlayerPrefs.SetInt("HighScore", GameManager.Instance.Player.HighScore);

                _currentFilling = GameManager.Instance.Player.CurrentXp / GameManager.Instance.Player.NeededXp;
                GameManager.Instance.Player.CurrentXp += GameManager.Instance.Player.Score;
                _newFilling = GameManager.Instance.Player.CurrentXp / GameManager.Instance.Player.NeededXp;
                SetState(DeathState.LerpBar);

                break;
            case DeathState.LerpBar:
                LerpBarNormal();
                break;
            case DeathState.LevelUp:
                NormalPanel.SetActive(false);
                LevelUpPanel.SetActive(true);

                break;
            case DeathState.NextBar:
               
                NormalPanel.SetActive(true);
                LevelUpPanel.SetActive(false);
                LerpBarLevelUp();
                break;
        }
    }

}


public enum DeathState
{
    Start,
    LerpBar,
    LevelUp,
    NextBar



}