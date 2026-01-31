using UnityEngine;

public class TunnelInteraction : MonoBehaviour
{
    public GameObject Instruction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instruction.SetActive(false);
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
        if(Input.GetKeyDown(KeyCode.E))
        {
            Instruction.SetActive(false);
            Loader.Load(Loader.Scene.LastScene);
        }
    }
}
