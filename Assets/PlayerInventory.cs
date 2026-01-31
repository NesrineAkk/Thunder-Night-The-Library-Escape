using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int keyCount = 0;

    UIMessage ui;

    void Awake()
    {
        ui = FindObjectOfType<UIMessage>();
    }

    public void AddKey(int amount = 1)
    {
        keyCount += amount;

        if (ui != null)
            ui.Show($"You picked up a key ({keyCount}/3)");
    }

    public bool HasKeys(int required)
    {
        return keyCount >= required;
    }
}
