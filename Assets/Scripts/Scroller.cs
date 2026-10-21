using UnityEngine;

public class Scroller : MonoBehaviour
{
    public float destroyZ = -15f; // behind the camera

    BikeController bike;

    void Start()
    {
        bike = FindFirstObjectByType<BikeController>();
    }

    void Update()
    {
        if (bike.dead) return;

        // move toward camera
        transform.position += Vector3.back * bike.scrollSpeed * Time.deltaTime;

        // destroy off screen
        if (transform.position.z < destroyZ) Destroy(gameObject);
    }
}