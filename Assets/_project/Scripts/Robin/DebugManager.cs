using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
public class DebugManager : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Time.timeScale += 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            Time.timeScale -= 0.1f;
        }
    }

    public void ClearData()
    {
        PlayerPrefs.DeleteAll();
        GameManager.Instance.Player.LoadData();
    }

}

#endif