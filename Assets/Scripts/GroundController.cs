using UnityEngine;

public class GroundController : MonoBehaviour
{
    public float forwardSpeed = 50f;
    public float sidewaysSpeed = 15f;
    
    public float destroyDistance = 100f;
    public float planeLength = 200f;

    private float lastZPosition;
    private GameObject currentPlane;

    private void Start()
    {
        lastZPosition = transform.position.z + planeLength;
        currentPlane = gameObject;
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.back * (forwardSpeed * Time.deltaTime));
        transform.Translate(Vector3.right * (-horizontalInput * sidewaysSpeed * Time.deltaTime));

        if (currentPlane.transform.position.z <= Landspeeder.Instance.transform.position.z - destroyDistance)
        {
            Destroy(currentPlane);
            GenerateNewPlane();
        }
    }

    private void GenerateNewPlane()
    {
        Vector3 newPosition = new Vector3(0f, transform.position.y, lastZPosition);
        currentPlane = Instantiate(gameObject, newPosition, Quaternion.identity);
        currentPlane.transform.SetParent(transform.parent);
        currentPlane.GetComponent<GroundController>().enabled = true;

        lastZPosition += planeLength;
    }
}