using UnityEngine;

public class KeyFloatGlow : MonoBehaviour
{
    [Header("Float")]
    public float floatHeight = 0.3f;
    public float floatSpeed = 2f;

    [Header("Rotate")]
    public float rotationSpeed = 60f;

    [Header("Glow")]
    public Renderer keyRenderer;          // assign in Inspector (or auto)
    public Color glowColor = Color.yellow;
    public float glowMin = 0.8f;
    public float glowMax = 2.5f;
    public float glowSpeed = 2f;

    private Vector3 startPos;
    private Material mat;

    void Start()
    {
        startPos = transform.position;

        if (!keyRenderer)
            keyRenderer = GetComponentInChildren<Renderer>();

        if (keyRenderer != null)
        {
            mat = keyRenderer.material;
            mat.EnableKeyword("_EMISSION");
        }
        else
        {
            Debug.LogWarning("KeyFloatGlow: No Renderer found on key.");
        }
    }

    void Update()
    {
        // float
        float y = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = startPos + Vector3.up * y;

        // rotate
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

        // glow pulse
        if (mat != null)
        {
            float g = Mathf.Lerp(glowMin, glowMax, (Mathf.Sin(Time.time * glowSpeed) + 1f) / 2f);
            mat.SetColor("_EmissionColor", glowColor * g);
        }
    }
}
