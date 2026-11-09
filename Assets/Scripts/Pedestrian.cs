using UnityEngine;

// Walks along the sidewalk, sometimes runs across the street when the bike gets close
public class Pedestrian : MonoBehaviour
{
    public GameObject poseA;            // left foot model
    public GameObject poseB;            // right foot model

    public float walkSpeed = 1.2f;      // along the sidewalk
    public float crossSpeed = 3f;       // running across the road
    public float turnSpeed = 270f;      // degrees per second, lower = slower turn
    public float stepTime = 0.25f;      // seconds per step while walking
    public float crossChance = 0.5f;    // chance this person crosses at all
    public float sidewalkX = 4f;        // where the sidewalks are
    public Vector2 crossDistance = new Vector2(8f, 20f); // how far ahead of the bike they decide to cross

    Scroller scroller;
    BikeController bike;
    int walkDir;            // 1 = walking away from camera, -1 = toward it
    bool willCross;
    float crossAt;
    bool crossing;
    float targetX;
    float stepTimer;
    bool showA = true;
    float yaw;              // which way he's facing

    void Start()
    {
        scroller = GetComponent<Scroller>();
        bike = FindFirstObjectByType<BikeController>();

        // walk either direction along the sidewalk
        walkDir = Random.value < 0.5f ? 1 : -1;
        scroller.ownSpeed = walkSpeed * walkDir;
        yaw = walkDir > 0 ? 0f : 180f;
        transform.rotation = Quaternion.Euler(0, yaw, 0);

        // decide now whether and when he'll cross
        willCross = Random.value < crossChance;
        crossAt = Random.Range(crossDistance.x, crossDistance.y);
    }

    void Update()
    {
        if (bike.dead || !bike.started) return;

        // turn smoothly toward the direction he's going
        Quaternion target = Quaternion.Euler(0, yaw, 0);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target, turnSpeed * Time.deltaTime);
        bool facingRightWay = Quaternion.Angle(transform.rotation, target) < 5f;

        // swap feet to animate the steps, faster while running
        stepTimer += Time.deltaTime;
        float step = crossing ? stepTime * 0.6f : stepTime;
        if (stepTimer >= step)
        {
            stepTimer = 0f;
            showA = !showA;
            poseA.SetActive(showA);
            poseB.SetActive(!showA);
        }

        // start crossing once the bike gets close enough
        if (willCross && !crossing && transform.position.z - bike.transform.position.z < crossAt)
        {
            willCross = false;
            crossing = true;
            float side = Mathf.Sign(transform.position.x);
            targetX = -side * sidewalkX;   // the other sidewalk
            scroller.ownSpeed = 0f;        // stop walking along the sidewalk
            yaw = side > 0 ? -90f : 90f;   // face across the road
        }

        // only run across once he's done turning to face the road
        if (crossing && facingRightWay)
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.MoveTowards(pos.x, targetX, crossSpeed * Time.deltaTime);
            transform.position = pos;

            if (pos.x == targetX)
            {
                crossing = false;
                scroller.ownSpeed = walkSpeed * walkDir;
                yaw = walkDir > 0 ? 0f : 180f;
            }
        }
    }
}