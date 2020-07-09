using System;
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
    public SkinData DefaultSkin;
    public SkinData CurrentSkin { get; set; }

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
    public int HighScore { get; set; }
    private float _chronosUndergroundBonus = 0f;
    public int StreakEggShell { get; set; } = 0;
    public int LastIndexEggShell { get; set; }

    [Header("Bonus")]
    public Bonus currentBonus = 0;
    public SpriteRenderer[] bonusRenderers;
    public enum Bonus
    {
        None = 0,
        Shield,
        Rage,
        Small
    }

    [Header("Munch Sounds")]
    [SerializeField] AudioSource encounteredObjSoundSource;
    
    [Space]

    [Header("Level System")]
    public TextAsset LevelData;
    public List<string> DataSplit { get; set; } = new List<string>();
    public int CurrentLevelPlayer { get; set; }
    public float NeededXp { get; set; }
    public int NextLevelPlayer { get;set; }
    public float CurrentXp { get;set; }
    public int CurrentSkinId { get; set; }

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
        //PlayerPrefs.DeleteAll();
        SetupWorm();
        LoadData();

        if (UiManager.Instance.State == State.Play)
            UiManager.Instance.MainMenu.Play();
    }

    void Update()
    {
        GainPointUnderground();
        //Debug.LogWarning("Actual worm speed: " + WormHead.Rigidbody.velocity.x);
        UiManager.Instance.GameMenu.ScoreText.text = Score.ToString();

        // refresh highscore realtime
        if (Score > HighScore)
        {
            HighScore = Score;
        }
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


                //Debug.Log("UndergroundBonus: " + UndergroundBonus);
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
        WormHead.SetupBody();
    }

    void YourAreDead(Obstacle obstacleTouched, Player player)
    {
        ++UiManager.Instance.SessionGameCount;
        if (!Application.isEditor && !Debug.isDebugBuild) // Log only for non-dev builds
            gameLogin.OnGameOver(player, obstacleTouched);
        IncreaseStatTotal("Deaths", 1);
        encounteredObjSoundSource.PlayOneShot(obstacleTouched.touchedSound);
        GameManager.Instance.SetState(State.Dead);
    }

    void GetPoint(Collectible collectible)
    {
        //collectible.starParticle.Play();
        string collectibleStatName = "";
        var collectibleParams = collectible.collectibleParameters;

        var pointGain = collectible.PointGain;
        if (pointGain > 0)
        {
            ScoreTextFB.text = "+" + pointGain.ToString();
            Score += pointGain;
            IncreaseStatTotal("EcoPoints", pointGain);
        }

        bool starEffect = true;
        switch (collectibleParams.Tag)
        {
            case "collectible_apple":
                collectibleStatName = "AppleCores";
                PlayObjSoundWithPitch(collectible, 1);
                break;
            case "collectible_breadcrumb":
                collectibleStatName = "BreadCrumbs";
                PlayObjSoundWithPitch(collectible, UnityEngine.Random.Range(0.75f, 1.25f));
                break;
            case "shield_bonus":
                collectibleStatName = "Shields";
                PlayObjSoundWithPitch(collectible, 1);
                ActivateBonus(Bonus.Shield);
                starEffect = false;
                break;
            /*case "rage_bonus":
                collectibleStatName = "Angrys";
                PlayObjSoundWithPitch(collectible, 1);
                ActivateBonus(Bonus.Rage);
                starEffect = false;
                break;*/
            case "collectible_eggshell":
                collectibleStatName = "EggShells";
                break;
        }
        if (starEffect)
            Instantiate(collectible.starParticle, collectible.objectBody.position, Quaternion.identity);

        if (collectibleParams.EggShellIndex > -1)
        {
            if (collectibleParams.EggShellIndex != LastIndexEggShell)
            {
                StreakEggShell = 0;
                LastIndexEggShell = collectibleParams.EggShellIndex;
            }
            ++StreakEggShell;

            //   Debug.Log("EggShellIndex " + collectible.collectibleParameters.EggShellIndex
            //+ "StreakEggShell " + StreakEggShell);

            //   Debug.Log("Point Before: " + Score);
            int scoreIncrease = 0;
            float eggPitch = 0.75f;
            switch (StreakEggShell)
            {
                case 1:
                    scoreIncrease = EggShellStreakOne;
                    break;
                case 2:
                    scoreIncrease = EggShellStreakTwo;
                    eggPitch = 1;
                    break;
                case 3:
                    //        Debug.Log(" Streak Completed EggShellIndex " + collectible.collectibleParameters.EggShellIndex
                    //+ "StreakEggShell " + StreakEggShell);
                    scoreIncrease = EggShellStreakThird;
                    StreakEggShell = 0;
                    ++UiManager.Instance.SessionStrikesTotal;
                    eggPitch = 1.25f;
                    IncreaseStatTotal("EggChains", 1);
                    break;
                default:
                    Debug.Log("I don't know what to do");
                    StreakEggShell = 0;
                    break;
            }
            PlayObjSoundWithPitch(collectible, eggPitch);
            Score += scoreIncrease;
            ScoreTextFB.text = "+" + scoreIncrease;
            IncreaseStatTotal("EcoPoints", scoreIncrease);
            //Debug.Log("Point After: " + Score);
        }
        IncreaseStatTotal(collectibleStatName, 1); // increase this collectible stat count (total collected of this type)
        //ScoreTextFB.enabled = true;
        StartCoroutine(DisplayText(ScoreTextFB, TimeDisplayFeedBackScore));
    }
    public void IncreaseStatTotal(in string pref, in int increase)
    {
        PlayerPrefs.SetInt(pref, PlayerPrefs.GetInt(pref, 0) + increase);
    }
    public void ActivateBonus(Bonus bonus)
    {
        foreach (SpriteRenderer renderer in bonusRenderers)
        {
            renderer.enabled = false;
        }

        var bonusIdx = (int)bonus - 1;
        if (bonusIdx > -1)
        {
            bonusRenderers[bonusIdx].enabled = true;
        }

        currentBonus = bonus;
    }
    void PlayObjSoundWithPitch(in SpawnableObject spawnable, in float pitch)
    {
        encounteredObjSoundSource.pitch = pitch;
        encounteredObjSoundSource.PlayOneShot(spawnable.touchedSound);
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
        PlayerPrefs.SetInt("HighScore", HighScore);
    }


    public void LoadData()
    {
        HighScore = PlayerPrefs.GetInt("HighScore", 0);
        CurrentXp = PlayerPrefs.GetFloat("CurrentXp", 0f);
        CurrentLevelPlayer = PlayerPrefs.GetInt("LevelPlayer", 1);
        LoadSkin(PlayerPrefs.GetInt("SkinID", 0));
        //Debug.Log("HighScore " + HighScore);
        //Debug.Log("CurrentXp " + CurrentXp);
        //Debug.Log("CurrentLevelPlayer " + CurrentLevelPlayer);

        LoadDataFromFile();



    }

    public void LoadDataFromFile()
    {
        string contentFile = LevelData.text;
        foreach (string line in contentFile.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None))
        {
            //Debug.Log("Line " + line);
            string[] splitLine = line.Split(',');
            //Debug.Log("splitLine " + splitLine);
            if (splitLine[0] == CurrentLevelPlayer.ToString())
            {
                NeededXp = Int32.Parse(splitLine[1]);
                //Debug.Log("NeededXp " + NeededXp);
                break;
            }
        }
    }

    public void SaveData()
    {
        PlayerPrefs.SetFloat("CurrentXp", CurrentXp);
        PlayerPrefs.SetInt("LevelPlayer", CurrentLevelPlayer);
    }

    public void LoadSkin(SkinData skinData)
    {
        CurrentSkinId = UiManager.Instance.allSkins.IndexOf(skinData);
        WormHead.SetSkin(skinData);
        PlayerPrefs.SetInt("SkinID", CurrentSkinId);
    }
    void LoadSkin(int skinID)
    {
        CurrentSkinId = skinID;
        WormHead.SetSkin(UiManager.Instance.allSkins[skinID]);
    }
}
