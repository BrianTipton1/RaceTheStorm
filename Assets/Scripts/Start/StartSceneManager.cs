using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Game;

namespace Start
{
    public class StartSceneManager : MonoBehaviour
    {
        private static StartSceneManager s;
        public static StartSceneManager S
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
                return "Start";
            }
        }

        private GameObject mainMenu;
        private GameObject controlsMenu;
        private GameObject aboutMenu;

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
            mainMenu = GameObject.Find("MainMenu");
            controlsMenu = GameObject.Find("ControlsMenu");
            aboutMenu = GameObject.Find("AboutMenu");

            controlsMenu.SetActive(false);
            aboutMenu.SetActive(false);
        }

        public void StartGame()
        {
            // Call the SceneManager to load the Game scene.
            SceneManager.LoadScene(GameSceneManager.sceneName);
        }

        public void OpenControls()
        {
            // Open the controls menu.
            controlsMenu.SetActive(true);
            mainMenu.SetActive(false);
        }

        public void CloseControls()
        {
            // Close the controls menu.
            controlsMenu.SetActive(false);
            mainMenu.SetActive(true);
        }

        public void OpenAbout()
        {
            // Open the about menu.
            aboutMenu.SetActive(true);
            mainMenu.SetActive(false);
        }

        public void CloseAbout()
        {
            // Close the about menu.
            aboutMenu.SetActive(false);
            mainMenu.SetActive(true);
        }
    }
}
