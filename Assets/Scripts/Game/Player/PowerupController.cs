using System;
using System.Collections;
using Player;
using UnityEngine;

namespace Game.Player
{
    public class PowerupController : MonoBehaviour
    {
        public static PowerupController Instance { get; private set; }
        private Landspeeder _landspeeder;
        private GroundController _groundController;

        public float boostMultiplier = 2f;
        public float boostDuration = 2f;
        public bool IsBoosting = false;
        public static int numPowerupsGathered = 0;

        public enum T
        {
            Jump,
            Boost,
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
            numPowerupsGathered = 0;
        }

        private void Update()
        {
            _landspeeder = Landspeeder.Instance;
            _groundController = GroundController.Instance;
        }

        private void BarrelRoll()
        {
            _landspeeder.DoABarrelRoll(1);
            _groundController.forwardSpeed *= 1.5f;
        }

        public void ActivatePowerup(T powerup)
        {
        }

        public void SetNewPowerup()
        {
            numPowerupsGathered++;
            if (!IsBoosting)
            {
                IsBoosting = true;
                StartCoroutine(Boost());
            }
        }

        IEnumerator Boost()
        {
            float originalSpeed = _groundController.forwardSpeed;
            _groundController.forwardSpeed *= boostMultiplier;
            yield return new WaitForSeconds(boostDuration);
            _groundController.forwardSpeed = originalSpeed * 1.01f;
            IsBoosting = false;
        }
    }
}