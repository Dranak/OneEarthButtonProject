using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DressingManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI MenuTitle;

    [Header("STATS")]
    [Header("Level")]
    [SerializeField] Slider currentProgressionSlider;
    [SerializeField] TextMeshProUGUI levelBarTitle, currentLevel, nextLevel;

    [Header("Other Stats")]
    [SerializeField] TextMeshProUGUI bestScore;
    [SerializeField] TextMeshProUGUI ecoPoints, breadCrumbs, appleCores, eggshells, eggshellsChains, shields, angryWorms, deaths;

    [Header("SKINS")]
    [SerializeField] Button[] skinActivators;
    
    private void OnEnable()
    {
        // level progression setup
        currentProgressionSlider.value = PlayerPrefs.GetFloat("CurrentXp", 0);
        var currentLvl = PlayerPrefs.GetInt("LevelPlayer", 1);
        currentLevel.text = currentLvl.ToString();
        levelBarTitle.text = "Level : " + currentLvl;
        nextLevel.text = (currentLvl + 1).ToString();

        // stats setup
        bestScore.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
        ecoPoints.text = PlayerPrefs.GetInt("EcoPoints", 0).ToString();
        breadCrumbs.text = PlayerPrefs.GetInt("BreadCrumbs", 0).ToString();
        appleCores.text = PlayerPrefs.GetInt("AppleCores", 0).ToString();
        eggshells.text = PlayerPrefs.GetInt("EggShells", 0).ToString();
        eggshellsChains.text = PlayerPrefs.GetInt("EggChains", 0).ToString();
        shields.text = PlayerPrefs.GetInt("Shields", 0).ToString();
        angryWorms.text = PlayerPrefs.GetInt("Angrys", 0).ToString();
        deaths.text = PlayerPrefs.GetInt("Deaths", 0).ToString();
    }
}
