using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("General - Setup")]
    [Space]
    public WormHead WormHead;
    int dirtDepth = 9; // playable heigth
    public TextMeshProUGUI ScoreText;
    [Space]
    [Header("Scoring")]
    [Space]
    public float StepDistanceScoreIncrease;

    public int UndergroundBonus { get; set; } = 1;
    public int Score { get; set; } = 0;
   
    private int _lastMultiplicator = 1;
    private float _chronosUndergroundBonus = 0f;
    private int _streakEggShell = 0;
    [Space]

    [Header("Motion")]
    [Space]

    [Tooltip("The Speed of the worm to the right")]
    public float SpeedRight;
    [Tooltip("The Time the worm take to make the distance From Grass To BedRock")]
    public float SpeedDig;
    [Tooltip("The Time the worm take to make the distance From BedRock To Grass")]
    public float SpeedRising;
    [Tooltip("The Time the worm take before he dig")]
    public float AccelerationTimeRising;
    public AnimationCurve AccelerationCurveRising;
   
    [Tooltip("The Time the worm take before he dig")]
    public float AccelerationTimeDig;
    public AnimationCurve AccelerationCurveDig;

    //[Header("Motion-Angle")]
    //[Space]

    //public float MaxAngleRising;
    //public float DurationRotateDig;
    //public float DurationRotateRising;
    //[Range(0, 360)]
    //public float MinAngleDig;
    //[Range(0, 360)]
    //public float MaxAngleDig;

    void Start()
    {
        SetupWorm();

    }

    void Update()
    {
        GainPointUnderground();
        ScoreText.text = Score.ToString();
    }

    void SetVelocityFromSpeed()
    {
        //float distance = Vector3.Distance(GrassTransform.position, BedRockTransform.position);
        WormHead.VelocityDig = dirtDepth / SpeedDig;
        WormHead.VelocityRising = dirtDepth / SpeedRising;
        WormHead.Speed = SpeedRight;
        WormHead.AccelerationCurveDig = AccelerationCurveDig;
        WormHead.AccelerationCurveRising = AccelerationCurveRising;
        WormHead.AccelerationTimeRising = AccelerationTimeRising;
        WormHead.AccelerationTimeDig = AccelerationTimeDig;
    }

   

    void GainPointUnderground()
    {
        if (WormHead.transform.position.y < WormHead.StartPosition.y)
        {
            int multiplicator = (int)WormHead.DistanceFromStart / (int)StepDistanceScoreIncrease;
            int tempBonus = UndergroundBonus * (int)multiplicator;

            if (multiplicator > _lastMultiplicator)
            {
                UndergroundBonus *= multiplicator;
                _lastMultiplicator = multiplicator;

            }
            _chronosUndergroundBonus += Time.deltaTime;
           
            if (_chronosUndergroundBonus >= 1f)
            {
                Score += UndergroundBonus;
                _chronosUndergroundBonus = 0f;
            }
        }
        else
        {
            _chronosUndergroundBonus = 0f;
        }
    }

    void SetupWorm()
    {
        SetVelocityFromSpeed();
        
        WormHead.CallBackDead = YourAreDead;
        WormHead.CallBackPoint = GetPoint;


    }
   
    void YourAreDead()
    {
        Time.timeScale = 0f;
        GameManager.Instance.DeathCanvas.gameObject?.SetActive(true);
    }

    void GetPoint(CollectibleSpawnable collectible)
    {
        if(!collectible.IsEggShell)
        {
            Score += collectible.PointGain;

        }
        else if(!collectible.IsEggShell && _streakEggShell>0)
        {
            _streakEggShell = 0;
        }
        else
        {
            ++_streakEggShell;
            switch(_streakEggShell)
            {
                case 1:
                    Score += 20;
                    break;
                case 2:
                    Score += 50;
                    break;
                case 3:
                    Score += 100;
                    break;
            }
        }
    }

    //void SetDiggingAngle()
    //{
    //    //WormHead.MinAngleDig = MinAngleDig;
    //    //WormHead.MaxAngleDig = MaxAngleDig;
    //    //WormHead.MaxAngleRising = MaxAngleRising;

    //    //WormHead.DurationRotateDig = DurationRotateDig;
    //    //WormHead.DurationRotateRising = DurationRotateRising;
    //}
    //IEnumerator  AddScoreEverySecond()
    //{
    //    Score += UndergroundBonus;
    //    yield return new WaitForSeconds(1f);
    //}
}
