using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DressingManager : MonoBehaviour
{
    public static DressingManager Instance = null;

    [SerializeField] TextMeshProUGUI MenuTitle;
    [SerializeField] string[] MenuTitles;
    [SerializeField] GameObject[] menuPanels;
    [SerializeField] Image switchButtonImage;
    [SerializeField] Sprite[] switchButtonSprites;

    [Header("STATS")]
    [Header("Level")]
    [SerializeField] Slider currentProgressionSlider;
    [SerializeField] TextMeshProUGUI levelBarTitle, currentLevel, nextLevel;

    [Header("Other Stats")]
    [SerializeField] TextMeshProUGUI bestScore;
    [SerializeField] TextMeshProUGUI ecoPoints, breadCrumbs, appleCores, eggshells, eggshellsChains, shields, angryWorms, deaths;

    [Header("SKINS")]
    public UnfoldButton unfoldManager;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
    }

    float currentXp;
    int levelPlayer, highScore, breadC, apples, eggs, eggChains, shieldB, angrys, deathC, ecoP;
    [HideInInspector] public int[] playerStatsArray;

    private void OnEnable()
    {
        GetPlayerPrefs();
        SetPrefsToMenus();
    }

    void GetPlayerPrefs()
    {
        currentXp = PlayerPrefs.GetFloat("CurrentXp", 0);
        levelPlayer = PlayerPrefs.GetInt("LevelPlayer", 1);
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        ecoP = PlayerPrefs.GetInt("EcoPoints", 0);
        breadC = PlayerPrefs.GetInt("BreadCrumbs", 0);
        apples = PlayerPrefs.GetInt("AppleCores", 0);
        eggs = PlayerPrefs.GetInt("EggShells", 0);
        eggChains = PlayerPrefs.GetInt("EggChains", 0);
        shieldB = PlayerPrefs.GetInt("Shields", 0);
        angrys = PlayerPrefs.GetInt("Angrys", 0);
        deathC = PlayerPrefs.GetInt("Deaths", 0);

        playerStatsArray = new int[9] { levelPlayer, highScore, breadC, apples, eggs, eggChains, shieldB, angrys, deathC };
    }
    void SetPrefsToMenus()
    {
        // level progression setup
        currentProgressionSlider.value = currentXp;
        currentLevel.text = levelPlayer.ToString();
        levelBarTitle.text = "Level : " + levelPlayer;
        nextLevel.text = (levelPlayer + 1).ToString();
        // stats setup
        bestScore.text = highScore.ToString();
        ecoPoints.text = ecoP.ToString();
        breadCrumbs.text = breadC.ToString();
        appleCores.text = apples.ToString();
        eggshells.text = eggs.ToString();
        eggshellsChains.text = eggChains.ToString();
        shields.text = shieldB.ToString();
        angryWorms.text = angrys.ToString();
        deaths.text = deathC.ToString();

        // skin list setup
        unfoldManager.UnfoldersSetup();
    }

    int selectedUIPanel = -1;
    public void SwitchMenuPanel(int PanelID)
    {
        selectedUIPanel = PanelID;
        MenuTitle.text = MenuTitles[PanelID];
        menuPanels[PanelID].SetActive(true);
        menuPanels[(PanelID + 1) % 2].SetActive(false); // deactivate possibly activated panel
        switchButtonImage.sprite = switchButtonSprites[PanelID];
    }
    public void SwitchMenuPanel()
    {
        SwitchMenuPanel((selectedUIPanel + 1) % 2);
    }
    public void BackToMainMenu()
    {
        menuPanels[(selectedUIPanel+1)%2].SetActive(true); // activate deactivated panel
        gameObject.SetActive(false); // deactivate main panel (dressing)
    }
}
