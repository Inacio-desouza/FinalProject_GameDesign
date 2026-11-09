using UnityEngine;

public class Scroller : MonoBehaviour
{
    public float ownSpeed = 0f;   // how fast it drives forward, 0 = parked
    public float destroyZ = -15f; // behind the camera

    BikeController bike;

    void Start()
    {
        bike = FindFirstObjectByType<BikeController>();
    }

    void Update()
    {
        if (bike.dead || !bike.started) return;

        // move toward camera, slower if it's driving forward
        transform.position += Vector3.back * (bike.scrollSpeed - ownSpeed) * Time.deltaTime;

        // destroy off screen
        if (transform.position.z < destroyZ) Destroy(gameObject);
    }
}