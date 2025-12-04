using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIParallaxBreathVertical : MonoBehaviour
{
    [Header("Mpve")]
    public float verticalAmplitude = 20f;      
    public float verticalFrequency = 0.3f;     

    [Header("Zoom")]
    public float scaleAmplitude = 0.04f;       
    public float scaleFrequency = 0.3f;

    private RectTransform rect;
    private Vector2 basePos;
    private Vector3 baseScale;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        basePos = rect.anchoredPosition;
        baseScale = rect.localScale;
    }

    private void Update()
    {
        float tVert  = Time.time * verticalFrequency * Mathf.PI * 2f;
        float tScale = Time.time * scaleFrequency   * Mathf.PI * 2f;

        float yOffset = Mathf.Sin(tVert) * verticalAmplitude;
        rect.anchoredPosition = new Vector2(basePos.x, basePos.y + yOffset);

        float scaleOffset = Mathf.Sin(tScale) * scaleAmplitude;
        float scaleFactor = 1f + scaleOffset;
        rect.localScale = baseScale * scaleFactor;
    }
}
