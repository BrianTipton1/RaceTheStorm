using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Game;
using Start;

namespace End
{
    public class EndSceneManager : MonoBehaviour
    {
        private static EndSceneManager s;
        public static EndSceneManager S
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

        public static string sceneName
        {
            get
            {
                return "End";
            }
        }

        private GameObject tryAgainButton;
        private GameObject mainMenuButton;

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
            tryAgainButton = GameObject.Find("TryAgainButton");
            mainMenuButton = GameObject.Find("MainMenuButton");
        }

        public void TryAgain()
        {
            // Call the SceneManager to load the Game scene.
            SceneManager.LoadScene(GameSceneManager.sceneName);
        }

        public void MainMenu()
        {
            // Call the SceneManager to load the Start scene.
            SceneManager.LoadScene(StartSceneManager.sceneName);
        }
    }
}
