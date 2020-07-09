using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Unfolder : MonoBehaviour
{
    [Header("List Button")]
    public Button button;
    public RectTransform rectTransform;
    public TextMeshProUGUI explanationText, ecoFact;
    public Image arrowImage;

    [Header("Skin Button")]
    public bool isUnlocked = false;
    public Button skinToggle;
    public Image progressionImage;
    public TextMeshProUGUI progressionText;
    public enum UNLOCK_COND
    {
        LEVEL = 0,
        HIGH,
        BREAD,
        APPLE,
        EGG,
        EGG_CHAIN,
        SHIELD,
        ANGRY,
        DEATH
    }
    public UNLOCK_COND unlockCond = 0;
    [SerializeField] int unlockPrice = 100;
    public SkinData thisSkinData;

    [Header("Skin Preview")]
    public Image skinBGImage;
    public Image skinHead, skinMouth, skinEyes;

    public void SetProgression()
    {
        var progression = DressingManager.Instance.playerStatsArray[(int)unlockCond];
        progressionText.text = progression.ToString() + "/" + unlockPrice.ToString();
        progressionImage.fillAmount = Mathf.Min(1, (float)progression / unlockPrice);
        if (progressionImage.fillAmount == 1)
            UnlockSkinSelection();
    }
    public void UnlockSkinSelection()
    {
        skinToggle.interactable = true;
        isUnlocked = true;
        ecoFact.enabled = true;
    }
    public void SelectSkin()
    {
        DressingManager.Instance.unfoldManager.activatedSkinUnfolder.skinBGImage.color = new Color(0, 0, 0, 0.5f);
        skinBGImage.color = DressingManager.Instance.unfoldManager.selectedSkinBGColor;
        DressingManager.Instance.unfoldManager.activatedSkinUnfolder = this;
        GameManager.Instance.Player.LoadSkin(thisSkinData);
    }
    public void SetupSkinPreview()
    {
        skinHead.sprite = thisSkinData.HeadSprite;
        var normalFace = thisSkinData.Faces.FirstOrDefault(w => w.name.Contains("Normal"));
        skinMouth.sprite = normalFace.Mouth;
        skinEyes.sprite = normalFace.Eyes;
    }
}
