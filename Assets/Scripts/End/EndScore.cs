using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace End
{
    public class EndScore : MonoBehaviour
    {
        public static int prevScore = -1;

        [Header("Set in Inspector")]
        public string scoreTemplate = "Your Score: {0}pts";
        public string newHighScoreTemplate = "New High Score!";
        public string notHighScoreTemplate = "Current High Score: {0}pts";

        private TextMeshProUGUI tmp;

        void Awake()
        {
            tmp = GetComponent<TextMeshProUGUI>();
            int highScore = PlayerPrefs.GetInt("RaceTheSun", 0);
            if (prevScore > highScore)
            {
                PlayerPrefs.SetInt("RaceTheSun", prevScore);
                string newHighScore = string.Format(scoreTemplate, prevScore);
                tmp.text = newHighScore + "\n\n" + newHighScoreTemplate;
            }
            else
            {
                string score = string.Format(scoreTemplate, prevScore);
                string notHighScoreText = string.Format(notHighScoreTemplate, highScore);
                tmp.text = score + "\n\n" + notHighScoreText;
            }
        }
    }
}
