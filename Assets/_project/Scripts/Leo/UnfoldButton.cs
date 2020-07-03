using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class UnfoldButton : MonoBehaviour
{
    [HideInInspector] public Unfolder selectedUnfolder = null;
    Coroutine rollingCoroutine = null;
    [SerializeField] AnimationCurve buttonResizeCurve;
    [SerializeField] Unfolder[] unfolders;
    public Unfolder activatedSkinUnfolder;
    public Color32 selectedSkinBGColor;

    public void Folding(Unfolder unfolder)
    {
        if (rollingCoroutine != null)
        {
            StopCoroutine(rollingCoroutine);
            rollingCoroutine = null;
        }
        if (selectedUnfolder != null)
        {
            Fold(selectedUnfolder);
            if (selectedUnfolder == unfolder)
            {
                UiManager.Instance.eventSystem.SetSelectedGameObject(null);
                selectedUnfolder = null;
            }
            else
            {
                Unfold(unfolder);
                selectedUnfolder = unfolder;
            }
        }
        else
        {
            Unfold(unfolder);
            selectedUnfolder = unfolder;
        }
    }

    void Unfold(Unfolder _selectedUnfolder)
    {
        rollingCoroutine = StartCoroutine("RollingCoroutine", new object[3] { _selectedUnfolder, 350 + 100 * Convert.ToInt32(_selectedUnfolder.isUnlocked), true });
    }

    void Fold(Unfolder _selectedUnfolder)
    {
        rollingCoroutine = StartCoroutine("RollingCoroutine", new object[3] { _selectedUnfolder, 200, false });
    }

    IEnumerator RollingCoroutine(object[] param)
    {
        Unfolder unfolder = (Unfolder)param[0];
        int endingDelta = (int)param[1];
        bool explanationEnabled = (bool)param[2];
        unfolder.arrowImage.transform.localScale = new Vector3(1, 1 - 2 * Convert.ToInt32(explanationEnabled), 1);

        var rectT = unfolder.rectTransform;
        float time = 0;
        float startingDelta = rectT.sizeDelta.y;
        //float endSpace = Mathf.Abs()
        
        while (rectT.sizeDelta.y != endingDelta)
        {
            time += Time.unscaledDeltaTime * 2;
            rectT.sizeDelta = new Vector2(rectT.sizeDelta.x, Mathf.Lerp(startingDelta, endingDelta, buttonResizeCurve.Evaluate(time)));
            yield return new WaitForEndOfFrame();
        }
        //rectT.sizeDelta = new Vector2(rectT.sizeDelta.x, endingDelta);
    }

    public void UnfoldersSetup()
    {
        var skinPreviewBool = UiManager.Instance.isSkinPreviewsSet;
        foreach(Unfolder unfolder in unfolders)
        {
            if (!unfolder.isUnlocked)
            {
                unfolder.SetProgression();
            }
            if (unfolder.thisSkinData == UiManager.Instance.allSkins[GameManager.Instance.Player.CurrentSkinId])
                unfolder.SelectSkin();
            if (!skinPreviewBool)
                unfolder.SetupSkinPreview();
        }
        UiManager.Instance.isSkinPreviewsSet = true;
    }
}
