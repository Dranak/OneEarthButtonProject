using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundEffect : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] List<AudioClip> audioClips;

    void OnEnable()
    {
        if (audioClips.Count > 0)
            audioSource.clip = audioClips[Random.Range(0, audioClips.Count)];
        audioSource.pitch = Random.Range(0.75f, 1.25f);
        //audioSource.Play();
    }
}
