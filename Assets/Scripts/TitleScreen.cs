using UnityEngine;
using TMPro;

public class TitleScreen : MonoBehaviour
{
    public BikeController bike;
    public GameObject titlePanel;
    public RectTransform logo;
    public TMP_Text tapText;

    float logoY;   // where the logo normally sits

    void Start()
    {
        logoY = logo.anchoredPosition.y;
    }

    void Update()
    {
        // only show the title until the run starts
        titlePanel.SetActive(!bike.started);
        if (bike.started) return;

        // blink the tap prompt
        tapText.alpha = 0.4f + 0.6f * Mathf.PingPong(Time.time * 1.5f, 1f);

        // logo gently bobs up and down
        float bob = Mathf.Sin(Time.time * 2f) * 12f;
        logo.anchoredPosition = new Vector2(logo.anchoredPosition.x, logoY + bob);
    }
}