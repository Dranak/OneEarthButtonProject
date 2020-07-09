using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupScore : MonoBehaviour
{

    GameMenu _gameMenu;
    WormHead _worm;
    public float Offset = 1f;
    public float TimeLerp;
    public float TimeStacking;
    float _chronoStacking;
    float _chronoLerp;
  public  TextMeshProUGUI ScoreText;
    GameObject _firstParent;
    public int Value { get; set; } = 0;
    private int _lastValue = -1;
    public bool IsAvailable = true;

    // Start is called before the first frame update
    void Start()
    {
        _gameMenu = UiManager.Instance.GameMenu;
        _worm = GameManager.Instance.Player.WormHead;
      // = new Vector3(_worm.transform.position.x, _worm.transform.position.y + Offset, _worm.transform.position.z); 



    }

    // Update is called once per frame
    void Update()
    {
        //if(IsAvailable)
        //{
            FollowWorm();
        
     

        //FinishStacking();
        //TimeToGo();
      
    }


    void FollowWorm()
    {
        Vector3 newPos = new Vector3(_worm.transform.position.x, _worm.transform.position.y + Offset, _worm.transform.position.z);
        ScoreText.transform.position = newPos;
    }

    void TimeToGo()
    {
        if(_chronoStacking >= TimeStacking)
        {
            IsAvailable = false;
            ScoreText.transform.SetParent(_gameMenu.transform);
            StartCoroutine(LerpThreadingPosition(ScoreText.transform.position, _gameMenu.ScoreText.transform.position, Time.time, TimeLerp));
        }
     

    }

    IEnumerator LerpThreadingPosition(Vector3 from, Vector3 to, float startTime, float duration)
    {

        float scaleTime = 0f;
        while (scaleTime < 1f)
        {
            //Debug.Log("startTime: "+startTime + "scaleTime: "+scaleTime + "Time.time" + Time.time);
            scaleTime = (Time.unscaledTime - startTime) / duration;
            //Debug.Log("startTime: " + startTime + "scaleTime: " + scaleTime + "Time.time" + Time.unscaledTime);
            ScoreText.transform.position = Vector3.Lerp(from, to, scaleTime);
            //  Debug.Log("LerpATM: "+ FillingImage.fillAmount + "scaleTime: " + scaleTime);
            yield return null;
        }

        AddPoint();


    }

    public bool Stacking(int value)
    {
        if(IsAvailable)
        {
            _lastValue = Value;
            Value += value;
            ScoreText.text = "+" + Value;
        }
        return IsAvailable;
     
    }

    void FinishStacking()
    {
        if (_lastValue == -1)
            return;

        if(_lastValue == Value)
        {
            _chronoStacking += Time.deltaTime;
        }
       else
        {
            _chronoStacking = 0f;
        }

    }

    void AddPoint()
    {
        int actualScore = GameManager.Instance.Player.Score;
        ScoreText.text = "";
        ScoreText.transform.SetParent(transform);
        ScoreText.transform.position = _worm.transform.position;

        IsAvailable = true;
        StartCoroutine(LerpPoint(actualScore, actualScore + Value,Time.time ,.2f));

    }


    IEnumerator LerpPoint(int from, int score, float startTime, float duration)
    {
        float scaleTime = 0f;
        while (scaleTime < 1f)
        {

            scaleTime = (Time.unscaledTime - startTime) / duration;

            ScoreText.text = ((int)Mathf.Lerp(from, score, scaleTime)).ToString();

            yield return null;
        }
        GameManager.Instance.Player.Score += Value;
        
      }

}
