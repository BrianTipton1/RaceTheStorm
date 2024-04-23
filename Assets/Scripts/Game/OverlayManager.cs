using System.Collections;
using System.Collections.Generic;
using Game.Player;
using UnityEngine;
using TMPro;
using Player;

namespace Game
{
    public enum LevelOverlayStatus
    {
        SHOWING,
        ALREADY_SHOWN,
        NOT_SHOWN
    }

    public class OverlayManager : MonoBehaviour
    {
        private static OverlayManager s;
        public static OverlayManager S
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
        [SerializeField]
        public GameObject currentLevelGO;
        [SerializeField]
        public GameObject tornadoDistanceGO;
        public string currentScoreTemplate = "Current Score: {0}pts";
        public string currentLevelTemplate = "Level: {0}";
        public string tornadoDistanceTemplate = "{0}m away";
        public static int currentLevel = 0;
        public int numFramesBetweenCheck = 100;
        public int startingTornadoDistance = 500;

        private TextMeshProUGUI currentScoreText;
        private TextMeshProUGUI currentLevelText;
        private TextMeshProUGUI tornadoDistanceText;
        private int dummyFrameCount = 0;
        private int frameIndex = 0;
        private LevelOverlayStatus levelOverlayStatus = LevelOverlayStatus.NOT_SHOWN;

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
            currentLevel = 0;
            currentScoreText = currentScoreGO.GetComponent<TextMeshProUGUI>();
            currentScoreText.text = string.Format(currentScoreTemplate, 0);

            currentLevelText = currentLevelGO.GetComponent<TextMeshProUGUI>();
            currentLevelGO.SetActive(false);

            tornadoDistanceText = tornadoDistanceGO.GetComponent<TextMeshProUGUI>();
        }

        void FixedUpdate()
        {
            // Update the current score
            currentScoreText.text = string.Format(currentScoreTemplate, Mathf.RoundToInt(Time.time - GameSceneManager.startTime) + PowerupController.numPowerupsGathered * 5);

            // Check if the player is on a blank plane, if so, show the level (every numFramesBetweenCheck frames)
            if (dummyFrameCount % numFramesBetweenCheck == 0)
            {
                // See what object is right below the player, determine if it has any children (if so, it is not a blank plane, means don't have text)
                RaycastHit hit;
                Vector3 direction = new Vector3(Landspeeder.Instance.transform.position.x, -2.5f, Landspeeder.Instance.transform.position.z);
                if (Physics.Raycast(direction, Vector3.down, out hit, 5f))
                {
                    // print("Hit: " + hit.transform.name);
                    // print("Hit child count: " + hit.transform.childCount);
                    // print("Level Overlay Status: " + levelOverlayStatus);
                    if (hit.transform.childCount == 0 && levelOverlayStatus == LevelOverlayStatus.NOT_SHOWN)
                    {
                        currentLevel++;
                        levelOverlayStatus = LevelOverlayStatus.SHOWING;
                        currentLevelGO.SetActive(true);
                        currentLevelText.text = string.Format(currentLevelTemplate, currentLevel);
                        StartCoroutine(HideLevelText());
                    }
                    else if (hit.transform.childCount != 0 && levelOverlayStatus == LevelOverlayStatus.ALREADY_SHOWN)
                    {
                        levelOverlayStatus = LevelOverlayStatus.NOT_SHOWN;
                    }
                }
                dummyFrameCount = 0;
            }
            dummyFrameCount++;

            // Decrement the storm's distance by 1 (so it gets closer) unless origForwardSpeed < forwardSpeed
            if (PowerupController.Instance.IsBoosting)
            {
                startingTornadoDistance += 1;
            }
            else if (frameIndex % 2 == 0)
            {
                frameIndex = 0;
                startingTornadoDistance -= 1;
            }
            frameIndex++;

            // If the tornado has reached the player, end the game
            if (startingTornadoDistance < 0)
            {
                GameSceneManager.S.EndGame();
            }
            tornadoDistanceText.text = string.Format(tornadoDistanceTemplate, startingTornadoDistance);
        }

        public float GetSpeedMultiplier()
        {
            return 1 + (currentLevel * 0.2f);
        }

        public float GetObstacleMultiplier()
        {
            return 1 + (currentLevel * 0.4f);
        }

        public float GetPowerupMultiplier()
        {
            return 1 + (currentLevel * 2f);
        }

        private IEnumerator HideLevelText()
        {
            yield return new WaitForSeconds(3);
            currentLevelGO.SetActive(false);
            levelOverlayStatus = LevelOverlayStatus.ALREADY_SHOWN;
        }
    }
}
