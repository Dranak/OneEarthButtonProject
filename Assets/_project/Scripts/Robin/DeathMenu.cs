using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

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

    }

    private void Update()
    {
        switch (_state)
        {
            case DeathState.Start:



                break;
            case DeathState.LerpBar:
                Debug.Log("LerpBar");
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

    IEnumerator LerpThreading(float from, float to, float startTime, float duration, bool levelUp = false)
    {
        float scaleTime = 0f;
        while (scaleTime < 1f)
        {
            //Debug.Log("startTime: "+startTime + "scaleTime: "+scaleTime + "Time.time" + Time.time);
            scaleTime = (Time.unscaledTime - startTime) / duration;
            //Debug.Log("startTime: " + startTime + "scaleTime: " + scaleTime + "Time.time" + Time.unscaledTime);
            FillingImage.fillAmount = Mathf.Lerp(from, to, scaleTime);
            //  Debug.Log("LerpATM: "+ FillingImage.fillAmount + "scaleTime: " + scaleTime);
            yield return new WaitForEndOfFrame();
        }

        if (levelUp)
        {
            _newFilling =  GameManager.Instance.Player.CurrentXp / nextNeededXps[0]; // get next needed XP to determine the next filling
            nextNeededXps.RemoveAt(0); // remove that needed XP from the list of upcoming needed XPs
            SetState(DeathState.NextBar); // (DeathState.LevelUp for skins ==> backlog)
        }
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
        
            scaleTime = (Time.unscaledTime - startTime) / duration;

            ScoreText.text = ((int)Mathf.Lerp(0, score, scaleTime)).ToString();

            yield return null;
        }
    }

    void LerpBarNormal()
    {
        if (_newFilling < 1f)
            StartCoroutine(LerpThreading(_currentFilling, _newFilling, Time.unscaledTime, 3f));
        else
        {
            StartCoroutine(LerpThreading(_currentFilling, 1f, Time.unscaledTime, 3f, true));
        }
    }

    void LerpBarLevelUp()
    {
        CurrentLevelText.text = NextLevelText.text;

        NextLevelText.text = (Convert.ToInt32(CurrentLevelText.text) + 1).ToString();

        StartCoroutine(LerpThreading(0, _newFilling, Time.unscaledTime, 3f, _newFilling > 1));
    }

    List<float> nextNeededXps = new List<float>();
    void SetState(DeathState state)
    {
        _state = state;
        switch (_state)
        {
            case DeathState.Start:
                StartCoroutine(LerpPoint(GameManager.Instance.Player.Score, Time.unscaledTime, 2f));

                CurrentLevelText.text = (GameManager.Instance.Player.CurrentLevelPlayer).ToString();
                Debug.Log("CurrentLevelText : " + CurrentLevelText.text);

                NextLevelText.text = (GameManager.Instance.Player.CurrentLevelPlayer + 1).ToString();
                Debug.Log("NextLevelText : " + NextLevelText.text);

                PlayerPrefs.SetInt("HighScore", GameManager.Instance.Player.HighScore);

                // Score saving
                _currentFilling = GameManager.Instance.Player.CurrentXp / GameManager.Instance.Player.NeededXp;
                GameManager.Instance.Player.CurrentXp += GameManager.Instance.Player.Score;
                _newFilling = GameManager.Instance.Player.CurrentXp / GameManager.Instance.Player.NeededXp;

                var _fillingRound = _newFilling;
                while (_fillingRound > 1f)
                {
                    ++GameManager.Instance.Player.CurrentLevelPlayer;
                    GameManager.Instance.Player.LoadDataFromFile();
                    _fillingRound = GameManager.Instance.Player.CurrentXp / GameManager.Instance.Player.NeededXp;
                    nextNeededXps.Add(GameManager.Instance.Player.NeededXp);
                }

                GameManager.Instance.Player.SaveData();
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