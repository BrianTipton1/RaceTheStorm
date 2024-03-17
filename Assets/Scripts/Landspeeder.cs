using System.Collections;
using UnityEngine;

public class Landspeeder : MonoBehaviour
{
    public static Landspeeder Instance { get; private set; }
    
    private Rigidbody _rb;
    private Quaternion _initialRotation;
    private Quaternion _targetRotation;

    public float rotationSpeed = 5f;
    public float tiltAngle = 25f;

    private bool PushLeft => Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
    private bool PushRight => Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
    private bool PushUp => Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);

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
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (PushRight)
            {
                HoldRotation(-90f);
            }
            else if (PushLeft)
            {
                HoldRotation(90f);
            }
            else if (PushUp)
            {
                HoldRotation(180f);
            }
            else
            {
                ResetRotation();
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.D))
            {
                HoldRotation(-tiltAngle);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                HoldRotation(tiltAngle);
            }
            else
            {
                ResetRotation();
            }
        }
    }

    public void DoABarrelRoll(int numTimes)
    {
        StartCoroutine(BarrelRollCoroutine(numTimes));
    }

    private IEnumerator BarrelRollCoroutine(int numTimes)
    {
        float speed = 0.25f;
        float direction = GetBarrelRollDirection();
        for (int i = 0; i < numTimes; i++)
        {
            float elapsedTime = 0f;
            float startAngle = transform.rotation.eulerAngles.z;
            float targetAngle = startAngle + 360f * direction;
            while (elapsedTime < speed)
            {
                float t = elapsedTime / speed;
                float angle = Mathf.Lerp(startAngle, targetAngle, t);
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        ResetRotation();
    }

    private float GetBarrelRollDirection() => PushLeft || (!PushLeft && !PushRight) ? 1f : -1f;

    private void HoldRotation(float angle)
    {
        _targetRotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void ResetRotation()
    {
        _targetRotation = _initialRotation;
    }

    private void SmoothRotation()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, rotationSpeed * Time.deltaTime);
    }
}