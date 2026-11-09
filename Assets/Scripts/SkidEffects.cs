using UnityEngine;

public class SkidEffects : MonoBehaviour
{
    public BikeController bike;
    public Transform backWheel;
    public GameObject skidMarkPrefab;
    public ParticleSystem dust;

    public float markSpacing = 0.15f; // gap between skid mark pieces
    public float dustAmount = 40f;    // puffs per second while skidding

    float distanceSinceMark;

    void Update()
    {
        // only skid while actually moving
        bool skidding = bike.skidding && !bike.dead && bike.speed > 0.5f;

        // drop marks under the back wheel as the bike slides
        if (skidding)
        {
            distanceSinceMark += bike.speed * Time.deltaTime;
            if (distanceSinceMark >= markSpacing)
            {
                distanceSinceMark = 0f;
                Vector3 pos = new Vector3(backWheel.position.x, 0.015f, backWheel.position.z);
                Quaternion angle = Quaternion.Euler(0, transform.eulerAngles.y, 0); // angled with the skid swing
                Instantiate(skidMarkPrefab, pos, angle);
            }
        }
        else
        {
            distanceSinceMark = markSpacing; // drop a mark right away on the next skid
        }

        // dust only while skidding
        var emission = dust.emission;
        emission.rateOverTime = skidding ? dustAmount : 0f;

        // puffs drift back with the road instead of hanging in the air
        var velocity = dust.velocityOverLifetime;
        velocity.x = 0f;
        velocity.y = 0f;
        velocity.z = -bike.scrollSpeed;
    }
}