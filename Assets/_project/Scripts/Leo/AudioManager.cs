using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioMixer musicMixer, sfxMixer;
    bool musicMute = false;
    bool sfxMute = false;

    void MuteUnmuteMixer(in AudioMixer audioMixer, ref bool toMute)
    {
        toMute = !toMute;
        audioMixer.SetFloat("Volume", -80f * Convert.ToInt32(toMute));
    }
    public void MuteUnmuteMusic(Toggle toggle)
    {
        MuteUnmuteMixer(in musicMixer, ref musicMute);
    }
    public void MuteUnmuteSfx(Toggle toggle)
    {
        MuteUnmuteMixer(in sfxMixer, ref sfxMute);
    }

    public void BackToMainMenu()
    {
        UiManager.Instance.BackToMainMenu(gameObject);
    }
}
