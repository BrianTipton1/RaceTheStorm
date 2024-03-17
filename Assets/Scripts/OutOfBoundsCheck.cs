using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBoundsCheck : MonoBehaviour
{
    private float destroyDistance = 100f;

    private void Update()
    {
        Transform landspeeder = Landspeeder.Instance.transform;

        float destroyThreshold = landspeeder.position.z - destroyDistance;

        if (transform.position.z < destroyThreshold)
        {
            Destroy(gameObject);
        }
    }
}