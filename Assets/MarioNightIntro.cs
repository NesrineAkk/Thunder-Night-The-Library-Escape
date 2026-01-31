using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MarioNightIntro : MonoBehaviour
{
    public PlayerStats stats;
    public Animator animator;   // assign the real Animator in Inspector
    public Image fadeImage;     // assign FadeImage in Inspector

    [Header("Timing")]
    public float waitBeforeFade = 3f;
    public float fadeToBlackTime = 3f;
    public float blackScreenTime = 2f;
    public float fadeFromBlackTime = 3f;

    [Header("Wake Control")]
    public float wakeUpHoldTime = 1.0f; // how long WakeUp is forced to play

    // Animator bool names
    public string isAwakeBool = "IsAwake";
    public string doWakeUpBool = "DoWakeUp";

    void Start()
    {
        stats = PlayerStats.Instance;
        if (!animator)
        {
            Debug.LogError("Animator NOT assigned (drag the child Animator that actually animates Mario).");
            enabled = false;
            return;
        }
        if (!fadeImage)
        {
            Debug.LogError("FadeImage NOT assigned.");
            enabled = false;
            return;
        }

        SetFadeAlpha(0f);

        // Start asleep
        animator.SetBool(isAwakeBool, false);
        animator.SetBool(doWakeUpBool, false);

        StartCoroutine(Routine());
    }

    IEnumerator Routine()
    {
        yield return new WaitForSeconds(waitBeforeFade);

        yield return Fade(0f, 1f, fadeToBlackTime);
        yield return new WaitForSeconds(blackScreenTime);
        yield return Fade(1f, 0f, fadeFromBlackTime);

        // Force WakeUp state
        animator.SetBool(doWakeUpBool, true);
        Debug.Log("DoWakeUp = true (forcing WakeUp state)");

        // Hold WakeUp for a moment so it can't be skipped
        yield return new WaitForSeconds(wakeUpHoldTime);

        // Now mark awake and allow transition to Idle
        animator.SetBool(isAwakeBool, true);
        animator.SetBool(doWakeUpBool, false);
        Debug.Log("IsAwake = true, DoWakeUp = false (go Idle)");
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        Color c = fadeImage.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(from, to, t / duration);
            fadeImage.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }

        fadeImage.color = new Color(c.r, c.g, c.b, to);
    }

    void SetFadeAlpha(float a)
    {
        var c = fadeImage.color;
        c.a = a;
        fadeImage.color = c;
    }
}
