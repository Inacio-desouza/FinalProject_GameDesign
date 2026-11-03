using UnityEngine;

// Shakes the camera harder the closer the bike gets to max speed
public class CameraShake : MonoBehaviour
{
    public BikeController bike;
    public float startShakingAt = 0.7f; // speed percent where shaking starts, 0.7 = 70%
    public float maxShake = 0.3f;       // how hard it shakes right before spinning out

    Vector3 startPos;                   // where the camera normally sits

    void Start()
    {
        startPos = transform.position;
    }

    void LateUpdate()
    {
        float percent = bike.SpeedPercent();
        float shake = 0f;

        if (!bike.dead && percent > startShakingAt)
        {
            // 0 when shaking starts, 1 at max speed
            float t = (percent - startShakingAt) / (1f - startShakingAt);

            // squaring it keeps it subtle at first, then ramps up fast near the end
            shake = t * t * maxShake;
        }

        // jitter around the normal position, or sit still if not shaking
        Vector3 offset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
        transform.position = startPos + offset * shake;
    }
}