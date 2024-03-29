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

        private SpeederTransform Transform => SpeederTransform.Instance;

        private readonly Controller _controller = Controller.Instance;

        private Rigidbody _rb;
        private Quaternion _initialRotation;
        private Quaternion _targetRotation;

        public bool IsJumping { get; private set; } = false;

        public float rotationSpeed = 5f;
        public float tiltAngleZ = 25f;
        public float tiltAngleY = 15f;

        public float jumpHeight = 5f;
        public float jumpDuration = 1f;

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

            _rb = GetComponent<Rigidbody>();
            _initialRotation = Transform.rotation;
            _targetRotation = _initialRotation;
        }

        void Update()
        {
            /// These are just placeholders to showcase functionality
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DoABarrelRoll(1);
            }

            if (Input.GetKeyDown(KeyCode.J) && !IsJumping)
            {
                Jump();
            }

            if (Input.GetKeyDown(KeyCode.R) && !Transform.IsReplaying)
            {
                Transform.Replay(Transform.GetMotionsNFramesBack(250), true);
            }

            _controller.RunMovements(UnModifiedMovements());
            _controller.RunMovements(ShiftModifiedMovements());

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
                        HoldRotation(-GetZAxisDirection() * 0f, tiltAngleY * _controller.HorizontalInput)
                },
                new()
                {
                    Direction = Direction.Right,
                    Modifier = Modifier.None,
                    Action = () =>
                        HoldRotation(-tiltAngleZ, tiltAngleY * _controller.HorizontalInput)
                },
                new()
                {
                    Direction = Direction.Left,
                    Modifier = Modifier.None,
                    Action = () =>
                        HoldRotation(tiltAngleZ, tiltAngleY * _controller.HorizontalInput)
                },
                new()
                {
                    Direction = Direction.None,
                    Modifier = Modifier.None,
                    Action = ResetRotation
                }
            };
        }

        private List<Movement> ShiftModifiedMovements()
        {
            return new()
            {
                new()
                {
                    Direction = Direction.Down,
                    Modifier = Modifier.Shift,
                    Action = () =>
                        HoldRotation(-GetZAxisDirection() * 0f, tiltAngleY * _controller.HorizontalInput)
                },
                new()
                {
                    Direction = Direction.Up,
                    Modifier = Modifier.Shift,
                    Action = () =>
                        HoldRotation(GetZAxisDirection() * 180f, tiltAngleY * _controller.HorizontalInput)
                },
                new()
                {
                    Direction = Direction.Right,
                    Modifier = Modifier.Shift,
                    Action = () =>
                        HoldRotation(-90f, tiltAngleY * _controller.HorizontalInput)
                },
                new()
                {
                    Direction = Direction.Left,
                    Modifier = Modifier.Shift,
                    Action = () =>
                        HoldRotation(90f, tiltAngleY * _controller.HorizontalInput)
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
            Quaternion initialRotation = Transform.rotation;
            float initialYRotation = initialRotation.eulerAngles.y;

            for (int i = 0; i < numTimes; i++)
            {
                float elapsedTime = 0f;
                float startAngle = Transform.rotation.eulerAngles.z;
                float targetAngle = startAngle + 360f * GetZAxisDirection();

                while (elapsedTime < barrelRollSpeed)
                {
                    float t = elapsedTime / barrelRollSpeed;
                    float angle = Mathf.Lerp(startAngle, targetAngle, t);

                    Quaternion targetRotation = Quaternion.Euler(0f, initialYRotation, angle);
                    Transform.rotation = targetRotation;

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
            IsJumping = true;
            Vector3 startPosition = Transform.position;
            float elapsedTime = 0f;

            Quaternion startRotation = Transform.rotation;
            Quaternion targetRotation = startRotation;

            float maxTiltAngle = Mathf.Clamp(jumpHeight * 100f, 0f, 90f);
            float tiltStartRatio = 0.4f; // Start tilting forward at 40% of the jump duration

            while (elapsedTime < jumpDuration)
            {
                float t = elapsedTime / jumpDuration;
                float heightOffset = jumpHeight * (4f * t * (1f - t));

                Vector3 newPosition = startPosition;
                newPosition.y = startPosition.y + heightOffset;

                Transform.position = newPosition;

                float tiltAngle;
                if (t < tiltStartRatio)
                {
                    tiltAngle = Mathf.Lerp(0f, -maxTiltAngle, t / tiltStartRatio);
                }
                else if (t < 0.5f)
                {
                    tiltAngle = Mathf.Lerp(-maxTiltAngle, -maxTiltAngle * 0.5f,
                        (t - tiltStartRatio) / (0.5f - tiltStartRatio));
                }
                else
                {
                    tiltAngle = Mathf.Lerp(-maxTiltAngle * 0.5f, 0f, (t - 0.5f) / 0.5f);
                }

                Quaternion tiltRotation = Quaternion.Euler(tiltAngle, 0f, 0f);
                targetRotation = GetRotationDuringJump(targetRotation);
                Quaternion combinedRotation = tiltRotation * targetRotation;
                Transform.rotation =
                    Quaternion.Slerp(Transform.rotation, combinedRotation, rotationSpeed * Time.deltaTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Transform.position = Vector3.zero;
            IsJumping = false;
        }

        private Quaternion GetRotationDuringJump(Quaternion currentRotation)
        {
            return _controller.RunMovements(
                new List<Movement<Quaternion>>()
                {
                    new()
                    {
                        Modifier = Modifier.Shift,
                        Direction = Direction.Down,
                        Action = () =>
                            Quaternion.Euler(-GetZAxisDirection() * 0f,
                                tiltAngleY * _controller.HorizontalInput,
                                currentRotation.eulerAngles.z)
                    },
                    new()
                    {
                        Modifier = Modifier.Shift,
                        Direction = Direction.Up,
                        Action = () => Quaternion.Euler(GetZAxisDirection() * 0f,
                            tiltAngleY * _controller.HorizontalInput,
                            currentRotation.eulerAngles.z)
                    },
                    new()
                    {
                        Modifier = Modifier.Shift,
                        Direction = Direction.Right,
                        Action = () => Quaternion.Euler(currentRotation.eulerAngles.x,
                            tiltAngleY * _controller.HorizontalInput, -90f)
                    },
                    new()
                    {
                        Modifier = Modifier.Shift,
                        Direction = Direction.Left,
                        Action = () => Quaternion.Euler(currentRotation.eulerAngles.x,
                            tiltAngleY * _controller.HorizontalInput, 90f)
                    },
                    new()
                    {
                        Modifier = Modifier.None,
                        Direction = Direction.Down,
                        Action = () => Quaternion.Euler(-GetZAxisDirection() * 0f,
                            tiltAngleY * _controller.HorizontalInput,
                            currentRotation.eulerAngles.z)
                    },
                    new()
                    {
                        Modifier = Modifier.None,
                        Direction = Direction.Right,
                        Action = () => Quaternion.Euler(currentRotation.eulerAngles.x,
                            tiltAngleY * _controller.HorizontalInput, -tiltAngleZ)
                    },
                    new()
                    {
                        Modifier = Modifier.None,
                        Direction = Direction.Left,
                        Action = () => Quaternion.Euler(currentRotation.eulerAngles.x,
                            tiltAngleY * _controller.HorizontalInput, tiltAngleZ)
                    },
                }
            )?.Result ?? currentRotation;
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
            Transform.rotation = Quaternion.Slerp(Transform.rotation, lockedRotation, rotationSpeed * Time.deltaTime);
        }

        void OnCollisionEnter(Collision other)
        {
            // Keep going up the hierarchy until we find a parent with an "Obstacle" tag
            Transform parent = other.transform;
            while (parent != null && parent.tag != "Obstacle")
            {
                parent = parent.parent;
            }
            if(parent) Destroy(parent.gameObject);
        }
    }
}