using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var inv = other.GetComponent<PlayerInventory>() ?? other.GetComponentInParent<PlayerInventory>();
        if (inv == null) return;

        inv.AddKey(1);
        Destroy(gameObject);
    }
}
