using UnityEngine;

public class WheelSpin : MonoBehaviour
{
    public BikeController bike;
    public Transform model;           // the wheels spin around its sideways axis
    public Transform frontWheel;
    public Transform backWheel;
    public float wheelRadius = 0.35f; // bigger = slower spin
    public float direction = 1f;      // set to -1 if the wheels spin backwards

    void Update()
    {
        if (bike.dead || !bike.started) return;

        // turn just enough each frame to roll at the bike's speed
        float degrees = bike.speed / wheelRadius * Mathf.Rad2Deg * Time.deltaTime * direction;

        // the front wheel always rolls
        frontWheel.Rotate(model.right, degrees, Space.World);

        // fixed gear: the back wheel locks when you skid
        if (!bike.skidding) backWheel.Rotate(model.right, degrees, Space.World);
    }
}