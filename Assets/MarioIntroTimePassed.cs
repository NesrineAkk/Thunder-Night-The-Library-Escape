using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class MarioIntroTimePassed : MonoBehaviour
{
    public PlayerStats stats;
    [Header("UI Fade (assign this)")]
    public Image fadeImage; // drag Canvas/FadeImage here

    [Header("Timing")]
    public float waitBeforeFade = 3f;
    public float fadeToBlackTime = 3f;
    public float blackScreenTime = 2f;
    public float fadeFromBlackTime = 3f;

    [Header("Wake Bool Param")]
    public string doWakeUpBool = "DoWakeUp"; // must exist in Animator
    public float wakeUpHoldTime = 1.0f;

    private Animator animator;
    private Behaviour movementScript; // works even if your movement class name changes

    void Awake()
    {
        stats = PlayerStats.Instance;
        animator = GetComponent<Animator>();

        // Find your movement script on the same object (Mario Movement (Script))
        movementScript = GetComponent<MonoBehaviour>(); // fallback (we'll override below)

        // Prefer exact type if present
        var mm = GetComponent<MarioMovement>();
        if (mm != null) movementScript = mm;

        if (!fadeImage)
            Debug.LogWarning("MarioIntroTimePassed: FadeImage not assigned yet.");
    }

    void Start()
    {
        stats = PlayerStats.Instance;
        if (!fadeImage)
        {
            Debug.LogError("MarioIntroTimePassed: FadeImage is NONE. Drag Canvas/FadeImage into the field.");
            enabled = false;
            return;
        }

        // Start with screen visible
        SetFadeAlpha(0f);

        // Disable movement during intro
        if (movementScript) movementScript.enabled = false;

        // Ensure asleep at start
        animator.SetBool(doWakeUpBool, false);

        StartCoroutine(Routine());
    }

    IEnumerator Routine()
    {
        yield return new WaitForSeconds(waitBeforeFade);

        yield return Fade(0f, 1f, fadeToBlackTime);
        yield return new WaitForSeconds(blackScreenTime);
        yield return Fade(1f, 0f, fadeFromBlackTime);

        // Wake up (force wake state)
        animator.SetBool(doWakeUpBool, true);
        Debug.Log("DoWakeUp=true on " + animator.gameObject.name);

        yield return new WaitForSeconds(wakeUpHoldTime);

        animator.SetBool(doWakeUpBool, false);
        Debug.Log("DoWakeUp=false (allow idle/movement)");

        // Re-enable movement after intro
        if (movementScript) movementScript.enabled = true;

        // Optional: disable this intro script so it never runs again
        enabled = false;
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        if (duration <= 0f)
        {
            SetFadeAlpha(to);
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(from, to, t / duration);
            SetFadeAlpha(a);
            yield return null;
        }
        SetFadeAlpha(to);
    }

    void SetFadeAlpha(float a)
    {
        Color c = fadeImage.color;
        c.a = a;
        fadeImage.color = c;
    }
}
