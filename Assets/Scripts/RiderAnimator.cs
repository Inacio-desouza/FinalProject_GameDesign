using UnityEngine;

public class RiderAnimator : MonoBehaviour
{
    public BikeController bike;
    public Transform model;          // holds the frame, wheels and rider, rocks side to side
    public GameObject[] legFrames;   // the leg poses, in pedaling order

    public float cadence = 0.12f;    // pedal turns per second for each unit of speed
    public float swayAngle = 6f;     // how far the bike rocks at top speed, in degrees

    float pedalTurns; // how far the pedals have gone around
    void Awake()
    {
        if (!bike) bike = GetComponent<BikeController>();
    }

    void Update()
    {
        if (bike.dead) return;

        // fixed gear: legs turn with the wheels and lock up when you skid
        if (!bike.skidding)
            pedalTurns += bike.speed * cadence * Time.deltaTime;

        // show the pose for where the pedals are right now
        int frame = (int)(pedalTurns * legFrames.Length) % legFrames.Length;
        for (int i = 0; i < legFrames.Length; i++)
            legFrames[i].SetActive(i == frame);

        // rock with each pedal stroke, more the faster you go, and hold still in a skid
        float sway = Mathf.Sin(pedalTurns * Mathf.PI * 2f) * swayAngle * bike.SpeedPercent();
        if (bike.skidding) sway = 0f;

        Quaternion target = Quaternion.Euler(0, 0, sway);
        model.localRotation = Quaternion.Lerp(model.localRotation, target, 10f * Time.deltaTime);
    }
}