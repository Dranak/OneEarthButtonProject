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
    const int detph = 9;
    [Tooltip("Distance detection of the obstacle or collectible to change face")]
    public float FieldOfView;
    public float TimeFaceDisplayed;
    public TextMeshProUGUI ScoreTextFB;
    [Space]

    [Header("Scoring")]
    [Space]
    public float TimeDisplayFeedBackScore = 0.1f;
    public float StepDistanceScoreIncrease;
    public int UndergroundBonus;
    public int MaxBonusUndergroundBonus;
    public int EggShellStreakOne;
    public int EggShellStreakTwo;
    public int EggShellStreakThird;

    private float _currentStateDistance;
    public int Score { get; set; } = 0;

    private float _chronosUndergroundBonus = 0f;
    public int StreakEggShell { get; set; } = 0;
    public int LastIndexEggShell { get; set; }
    [Space]

    [Header("Motion")]
    [Space]

    [Tooltip("The Speed of the worm to the right")]
    public float SpeedRight;
    public float TimeToIncreaseSpeed;
    public float MaxSpeed;
    public float ValueIncreaseSpeed;

    [Tooltip("The Time the worm take to make the distance From Grass To BedRock")]
    public float SpeedDig;
    [Tooltip("The Time the worm take before he dig")]
    public float AccelerationTimeDig;
    public AnimationCurve AccelerationCurveDig;
    [Tooltip("The Time the worm take to make the distance From BedRock To Grass")]
    public float SpeedRising;
    [Tooltip("The Time the worm take before he dig")]
    public float AccelerationTimeRising;
    public AnimationCurve AccelerationCurveRising;

    [Header("Analytics")]
    [SerializeField] GameLogin gameLogin;
    public string playingBlocName { get; set; }

    void Start()
    {
        _currentStateDistance = StepDistanceScoreIncrease;
        SetupWorm();

    }

    void Update()
    {
        GainPointUnderground();
        Debug.LogWarning("Actual worm speed: " + WormHead.Rigidbody.velocity.x);
        UiManager.Instance.GameMenu.ScoreText.text = Score.ToString();
    }

    void SetVelocityFromSpeed()
    {
        WormHead.VelocityDig = detph / SpeedDig;
        WormHead.VelocityRising = detph / SpeedRising;
        WormHead.Speed = SpeedRight;
        WormHead.AccelerationCurveDig = AccelerationCurveDig;
        WormHead.AccelerationCurveRising = AccelerationCurveRising;
        WormHead.AccelerationTimeRising = AccelerationTimeRising;
        WormHead.AccelerationTimeDig = AccelerationTimeDig;
        WormHead.TimeToIncreaseSpeed = TimeToIncreaseSpeed;
        WormHead.MaxSpeed = MaxSpeed;
        WormHead.ValueIncreaseSpeed = ValueIncreaseSpeed;
    }




    void GainPointUnderground()
    {
        if (WormHead.transform.position.y < WormHead.StartPosition.y)
        {
            //Debug.Log("DistanceFromStart: " + WormHead.DistanceFromStart);

            if (WormHead.DistanceFromStart >= _currentStateDistance && UndergroundBonus < MaxBonusUndergroundBonus)
            {
                _currentStateDistance += StepDistanceScoreIncrease;
                UndergroundBonus += 1;


                Debug.Log("UndergroundBonus: " + UndergroundBonus);
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
        WormHead.FieldOfView = FieldOfView;
        WormHead.TimeFaceDisplayed = TimeFaceDisplayed;
        WormHead.CallBackDead = YourAreDead;
        WormHead.CallBackPoint = GetPoint;


    }

    void YourAreDead(Obstacle obstacleTouched, Player player)
    {
        ++UiManager.Instance.SessionGameCount;
        if (!Application.isEditor && !Debug.isDebugBuild) // Log only for non-dev builds
            gameLogin.OnGameOver(player, obstacleTouched);
        GameManager.Instance.SetState(State.Dead);
    }

    void GetPoint(Collectible collectible)
    {
        if (collectible.collectibleParameters.EggShellIndex == -1)
        {
            ScoreTextFB.text = "+" + collectible.PointGain.ToString();
            Score += collectible.PointGain;

        }
        //else if (collectible.collectibleParameters.EggShellIndex == -1 && StreakEggShell > 0)
        //{
        //    StreakEggShell = 0;

        //    ScoreTextFB.text = "+" + collectible.PointGain.ToString();
        //    Score += collectible.PointGain;
        //}
        else if (collectible.collectibleParameters.EggShellIndex > -1)
        {
            if (collectible.collectibleParameters.EggShellIndex != LastIndexEggShell)
            {
                StreakEggShell = 0;
                LastIndexEggShell = collectible.collectibleParameters.EggShellIndex;
            }
            ++StreakEggShell;

            Debug.Log("EggShellIndex " + collectible.collectibleParameters.EggShellIndex
         + "StreakEggShell " + StreakEggShell);

            Debug.Log("Point Before: " + Score);
            switch (StreakEggShell)
            {
                case 1:
                    Score += EggShellStreakOne;
                    ScoreTextFB.text = "+" + EggShellStreakOne;
                    break;
                case 2:
                    Score += EggShellStreakTwo;
                    ScoreTextFB.text = "+" + EggShellStreakTwo;
                    break;
                case 3:
                    Debug.Log(" Streak Completed EggShellIndex " + collectible.collectibleParameters.EggShellIndex
            + "StreakEggShell " + StreakEggShell);
                    Score += EggShellStreakThird;
                    ScoreTextFB.text = "+" + EggShellStreakThird;
                    StreakEggShell = 0;
                ++UiManager.Instance.SessionStrikesTotal;
                
                    break;
                default:
                    Debug.Log("I don't know what to do");
                    StreakEggShell = 0;
                    break;
            }
            Debug.Log("Point After: " + Score);
        }
        //ScoreTextFB.enabled = true;
        StartCoroutine(DisplayText(ScoreTextFB, TimeDisplayFeedBackScore));
    }

    IEnumerator DisplayText(TextMeshProUGUI text, float time)
    {


        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);
        ScoreTextFB.text = "";
        // text.ClearMesh();

        //text.enabled = false;


    }

    private void OnApplicationQuit()
    {
        if (UiManager.Instance.SessionGameCount > 0)
            gameLogin.OnSessionOver(this);
    }

    

}
