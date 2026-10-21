using UnityEngine;
using UnityEngine.SceneManagement;

public class BikeController : MonoBehaviour
{
    //Speed
    public float scrollSpeed = 7f;   // scroll speed 
    public float scrollRamp = 0.08f; // road speeds up over time
    public float speed = 7f;         // bike speed
    public float accel = 0.25f;      // bike acceleration
    public float pedalBoost = 1.3f;  // speed added each pedal
    public float maxSpeed = 17f;     // spin out
    public float skidDecel = 17f;    // skid deceleration

    //road limit
    public float minZ = -5f;         // keep up with screen
    public float maxZ = 5f;          // too far ahead

    //lanes
    public float[] lanes = { -2f, 0f, 2f };
    public int lane = 1;
    public float laneSwitchSpeed = 20f;

    //skid swing
    public float skidAngle = 25f;    // steeze
    public float swingSpeed = 200f;

    public bool skidding;
    public bool dead;
    public float distance;           // score

    float holdTime;                  // skid holdtime
    int skidSide;                    // -1 = left, 1 = right, 0 = not skidding
    string deathReason;

    void Update()
    {
        if (dead) return;

        // road gets faster over time
        scrollSpeed += scrollRamp * Time.deltaTime;

        if (skidding)
        {
            // longer you hold harder you brake, up to 1 second
            holdTime += Time.deltaTime;
            speed -= skidDecel * Mathf.Min(holdTime, 1f) * Time.deltaTime;
        }
        else
        {
            holdTime = 0f;
            speed += accel * Time.deltaTime;
        }
        if (speed < 0f) speed = 0f;

        // random side for skids
        if (skidding && skidSide == 0) skidSide = Random.value < 0.5f ? -1 : 1;
        if (!skidding) skidSide = 0;

        // turn that side
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

        if (pos.z < minZ) GameOver("Caught by the screen");
        else if (speed >= maxSpeed) GameOver("Spun out");
    }

    public void MoveLane(int direction)
    {
        lane = Mathf.Clamp(lane + direction, 0, lanes.Length - 1);
    }

    public void Pedal()
    {
        if (!skidding)
        {
            speed += pedalBoost;
        }
    }

    // 0 to 1, for the speed bar later
    public float SpeedPercent()
    {
        return Mathf.Clamp01(speed / maxSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            GameOver("Crashed");
        }
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

    // gameover text
    void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 40;
        GUI.Label(new Rect(30, 60, 1000, 60), (int)distance + " m", style);
        if (dead) GUI.Label(new Rect(30, 120, 1000, 60), deathReason + "! Space to restart", style);
    }
}