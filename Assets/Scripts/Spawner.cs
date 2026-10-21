using UnityEngine;

public class Spawner : MonoBehaviour
{
    public BikeController bike;
    public GameObject dashPrefab;
    public GameObject obstaclePrefab;
    public GameObject buildingPrefab;

    public float spawnZ = 40f;
    public float dashGap = 3f;      // gaps between lines
    public float obstacleGap = 10f; // cars 
    public float buildingGap = 7f;  // building space

    // how far the road has moved since the last spawn of each thing
    float dashDistance;
    float obstacleDistance;
    float buildingDistance;

    void Start()
    {
        // fill road
        for (float z = -10f; z < spawnZ; z += dashGap) SpawnDashes(z);
        for (float z = -10f; z < spawnZ; z += buildingGap) SpawnBuildings(z);
    }

    void Update()
    {
        if (bike.dead) return;

        float moved = bike.scrollSpeed * Time.deltaTime;
        dashDistance += moved;
        obstacleDistance += moved;
        buildingDistance += moved;

        if (dashDistance >= dashGap)
        {
            dashDistance -= dashGap;
            SpawnDashes(spawnZ);
        }

        if (obstacleDistance >= obstacleGap)
        {
            obstacleDistance = 0f;
            SpawnObstacle();
        }

        if (buildingDistance >= buildingGap)
        {
            buildingDistance -= buildingGap;
            SpawnBuildings(spawnZ);
        }
    }

    void SpawnDashes(float z)
    {

        Instantiate(dashPrefab, new Vector3(-1f, 0.01f, z), Quaternion.identity);
        Instantiate(dashPrefab, new Vector3(1f, 0.01f, z), Quaternion.identity);
    }

    //random spawns for cars and buildings
    void SpawnObstacle()
    {
        int lane = Random.Range(0, bike.lanes.Length);
        Instantiate(obstaclePrefab, new Vector3(bike.lanes[lane], 0.6f, spawnZ), Quaternion.identity);
    }

    void SpawnBuildings(float z)
    {
        SpawnBuilding(-1, z); // left 
        SpawnBuilding(1, z);  // right 
    }

    void SpawnBuilding(int side, float z)
    {
        float width = Random.Range(2.5f, 4f);
        float height = Random.Range(2f, 7f);
        float length = Random.Range(4f, 6.5f);


        float x = side * (5.2f + width / 2f);
        GameObject b = Instantiate(buildingPrefab, new Vector3(x, height / 2f, z), Quaternion.identity);
        b.transform.localScale = new Vector3(width, height, length);

        // random color
        b.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 0.1f, 0.3f, 0.5f, 0.8f);
    }
}