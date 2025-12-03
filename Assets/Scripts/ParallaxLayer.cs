using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    public Vector2 amplitude = new Vector2(20f, 0f);   // jak daleko ma „chodzić”
    public Vector2 frequency = new Vector2(0.2f, 0f);  // jak szybko

    private RectTransform rt;
    private Vector2 startPos;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        if (rt != null)
            startPos = rt.anchoredPosition;
    }

    private void Update()
    {
        if (rt == null) return;

        float t = Time.time;
        float offsetX = Mathf.Sin(t * frequency.x) * amplitude.x;
        float offsetY = Mathf.Sin(t * frequency.y) * amplitude.y;

        rt.anchoredPosition = startPos + new Vector2(offsetX, offsetY);
    }
}
