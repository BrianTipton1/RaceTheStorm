using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Landspeeder : MonoBehaviour
{
    public static Landspeeder Instance { get; private set; }

    private Rigidbody _rb;
    private Quaternion _initialRotation;
    private Quaternion _targetRotation;

    public float rotationSpeed = 5f;
    public float tiltAngleZ = 25f;
    public float tiltAngleY = 15f;

    private bool IsPushingLeft => Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
    private bool IsPushingRight => Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
    private bool IsPushingUp => Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
    private bool IsPushingDown => Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
    private bool IsPushingShift => Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift);

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DoABarrelRoll(1);
        }

        HandleRotation();
        SmoothRotation();
    }

    private void HandleRotation()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (IsPushingShift)
        {
            if (IsPushingDown)
            {
                HoldRotation(-GetZAxisDirection() * 0f, tiltAngleY * horizontalInput);
            }
            else if (IsPushingUp)
            {
                HoldRotation(GetZAxisDirection() * 180f, tiltAngleY * horizontalInput);
            }
            else if (IsPushingRight)
            {
                HoldRotation(-90f, tiltAngleY * horizontalInput);
            }
            else if (IsPushingLeft)
            {
                HoldRotation(90f, tiltAngleY * horizontalInput);
            }
            else
            {
                ResetRotation();
            }
        }
        else
        {
            if (IsPushingDown)
            {
                HoldRotation(-GetZAxisDirection() * 0f, tiltAngleY * horizontalInput);
            }
            else if (IsPushingRight)
            {
                HoldRotation(-tiltAngleZ, tiltAngleY * horizontalInput);
            }
            else if (IsPushingLeft)
            {
                HoldRotation(tiltAngleZ, tiltAngleY * horizontalInput);
            }
            else
            {
                ResetRotation();
            }
        }
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

    private float GetZAxisDirection() => IsPushingLeft ? 1f :
        (!IsPushingLeft && !IsPushingRight) ? (Random.value < 0.5f ? 1f : -1f) : -1f;

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
        Quaternion lockedRotation = Quaternion.Euler(0f, _targetRotation.eulerAngles.y, _targetRotation.eulerAngles.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, lockedRotation, rotationSpeed * Time.deltaTime);
    }
}