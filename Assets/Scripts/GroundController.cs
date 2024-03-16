using UnityEngine;

public class GroundController : MonoBehaviour
{
    public float forwardSpeed = 10f;
    public float sidewaysSpeed = 5f;

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        transform.Translate(Vector3.back * (forwardSpeed * Time.deltaTime));

        transform.Translate(Vector3.right * (-horizontalInput * sidewaysSpeed * Time.deltaTime));

        if (transform.position.z <= -100f)
        {
            var transform1 = transform;
            var position = transform1.position;
            position = new Vector3(position.x, position.y, 0f);
            transform1.position = position;
        }
    }
}