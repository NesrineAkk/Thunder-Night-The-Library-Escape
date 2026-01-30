using UnityEngine;
using TMPro;


public class KeysText : MonoBehaviour
{
    public TextMeshProUGUI keys;

    public void SetText(float amount)
    {
        keys.text = amount.ToString();
    }

}
