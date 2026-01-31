using UnityEngine;

public class GateInteraction : MonoBehaviour
{
    public GameObject Instruction;
    public GameObject Notice;
    public GameObject win;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instruction.SetActive(false);
        Notice.SetActive(false);
        win.SetActive(false);
    }

    void OnTriggerEnter(Collider collision)
    {
        if(collision.transform.tag == "Player")
        {
            Instruction.SetActive(true);
        }
    }
    void OnTriggerExit(Collider collision)
    {
        if(collision.transform.tag == "Player")
        {
            Instruction.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            Instruction.SetActive(false);
            if(PlayerStats.Get().keys == 3)
            {
                win.SetActive(true);
            }
            else{
                Notice.SetActive(true);
            }
            
        }
    }
}
