using UnityEngine;

public class TurningCar : MonoBehaviour
{
    public float targetX;           // lane it's turning into
    public float roadSpeed;         // speed once on the road, negative = toward the bike
    public float crossSpeed = 4f;   // how fast it drives across
    public float turnSpeed = 300f;  // how fast it turns, degrees per second

    Scroller scroller;
    BikeController bike;
    bool onRoad;

    void Start()
    {
        scroller = GetComponent<Scroller>();
        bike = FindFirstObjectByType<BikeController>();
    }

    void Update()
    {
        if (bike.dead) return;

        if (!onRoad)
        {
            // drive across toward the lane
            Vector3 pos = transform.position;
            pos.x = Mathf.MoveTowards(pos.x, targetX, crossSpeed * Time.deltaTime);
            transform.position = pos;

            // reached the lane, start driving along the road
            if (pos.x == targetX)
            {
                onRoad = true;
                scroller.ownSpeed = roadSpeed;
            }
        }
        else
        {
            // turn to face along the road, away from you or toward you
            float angle = roadSpeed >= 0 ? 0f : 180f;
            Quaternion target = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, turnSpeed * Time.deltaTime);
        }
    }
}