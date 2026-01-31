using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFade : MonoBehaviour
{
    public Image fadeImage;

    public float fadeToBlackTime = 2f;
    public float blackScreenTime = 2f;
    public float fadeFromBlackTime = 2f;

    void Start()
    {
        StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        // Fade TO black
        yield return Fade(0f, 1f, fadeToBlackTime);

        // Stay black (time passes)
        yield return new WaitForSeconds(blackScreenTime);

        // Fade FROM black
        yield return Fade(1f, 0f, fadeFromBlackTime);
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
}
