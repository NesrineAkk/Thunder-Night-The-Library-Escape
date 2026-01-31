using UnityEngine;
using TMPro;
using System.Collections;

public class UIMessage : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public float showTime = 2f;

    Coroutine routine;

    void Start()
    {
        messageText.text = "";
    }

    public void Show(string msg)
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(ShowRoutine(msg));
    }

    IEnumerator ShowRoutine(string msg)
    {
        messageText.text = msg;
        yield return new WaitForSeconds(showTime);
        messageText.text = "";
    }
}
