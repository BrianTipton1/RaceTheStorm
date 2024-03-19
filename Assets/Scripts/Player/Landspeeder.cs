using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Player.Motion;
using Player.SpeederInput;
using UnityEngine;
using Random = UnityEngine.Random;
using Direction = Player.SpeederInput.Controller.Direction;
using Modifier = Player.SpeederInput.Controller.Modifier;

namespace Player
{
    public class Landspeeder : MonoBehaviour
    {
        public static Landspeeder Instance { get; private set; }

        private static SpeederTransform Transform => SpeederTransform.Instance;

        private readonly Controller _controller = Controller.Instance;

        private Rigidbody _rb;
        private Quaternion _initialRotation;
        private Quaternion _targetRotation;

        public float rotationSpeed = 5f;
        public float tiltAngleZ = 25f;
        public float tiltAngleY = 15f;

        public float jumpHeight = 5f;
        public float jumpDuration = 1f;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            _rb = GetComponent<Rigidbody>();
            _initialRotation = transform.rotation;
            _targetRotation = _initialRotation;
        }

        void Update()
        {
            /// These are just placeholders to showcase functionality
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DoABarrelRoll(1);
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                Jump();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
            }

            _controller.RunMovements(UnModifiedMovements());
            _controller.RunMovements(ModifiedMovements());

            SmoothRotation();
        }

        private List<Movement> UnModifiedMovements()
        {
            return new()
            {
                new()
                {
                    Direction = Direction.Down,
                    Modifier = Modifier.None,
                    Action = () =>
                        HoldRotation(-GetZAxisDirection() * 0f, tiltAngleY * Input.GetAxis("Horizontal"))
                },
                new()
                {
                    Direction = Direction.Right,
                    Modifier = Modifier.None,
                    Action = () =>
                        HoldRotation(-tiltAngleZ, tiltAngleY * Input.GetAxis("Horizontal"))
                },
                new()
                {
                    Direction = Direction.Left,
                    Modifier = Modifier.None,
                    Action = () =>
                        HoldRotation(tiltAngleZ, tiltAngleY * Input.GetAxis("Horizontal"))
                },
                new()
                {
                    Direction = Direction.None,
                    Modifier = Modifier.None,
                    Action = ResetRotation
                }
            };
        }

        private List<Movement> ModifiedMovements()
        {
            return new()
            {
                new()
                {
                    Direction = Direction.Down,
                    Modifier = Modifier.Shift,
                    Action = () =>
                        HoldRotation(-GetZAxisDirection() * 0f, tiltAngleY * Input.GetAxis("Horizontal"))
                },
                new()
                {
                    Direction = Direction.Up,
                    Modifier = Modifier.Shift,
                    Action = () =>
                        HoldRotation(GetZAxisDirection() * 180f, tiltAngleY * Input.GetAxis("Horizontal"))
                },
                new()
                {
                    Direction = Direction.Right,
                    Modifier = Modifier.Shift,
                    Action = () =>
                        HoldRotation(-90f, tiltAngleY * Input.GetAxis("Horizontal"))
                },
                new()
                {
                    Direction = Direction.Left,
                    Modifier = Modifier.Shift,
                    Action = () =>
                        HoldRotation(90f, tiltAngleY * Input.GetAxis("Horizontal"))
                },
                new()
                {
                    Direction = Direction.None,
                    Modifier = Modifier.Shift,
                    Action = ResetRotation
                }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numTimes">Number of times Barrel Roll is performed on the speeder</param>
        /// <param name="barrelRollSpeed">Amount of time taken to perform a barrel roll. Default is 0.25f</param>
        public void DoABarrelRoll(int numTimes, float barrelRollSpeed = 0.25f)
        {
            StartCoroutine(BarrelRollCoroutine(numTimes, barrelRollSpeed));
        }

        public static float GetZPos()
        {
            return Instance.transform.position.z;
        }

        public static float GetXPos()
        {
            return Instance.transform.position.x;
        }

        private IEnumerator BarrelRollCoroutine(int numTimes, float barrelRollSpeed)
        {
            Quaternion initialRotation = transform.rotation;
            float initialYRotation = initialRotation.eulerAngles.y;

            for (int i = 0; i < numTimes; i++)
            {
                float elapsedTime = 0f;
                float startAngle = transform.rotation.eulerAngles.z;
                float targetAngle = startAngle + 360f * GetZAxisDirection();

                while (elapsedTime < barrelRollSpeed)
                {
                    float t = elapsedTime / barrelRollSpeed;
                    float angle = Mathf.Lerp(startAngle, targetAngle, t);

                    Quaternion targetRotation = Quaternion.Euler(0f, initialYRotation, angle);
                    transform.rotation = targetRotation;

                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
            }

            ResetRotation();
        }

        public void Jump()
        {
            StartCoroutine(JumpCoroutine());
        }

        private IEnumerator JumpCoroutine()
        {
            Vector3 startPosition = transform.position;
            float elapsedTime = 0f;

            Quaternion startRotation = transform.rotation;
            Quaternion targetRotation = startRotation;

            float maxTiltAngle = Mathf.Clamp(jumpHeight * 10f, 0f, 90f);

            while (elapsedTime < jumpDuration)
            {
                float t = elapsedTime / jumpDuration;
                float heightOffset = jumpHeight * (4f * t * (1f - t));

                Vector3 newPosition = startPosition;
                newPosition.y = startPosition.y + heightOffset;

                transform.position = newPosition;

                float tiltAngle;
                if (t < 0.25f)
                {
                    tiltAngle = Mathf.Lerp(0f, maxTiltAngle, t * 4f);
                }
                else if (t < 0.55f)
                {
                    tiltAngle = maxTiltAngle;
                }
                else
                {
                    tiltAngle = Mathf.Lerp(maxTiltAngle, -maxTiltAngle,
                        (t - 0.55f) * 4f);
                }

                Quaternion tiltRotation = Quaternion.Euler(tiltAngle, 0f, 0f);
                targetRotation = GetRotationDuringJump(targetRotation);
                Quaternion combinedRotation = tiltRotation * targetRotation;
                transform.rotation =
                    Quaternion.Slerp(transform.rotation, combinedRotation, rotationSpeed * Time.deltaTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = Vector3.zero;
        }


        private Quaternion GetRotationDuringJump(Quaternion currentRotation)
        {
            if (_controller.IsPushingShift)
            {
                if (_controller.IsPushingDown)
                {
                    return Quaternion.Euler(-GetZAxisDirection() * 0f, tiltAngleY * Input.GetAxis("Horizontal"),
                        currentRotation.eulerAngles.z);
                }
                else if (_controller.IsPushingUp)
                {
                    return Quaternion.Euler(GetZAxisDirection() * 0f, tiltAngleY * Input.GetAxis("Horizontal"),
                        currentRotation.eulerAngles.z);
                }
                else if (_controller.IsPushingRight)
                {
                    return Quaternion.Euler(currentRotation.eulerAngles.x,
                        tiltAngleY * Input.GetAxis("Horizontal"), -90f);
                }
                else if (_controller.IsPushingLeft)
                {
                    return Quaternion.Euler(currentRotation.eulerAngles.x,
                        tiltAngleY * Input.GetAxis("Horizontal"), 90f);
                }
            }
            else
            {
                if (_controller.IsPushingDown)
                {
                    return Quaternion.Euler(-GetZAxisDirection() * 0f, tiltAngleY * Input.GetAxis("Horizontal"),
                        currentRotation.eulerAngles.z);
                }
                else if (_controller.IsPushingRight)
                {
                    return Quaternion.Euler(currentRotation.eulerAngles.x,
                        tiltAngleY * Input.GetAxis("Horizontal"), -tiltAngleZ);
                }
                else if (_controller.IsPushingLeft)
                {
                    return Quaternion.Euler(currentRotation.eulerAngles.x,
                        tiltAngleY * Input.GetAxis("Horizontal"), tiltAngleZ);
                }
            }

            return currentRotation;
        }

        private float GetZAxisDirection() => _controller.IsPushingLeft
            ? 1f
            : (!_controller.IsPushingLeft && !_controller.IsPushingRight)
                ? (Random.value < 0.5f ? 1f : -1f)
                : -1f;

        private void HoldRotation(float angleZ, float angleY)
        {
            _targetRotation = Quaternion.Euler(0f, angleY, angleZ);
        }

        private void ResetRotation()
        {
            _targetRotation = _initialRotation;
        }

        private void SmoothRotation()
        {
            Quaternion lockedRotation =
                Quaternion.Euler(0f, _targetRotation.eulerAngles.y, _targetRotation.eulerAngles.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, lockedRotation, rotationSpeed * Time.deltaTime);
        }

        void OnCollisionEnter(Collision other)
        {
            // print("Collision detected with " + other.gameObject.name);
            Destroy(other.gameObject);
        }
    }
}