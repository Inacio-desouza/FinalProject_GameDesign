using UnityEngine;

// Lays out four crosswalks in a box around the intersection
public class CrossWalks : MonoBehaviour
{
    public Material stripeMaterial;
    public Transform street;          // the cross street, found automatically if left empty

    public float boxSize = 3.8f;      // distance from the center to each crosswalk
    public float stripeLength = 1.5f; // how long each stripe is
    public float stripeWidth = 0.4f;  // how thick each stripe is
    public float stripeGap = 0.8f;    // spacing between stripes
    public float halfWidth = 2.6f;    // how far stripes go out from the middle of each crosswalk

    void Start()
    {
        if (!street) street = transform.Find("Street");

        // sit just on top of the cross street
        float streetTop = street.localPosition.y + street.localScale.y / 2f + 0.01f;

        // sit just on top of the main road, which is at height 0 in the world
        float roadTop = 0.02f - transform.position.y;

        for (float i = -halfWidth; i <= halfWidth; i += stripeGap)
        {
            // top and bottom: across the main road, stripes run up and down
            MakeStripe(new Vector3(i, roadTop, boxSize), new Vector3(stripeWidth, 0.02f, stripeLength));
            MakeStripe(new Vector3(i, roadTop, -boxSize), new Vector3(stripeWidth, 0.02f, stripeLength));

            // left and right: across the cross street, stripes run side to side
            MakeStripe(new Vector3(boxSize, streetTop, i), new Vector3(stripeLength, 0.02f, stripeWidth));
            MakeStripe(new Vector3(-boxSize, streetTop, i), new Vector3(stripeLength, 0.02f, stripeWidth));
        }
    }

    void MakeStripe(Vector3 position, Vector3 size)
    {
        GameObject stripe = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Destroy(stripe.GetComponent<Collider>()); // decoration only

        // attach to the intersection so it scrolls with it
        stripe.transform.SetParent(transform, false);
        stripe.transform.localPosition = position;
        stripe.transform.localScale = size;
        stripe.GetComponent<Renderer>().material = stripeMaterial;
    }
}