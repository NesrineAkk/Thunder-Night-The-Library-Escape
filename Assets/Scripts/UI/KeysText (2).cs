using UnityEngine;
using TMPro;


public class KeysText : MonoBehaviour
{
    public TextMeshProUGUI keys;
    private PlayerStats manager;

    void Start(){
        manager = PlayerStats.Get();
        if(manager == null)
        {
            Debug.Log("manager not found");
        }

        SetText(manager.keys);
    }

    void Update()
    {
        SetText(manager.keys);
    }

    public void SetText(float amount)
    {
        keys.text = amount.ToString();
    }

}
