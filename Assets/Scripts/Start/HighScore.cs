using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Start
{
    public class HighScore : MonoBehaviour
    {
        [Header("Set in Inspector")]
        public string highScoreTemplate = "Current High Score: {0}pts";
        public string noHighScoreTemplate = "No High Score Set";

        private TextMeshProUGUI tmp;

        void Start()
        {
            tmp = GetComponent<TextMeshProUGUI>();
            int highScore = PlayerPrefs.GetInt("RaceTheSun", 0);
            if (highScore > 0)
            {
                tmp.text = string.Format(highScoreTemplate, highScore);
            }
            else
            {
                tmp.text = noHighScoreTemplate;
            }
        }
    }
}
