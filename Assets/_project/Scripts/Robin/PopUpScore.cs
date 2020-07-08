using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUpScore : MonoBehaviour
{
    public float OffsetY;
    private WormHead Worm { get; set; } 
    public TextMeshProUGUI ScoreText;
    // Start is called before the first frame update
    void Start()
    {
        //  ScoreText = GetComponentInChildren<TextMeshProUGUI>();
        Worm = GameManager.Instance.Player.WormHead;
    }

    // Update is called once per frame
    void Update()
    {
        FolllowHead();
    }


    void FolllowHead()
    {
        Vector3 newPos = new Vector3(Worm.transform.position.x, Worm.transform.position.y+OffsetY, Worm.transform.position.z);
        ScoreText.transform.position = newPos;
    }
}
