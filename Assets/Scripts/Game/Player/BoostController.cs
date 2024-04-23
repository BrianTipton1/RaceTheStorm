using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game.Player
{
    public class BoostController : MonoBehaviour
    {
        public static BoostController Instance { get; private set; }
        private Landspeeder _landspeeder;
        private GroundController _groundController;

        public float barrelBoostMultiplier = 3f;
        public int barrelBoostDur = 3;

        public float regBoostMultiplier = 2f;
        public int regBoostDur = 2;

        public static int numPowerupsGathered = 0;
        public bool IsBoosting = false;
        private List<TBoost> boosts = new List<TBoost>();
        private int currentBoostDur = 0;

        private TextMeshProUGUI BoostCounterText;
        private GameObject BoostCounter;

        public enum TBoost
        {
            Regular,
            BarrelRoll,
        }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            BoostCounter = GameObject.FindGameObjectWithTag("BoostCounter");
            BoostCounterText = BoostCounter.GetComponent<TextMeshProUGUI>();
            numPowerupsGathered = 0;
        }

        private void Update()
        {
            if (currentBoostDur > 0)
            {
                BoostCounter.SetActive(true);
                BoostCounterText.text = $"Boosting for {currentBoostDur}";
            }
            else
            {
                BoostCounter.SetActive(false);
            }

            _landspeeder = Landspeeder.Instance;
            _groundController = GroundController.Instance;
        }


        public void Boost()
        {
            numPowerupsGathered++;
            IncBoosts();
            if (!IsBoosting)
            {
                IsBoosting = true;
                StartCoroutine(BoostCoroutine());
            }
        }

        private void IncBoosts()
        {
            if (Random.Range(0, 2) == 0)
            {
                currentBoostDur += regBoostDur;
                boosts.Add(TBoost.Regular);
            }
            else
            {
                currentBoostDur += barrelBoostDur;
                _landspeeder.DoABarrelRoll(1, 0.5f);
                boosts.Add(TBoost.BarrelRoll);
            }
        }

        IEnumerator BoostCoroutine(float? parentSpeedForward = null, float? parentSpeedSideways = null)
        {
            float originalSpeedForward = parentSpeedForward ?? _groundController.forwardSpeed;
            float originalSpeedSideways = parentSpeedSideways ?? _groundController.sidewaysSpeed;
            float totalMultiplier = boosts[0] == TBoost.Regular ? regBoostMultiplier : barrelBoostMultiplier;

            _groundController.forwardSpeed = originalSpeedForward * totalMultiplier;
            _groundController.sidewaysSpeed = originalSpeedSideways * totalMultiplier;

            int boostDuration = boosts[0] == TBoost.Regular ? regBoostDur : barrelBoostDur;
            for (int i = 0; i < boostDuration; i++)
            {
                yield return new WaitForSeconds(1);
                currentBoostDur--;
            }

            boosts.RemoveAt(0);
            if (boosts.Count > 0)
            {
                StartCoroutine(BoostCoroutine(originalSpeedForward, originalSpeedSideways));
            }
            else
            {
                _groundController.forwardSpeed = originalSpeedForward;
                _groundController.sidewaysSpeed = originalSpeedSideways;
                IsBoosting = false;
            }
        }
    }
}