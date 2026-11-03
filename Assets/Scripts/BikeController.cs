using UnityEngine;
using UnityEngine.SceneManagement;

public class BikeController : MonoBehaviour
{
    //Speed
    public float baseScrollSpeed = 7f; // normal road speed, ramps up over time
    public float scrollRamp = 0.08f;   // how fast the normal road speed ramps up
    public float scrollEase = 10f;     // how quickly the road catches up to you or settles back down
    public float speed = 7f;           // the bike's own speed
    public float accel = 0.25f;        // bike slowly speeds up on its own
    public float pedalBoost = 1.3f;    // speed added each pedal
    public float maxSpeed = 17f;       // reach this and you spin out
    public float skidDecel = 17f;      // how hard skidding slows you down

    [HideInInspector] public float scrollSpeed; // how fast the road is actually moving right now

    //road limit
    public float minZ = -5f;           // fall back past this and the screen catches you
    public float maxZ = 5f;            // the road speeds up once you reach this

    //lanes
    public float[] lanes = { -2f, 0f, 2f };
    public int lane = 1;
    public float laneSwitchSpeed = 20f;

    //skid swing
    public float skidAngle = 25f;      // how far the back swings out
    public float swingSpeed = 200f;    // degrees per second

    public bool skidding;              // set by BikeInput
    public bool dead;
    public float distance;             // score

    float holdTime;                    // how long the skid has been held
    int skidSide;                      // -1 = left, 1 = right, 0 = not skidding
    string deathReason;

    void Start()
    {
        scrollSpeed = baseScrollSpeed;
    }

    void Update()
    {
        if (dead) return;

        // normal road speed gets faster over time
        baseScrollSpeed += scrollRamp * Time.deltaTime;

        if (skidding)
        {
            // the longer you hold, the harder you brake, up to 1 second
            holdTime += Time.deltaTime;
            speed -= skidDecel * Mathf.Min(holdTime, 1f) * Time.deltaTime;
        }
        else
        {
            holdTime = 0f;
            speed += accel * Time.deltaTime;
        }
        if (speed < 0f) speed = 0f;

        // at the top of the screen and still faster than the road, so the road speeds up to match
        // otherwise the road settles back to its normal speed
        bool atTop = transform.position.z >= maxZ;
        float targetScroll = baseScrollSpeed;
        if (atTop && speed > baseScrollSpeed) targetScroll = speed;
        scrollSpeed = Mathf.MoveTowards(scrollSpeed, targetScroll, scrollEase * Time.deltaTime);

        // pick a random side when a skid starts, reset when it ends
        if (skidding && skidSide == 0) skidSide = Random.value < 0.5f ? -1 : 1;
        if (!skidding) skidSide = 0;

        // turn the bike toward that side, or back to straight
        Quaternion targetRotation = Quaternion.Euler(0, skidSide * skidAngle, 0);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, swingSpeed * Time.deltaTime);

        distance += speed * Time.deltaTime;

        Vector3 pos = transform.position;

        // faster than the road = move forward, slower = drift back
        pos.z += (speed - scrollSpeed) * Time.deltaTime;
        if (pos.z > maxZ) pos.z = maxZ;

        // slide toward the chosen lane
        pos.x = Mathf.MoveTowards(pos.x, lanes[lane], laneSwitchSpeed * Time.deltaTime);

        transform.position = pos;

        // the only speed penalty is spinning out
        if (pos.z < minZ) GameOver("Caught by the screen");
        else if (speed >= maxSpeed) GameOver("Spun out");
    }

    public void MoveLane(int direction)
    {
        lane = Mathf.Clamp(lane + direction, 0, lanes.Length - 1);
    }

    public void Pedal()
    {
        if (!skidding) speed += pedalBoost;
    }

    // 0 to 1, for the camera shake and speed bar
    public float SpeedPercent()
    {
        return Mathf.Clamp01(speed / maxSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle")) GameOver("Crashed");
    }

    void GameOver(string reason)
    {
        dead = true;
        skidding = false;
        deathReason = reason;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // temporary on-screen text
    void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 40;
        GUI.Label(new Rect(30, 60, 1000, 60), (int)distance + " m", style);
        GUI.Label(new Rect(30, 120, 1000, 60), "Speed: " + (int)(SpeedPercent() * 100) + "%", style);
        if (dead) GUI.Label(new Rect(30, 180, 1000, 60), deathReason + "! Space to restart", style);
    }
}