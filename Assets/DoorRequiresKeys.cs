using UnityEngine;

public class DoorRequiresKeys : MonoBehaviour
{
    public int requiredKeys = 1;
    public KeyCode interactKey = KeyCode.E;

    public Transform doorToMove;
    public Vector3 openOffset = new Vector3(0, 0, 3f);
    public float openSpeed = 2f;

    Vector3 closedPos;
    Vector3 openPos;
    bool isOpen;

    bool playerInRange;
    PlayerInventory playerInv;
    UIMessage ui;

    void Start()
    {
        if (!doorToMove) doorToMove = transform;
        closedPos = doorToMove.position;
        openPos = closedPos + openOffset;

        ui = FindObjectOfType<UIMessage>();
    }

    void Update()
    {
        if (!playerInRange || !Input.GetKeyDown(interactKey))
            return;

        if (playerInv.HasKeys(requiredKeys))
        {
            isOpen = true;

            if (ui != null)
                ui.Show("Door opened!");
        }
        else
        {
            if (ui != null)
                ui.Show($"You need {requiredKeys} key(s) to open this door");
        }

        Vector3 target = isOpen ? openPos : closedPos;
        doorToMove.position =
            Vector3.Lerp(doorToMove.position, target, Time.deltaTime * openSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        var inv = other.GetComponent<PlayerInventory>() ?? other.GetComponentInParent<PlayerInventory>();
        if (inv == null) return;

        playerInv = inv;
        playerInRange = true;

        if (ui != null)
            ui.Show("Press E to open door");
    }

    void OnTriggerExit(Collider other)
    {
        var inv = other.GetComponent<PlayerInventory>() ?? other.GetComponentInParent<PlayerInventory>();
        if (inv == null) return;

        playerInv = null;
        playerInRange = false;
    }
}
