using UnityEngine;

public class Spawner : MonoBehaviour
{
    public BikeController bike;
    public GameObject dashPrefab;
    public GameObject obstaclePrefab;
    public GameObject buildingPrefab;
    public GameObject intersectionPrefab;

    public float spawnZ = 40f;
    public float dashGap = 3f;          // gaps between lines
    public float buildingGap = 7f;      // building space

    // cars
    public float minDistance = 5f;      // smallest gap between cars
    public float maxDistance = 20f;     // biggest gap between cars
    public float movingCarChance = 0.4f;    // cars driving your way
    public float oncomingCarChance = 0.25f; // cars driving toward you, the rest are parked
    public float minCarSpeed = 2f;
    public float maxCarSpeed = 5f;      // keep below the road speed of 7

    // intersections
    public float intersectionChance = 0.25f; // chance a building spot becomes an intersection
    public float turningCarChance = 0.7f;    // chance a car pulls out of it
    public int minBlocksBetween = 6;         // building spots that must pass before another intersection

    float obstacleGap;                  // gap until the next car, re-rolled every spawn
    int blocksSinceIntersection = 4;    // counts up every building spot

    // how far the road has moved since the last spawn of each thing
    float dashDistance;
    float obstacleDistance;
    float buildingDistance;

    void Start()
    {
        obstacleGap = Random.Range(minDistance, maxDistance);

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
            obstacleGap = Random.Range(minDistance, maxDistance); // new random gap for the next car
        }

        if (buildingDistance >= buildingGap)
        {
            buildingDistance -= buildingGap;
            blocksSinceIntersection++;

            // only allow an intersection if enough blocks have passed since the last one
            bool allowed = blocksSinceIntersection >= minBlocksBetween;

            if (allowed && Random.value < intersectionChance)
            {
                SpawnIntersection(spawnZ);
                blocksSinceIntersection = 0; // start counting again
            }
            else
            {
                SpawnBuildings(spawnZ);
            }
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
        GameObject car = Instantiate(obstaclePrefab, new Vector3(bike.lanes[lane], 0.3f, spawnZ), Quaternion.identity);
        Scroller scroller = car.GetComponent<Scroller>();

        float roll = Random.value;
        if (roll < oncomingCarChance)
        {
            // driving toward you: negative speed, turned around
            scroller.ownSpeed = -Random.Range(minCarSpeed, maxCarSpeed);
            car.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (roll < oncomingCarChance + movingCarChance)
        {
            // driving your way
            scroller.ownSpeed = Random.Range(minCarSpeed, maxCarSpeed);
        }
        // otherwise parked, ownSpeed stays 0
    }

    void SpawnIntersection(float z)
    {
        // cross street laid over the road and sidewalks
        Instantiate(intersectionPrefab, new Vector3(0f, 0.1f, z), Quaternion.identity);

        if (Random.value < turningCarChance) SpawnTurningCar(z);
    }

    void SpawnTurningCar(float z)
    {
        // start off to the left or right side, facing the road
        int side = Random.value < 0.5f ? -1 : 1;
        Quaternion facingRoad = Quaternion.Euler(0, side == -1 ? 90 : -90, 0);
        GameObject car = Instantiate(obstaclePrefab, new Vector3(side * 12f, 0.3f, z), facingRoad);

        // pick a lane to turn into, and whether it turns with you or against you
        TurningCar turning = car.AddComponent<TurningCar>();
        turning.targetX = bike.lanes[Random.Range(0, bike.lanes.Length)];

        float speed = Random.Range(minCarSpeed, maxCarSpeed);
        turning.roadSpeed = Random.value < 0.5f ? speed : -speed;
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