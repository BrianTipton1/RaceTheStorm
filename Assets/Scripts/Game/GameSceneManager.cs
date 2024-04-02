using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using End;
using Start;
using TMPro;

namespace Game
{
    public class GameSceneManager : MonoBehaviour
    {
        private static GameSceneManager s;
        public static GameSceneManager S
        {
            get
            {
                return s;
            }
            private set
            {
                if (s == null)
                {
                    s = value;
                }
            }
        }

        [SerializeField]
        public GameObject currentScoreGO;
        public string currentScoreTemplate = "Current Score: {0}pts";

        public static string sceneName
        {
            get
            {
                return "Game";
            }
        }

        private float startTime;
        private GameObject pauseMenu;
        private TextMeshProUGUI currentScoreText;

        void Awake()
        {
            if (S == null)
            {
                S = this;
            }
            else
            {
                Destroy(this);
            }
        }

        void Start()
        {
            startTime = Time.time;
            Time.timeScale = 1;

            pauseMenu = GameObject.Find("PauseMenu");
            pauseMenu.SetActive(false);

            currentScoreText = currentScoreGO.GetComponent<TextMeshProUGUI>();
            currentScoreText.text = string.Format(currentScoreTemplate, 0);
        }

        void Update()
        {
            currentScoreText.text = string.Format(currentScoreTemplate, Mathf.RoundToInt(Time.time - startTime));
        }

        public void EndGame()
        {
            EndScore.prevScore = Mathf.RoundToInt(Time.time - startTime);
            SceneManager.LoadScene(EndSceneManager.sceneName);
        }

        public void BackToMainMenu()
        {
            SceneManager.LoadScene(StartSceneManager.sceneName);
        }

        public void PauseGame()
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }

        public void ResumeGame()
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }

        public void SlowMotionGame()
        {
            Time.timeScale = 0.5f;
        }

        public void NormalSpeedGame()
        {
            Time.timeScale = 1;
        }

        public void FastForwardGame()
        {
            Time.timeScale = 2;
        }
    }
}
