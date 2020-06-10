using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainMenu : MonoBehaviour
{


    public void Play()
    {
        Debug.Log("Play");
        GameManager.Instance.State = State.Play;
        UiManager.Instance.GameMenu.gameObject.SetActive(true);
    }

    private void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }

}