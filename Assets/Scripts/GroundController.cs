using UnityEngine;
using uRandom = UnityEngine.Random;
using System.Collections.Generic;
using System;

[Serializable]
public class SizeTuple
{
    public float min;
    public float max;
}

public class GroundController : MonoBehaviour
{
    public static GroundController Instance { get; private set; }

    [Header("Set in Inspector")]
    public GameObject ground;

    public GameObject[] obstacles;
    public int[] maxPerObstacle;

    [SerializeField]
    public SizeTuple[] obstacleSizeRanges;

    public float verticalDistToGenNewPlane=500f;
    public float horizontalDistToGenNewPlane=250f;
    public float forwardSpeed=50f;
    public float sidewaysSpeed = 15f;
    
    public float destroyDistance = 100f;
    public int numPlanesWide = 4;
    public int numPlanesLong = 2;

    public float planeLength = 2000f;
    public float planeWidth = 500f;

    private float maxZPosition, minXPosition, maxXPosition;
    private List<GameObject> grounds = new List<GameObject>();
    private Dictionary<string, int> gameObjectCounts;

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
    }

    void Start()
    {
        gameObjectCounts = new Dictionary<string, int>();
        for (int i = 0; i < obstacles.Length; i++)
        {
            gameObjectCounts.Add(obstacles[i].name, maxPerObstacle[i]);
        }

        // Instantiate the first numPlanesLong * numPlanesWide ground objects to start the game
        for (int i = 0; i < numPlanesLong; i++) {
            for (int j = 0; j < numPlanesWide; j++) {
                Vector3 newPosition = new Vector3((j-Mathf.Floor(numPlanesWide/2)) * planeWidth + planeWidth / 2, ground.transform.position.y, i * planeLength);
                print("Generating plane at " + newPosition);
                GameObject currentPlane = Instantiate(ground, newPosition, Quaternion.identity);
                currentPlane.transform.SetParent(transform);
                grounds.Add(currentPlane);
                FillGround(currentPlane);
            }
        }

        UpdateMaxesAndMins();
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 forwardMovement = Vector3.back * (forwardSpeed * Time.deltaTime);
        Vector3 horizontalMovement = Vector3.right * (-horizontalInput * sidewaysSpeed * Time.deltaTime);
        Vector3 totalMovement = forwardMovement + horizontalMovement;

        List<GameObject> toRemove = new List<GameObject>();

        foreach (GameObject ground in grounds) {
            if (ground == null) {
                continue;
            }
            ground.transform.Translate(totalMovement);
            if (ground.transform.position.z + planeLength / 2 < Landspeeder.GetZPos() - verticalDistToGenNewPlane)
            {
                Destroy(ground);
                toRemove.Add(ground);
            }
        }

        foreach (GameObject ground in toRemove) {
            print("Removing ground from " + ground.transform.position.ToString());
            grounds.Remove(ground);
        }

        UpdateMaxesAndMins();

        if (maxZPosition < Landspeeder.GetZPos() + verticalDistToGenNewPlane)
        {
            print("Generating the next line of planes");
            for (int i = 0; i < numPlanesWide; i++) {
                GenerateNewPlane(new Vector3(minXPosition + i * planeWidth, ground.transform.position.y, maxZPosition + planeLength));
            }
            UpdateMaxesAndMins();
        }

        if (maxXPosition < Landspeeder.GetXPos() + horizontalDistToGenNewPlane)
        {  
            print("Generating the column of planes to the right");
            // Generate the rightmost of numPlanesLong new planes
            for (int i = 0; i < numPlanesLong; i++) {
                GenerateNewPlane(new Vector3(maxXPosition + planeWidth, ground.transform.position.y, maxZPosition - i * planeLength));
            }
            UpdateMaxesAndMins();
        }

        if (minXPosition > Landspeeder.GetXPos() - horizontalDistToGenNewPlane)
        {
            print("Generating the column of planes to the left");
            // Generate the leftmost of numPlanesLong new planes
            for (int i = 0; i < numPlanesLong; i++) {
                GenerateNewPlane(new Vector3(minXPosition - planeWidth, ground.transform.position.y, maxZPosition - i * planeLength));
            }
            UpdateMaxesAndMins();
        }
    }

    private void GenerateNewPlane(Vector3 position)
    {
        GameObject currentPlane = Instantiate(ground, position, Quaternion.identity);
        currentPlane.transform.SetParent(transform);
        FillGround(currentPlane);
        grounds.Add(currentPlane);
        print("Generating plane at " + position.ToString());
    }

    private GameObject MakeNewObstacle(float x, float y, float z, GameObject prefab)
    {
        float xPosition = uRandom.Range(x - planeWidth / 2, x + planeWidth / 2);
        float zPosition = uRandom.Range(z - planeLength / 2, z + planeLength / 2);
        Vector3 newPosition = new Vector3(xPosition, y, zPosition);
        GameObject newObj = Instantiate(prefab, newPosition, Quaternion.identity);
        return newObj;
    }

    private void FillGround(GameObject ground)
    {
        float x = ground.transform.position.x;
        float z = ground.transform.position.z;
        float y = ground.transform.position.y;
        for (int i = 0; i < obstacles.Length; i++)
        {
            int max = maxPerObstacle[i];
            for (int j = 0; j < max; j++)
            {
                GameObject newObstacle = MakeNewObstacle(x, y, z, obstacles[i]);
                float scale = uRandom.Range(obstacleSizeRanges[i].min, obstacleSizeRanges[i].max);
                newObstacle.transform.localScale = newObstacle.transform.localScale * scale;
                newObstacle.transform.SetParent(ground.transform);
            }
        }
    }

    private void UpdateMaxesAndMins() {
        maxZPosition = float.MinValue;
        maxXPosition = float.MinValue;
        minXPosition = float.MaxValue;

        foreach (GameObject ground in grounds) {
            if (ground == null) {continue;}
            maxZPosition = Mathf.Max(maxZPosition, ground.transform.position.z);
            maxXPosition = Mathf.Max(maxXPosition, ground.transform.position.x);
            minXPosition = Mathf.Min(minXPosition, ground.transform.position.x);
        }
    }
}