using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndLevel : MonoBehaviour
{
    public string levelTemplate = "Lost on Level: {0}";
    private TextMeshProUGUI tmp;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        tmp.text = string.Format(levelTemplate, Game.OverlayManager.currentLevel);
    }
}
