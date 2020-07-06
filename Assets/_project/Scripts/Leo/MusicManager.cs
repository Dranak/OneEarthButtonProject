using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource gameMusicSource, menuMusicSource;
    [SerializeField] AudioClip mainMenuMusic, pauseMusic, deathMusic;

    public void SwitchMusic(in State state)
    {
        switch (state)
        {
            case State.InMenu:
                menuMusicSource.clip = mainMenuMusic;
                menuMusicSource.volume = 0.75f;
                break;
            case State.Play:
                if (gameMusicSource.isVirtual)
                    gameMusicSource.UnPause();
                else
                    gameMusicSource.Play();
                menuMusicSource.Stop();
                break;
            case State.Dead:
                gameMusicSource.Stop();
                menuMusicSource.clip = deathMusic;
                menuMusicSource.volume = 0.5f;
                menuMusicSource.Play();
                break;
            case State.Pause:
                gameMusicSource.Pause();
                menuMusicSource.clip = pauseMusic;
                menuMusicSource.volume = 0.5f;
                menuMusicSource.Play();
                break;
        }
    }

}
