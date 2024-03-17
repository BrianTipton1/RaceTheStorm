using System.Collections;
using UnityEngine;

public class Landspeeder : MonoBehaviour
{
    public static Landspeeder Instance { get; private set; }
    private Rigidbody rb;
    private Quaternion initialRotation;
    private Quaternion targetRotation;
    
    public float rotationSpeed = 5f;
    public float tiltAngle = 25f;
    
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

        rb = GetComponent<Rigidbody>();
        initialRotation = transform.rotation;
        targetRotation = initialRotation;
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
            if (Input.GetKey(KeyCode.D))
            {
                HoldRotation(-90f);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                HoldRotation(90f);
            }
            else if (Input.GetKey(KeyCode.W))
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
        for (int i = 0; i < numTimes; i++)
        {
            float elapsedTime = 0f;
            float startAngle = transform.rotation.eulerAngles.z;
            float targetAngle = startAngle + 360f;

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

    private void HoldRotation(float angle)
    {
        targetRotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void ResetRotation()
    {
        targetRotation = initialRotation;
    }

    private void SmoothRotation()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}